using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Constants.System;
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Identity.Permission;
using HRM_BE.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HRM_BE.Api.Services
{
    public class PermissionService: IPermissionService
    {
        private readonly HrmContext _dbContext;
        private readonly IMapper _mapper;


        public PermissionService(HrmContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private List<PermissionDto> GetChildren(int parentId)
        {
            var children = _dbContext.Permissions
                .Where(p => p.ParentPermissionId == parentId)
                .ToList();

            var childDtos = new List<PermissionDto>();

            foreach (var childPermission in children)
            {
                var childDto = _mapper.Map<PermissionDto>(childPermission);
                childDto.Childrens = GetChildren(childPermission.Id);
                childDtos.Add(childDto);
            }

            return childDtos;
        }


        public async Task<PagingResult<PermissionDto>> GetPaging(GetPermissionRequest request)
        {
            try
            {
                var permissions = await GetRecursive(null);

                if (!string.IsNullOrWhiteSpace(request.Keyword) ||
                    request.Section.HasValue)
                {
                    // Keyword chung: ưu tiên Name, rồi DisplayName, rồi Description
                    var keyword = request.Keyword?.Trim();
                    var hasKeyword = !string.IsNullOrWhiteSpace(keyword);
                    var sectionFilter = request.Section;

                    bool Match(PermissionDto p)
                    {
                        if (p == null) return false;

                        // Nếu có keyword thì check trên 3 trường: Name, DisplayName, Description
                        var isKeywordMatch = !hasKeyword ||
                                             (!string.IsNullOrEmpty(p.Name) &&
                                              p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                             (!string.IsNullOrEmpty(p.DisplayName) &&
                                              p.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                             (!string.IsNullOrEmpty(p.Description) &&
                                              p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));

                        if (!isKeywordMatch) return false;

                        if (sectionFilter.HasValue && p.Section != sectionFilter)
                            return false;

                        return true;
                    }

                    List<PermissionDto> FilterTree(IEnumerable<PermissionDto> nodes)
                    {
                        var result = new List<PermissionDto>();
                        foreach (var node in nodes ?? Enumerable.Empty<PermissionDto>())
                        {
                            var filteredChildren = FilterTree(node.Childrens);
                            var isMatch = Match(node) || filteredChildren.Count > 0;
                            if (!isMatch) continue;

                            node.Childrens = filteredChildren;
                            result.Add(node);
                        }

                        return result;
                    }

                    permissions = FilterTree(permissions);
                }

                var total = permissions.Count;

                if (request.PageIndex == null) request.PageIndex = 1;
                if (request.PageSize == null) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize);

                if (string.IsNullOrEmpty(request.SortBy) || request.SortBy == SortByConstant.Desc)
                {
                    permissions = request.OrderBy switch
                    {
                        OrderByConstant.Id or _ => permissions.OrderByDescending(p => p.Id).ToList(),
                    };
                }
                else if (request.SortBy == SortByConstant.Asc)
                {
                    permissions = request.OrderBy switch
                    {
                        OrderByConstant.Id or _ => permissions.OrderBy(p => p.Id).ToList(),
                    };
                }

                var items = permissions
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var result = new PagingResult<PermissionDto>(items, request.PageIndex, request.PageSize, request.SortBy,
                    request.OrderBy, total);

                return result;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        private async Task<List<PermissionDto>> GetRecursive(int? parentPermissionId)
        {
            var children = _dbContext.Permissions
                 .Where(p => p.ParentPermissionId == parentPermissionId)
                 .ToList();

            var childDtos = new List<PermissionDto>();

            foreach (var childPermission in children)
            {
                var childDto = _mapper.Map<PermissionDto>(childPermission);
                childDto.Childrens = GetChildren(childPermission.Id);
                childDtos.Add(childDto);
            }

            return childDtos;
        }


        public async Task<List<PermissionDto>> GetByRoleId(int roleId)
        {
            try
            {
                var rolePermissions = await _dbContext.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Include(rp => rp.Permission)
                    .ToListAsync();

                var permissions = rolePermissions.Select(rp => rp.Permission);

                var result = await GetRecursive(null, permissions);

                return result;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        private async Task<List<PermissionDto>> GetRecursive(int? parentPermissionId, IEnumerable<Permission> allPermissions)
        {
            var children = allPermissions
                .Where(p => p.ParentPermissionId == parentPermissionId)
                .ToList();

            var childDtos = new List<PermissionDto>();

            foreach (var childPermission in children)
            {
                var childDto = _mapper.Map<PermissionDto>(childPermission);
                childDto.Childrens = await GetRecursive(childPermission.Id, allPermissions);
                childDtos.Add(childDto);
            }

            return childDtos;
        }


        public async Task<PermissionDto> Create(CreatePermissionRequest request)
        {
            try
            {
                var permission = _mapper.Map<Permission>(request);
                permission.Name = request.Name?.Trim();
                permission.DisplayName = request.DisplayName?.Trim();
                permission.CreatedAt = DateTime.Now;

                await _dbContext.Permissions.AddAsync(permission);
                await _dbContext.SaveChangesAsync();

                var permissionDto = _mapper.Map<PermissionDto>(permission);

                return permissionDto;
            }
            catch(Exception ex) {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }

        }

        public async Task Update(int id, UpdatePermissionRequest request)
        {
            try
            {
                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission == null)
                {
                    throw new ApiException("Không tìm thấy quyền hạn", HttpStatusCodeConstant.NotFound);
                }

                if (request.ParentPermissionId.HasValue && request.ParentPermissionId.Value == id)
                {
                    throw new ApiException("Quyền cha không hợp lệ", HttpStatusCodeConstant.BadRequest);
                }

                if (request.ParentPermissionId.HasValue)
                {
                    var parentExists = await _dbContext.Permissions.AnyAsync(p => p.Id == request.ParentPermissionId.Value);
                    if (!parentExists)
                    {
                        throw new ApiException("Không tìm thấy quyền cha", HttpStatusCodeConstant.BadRequest);
                    }
                    permission.ParentPermissionId = request.ParentPermissionId;
                }

                if (request.Name != null)
                {
                    permission.Name = request.Name.Trim();
                }

                if (request.DisplayName != null)
                {
                    permission.DisplayName = request.DisplayName.Trim();
                }

                if (request.Description != null)
                {
                    permission.Description = request.Description;
                }

                if (request.Section.HasValue)
                {
                    permission.Section = request.Section;
                }

                permission.UpdatedAt = DateTime.Now;
                _dbContext.Permissions.Update(permission);
                await _dbContext.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission == null)
                {
                    throw new ApiException("Không tìm thấy quyền hạn", HttpStatusCodeConstant.NotFound);
                }

                var hasChildren = await _dbContext.Permissions.AnyAsync(p => p.ParentPermissionId == id);
                if (hasChildren)
                {
                    throw new ApiException("Không thể xóa quyền đang có quyền con", HttpStatusCodeConstant.BadRequest);
                }

                var isAssigned = await _dbContext.RolePermissions.AnyAsync(rp => rp.PermissionId == id);
                if (isAssigned)
                {
                    throw new ApiException("Không thể xóa quyền đã được gán cho vai trò", HttpStatusCodeConstant.BadRequest);
                }

                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }


    }
}
