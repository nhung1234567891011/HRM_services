using AutoMapper;
using Azure.Core;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Company;
using HRM_BE.Core.Models.Organization;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRM_BE.Data.Repositories
{
    public class OrganizationRepository : RepositoryBase<Organization, int>, IOrganizationRepository
    {
        private readonly IMapper _mapper;
        public OrganizationRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<OrganizationDto> Create(CreateOrganizationRequest request)
        {
            var entity = _mapper.Map<Organization>(request);
            var organizationCreated = await CreateAsync(entity);
            var orgnizationId = organizationCreated.Id;
            foreach (var item in request.OrganizationLeaders)
            {
                var orginiazationLeaderEnity = new OrganizationLeader
                {
                    OrganizationLeaderType = OrganizationLeaderType.Leader,
                    EmployeeId = item.EmployeeId,
                    OrganizationId = orgnizationId
                };
                await _dbContext.OrganizationLeaders.AddAsync(orginiazationLeaderEnity);
                await SaveChangesAsync();
            }

            return _mapper.Map<OrganizationDto>(entity);
        }

        public async Task Delete(int id)
        {
            var entity = await GetOrganizationAndCheckExist(id);

            // Không được xóa đơn vị cha khi còn đơn vị con
            var hasChildren = await _dbContext.Organizations
                .AsNoTracking()
                .AnyAsync(o => o.OrganizatioParentId == id && o.IsDeleted == false);
            if (hasChildren)
            {
                throw new EntityAlreadyExistsException("Không thể xóa đơn vị vì còn đơn vị con.");
            }

            // Không được xóa đơn vị đang có nhân viên
            var hasEmployees = await _dbContext.Employees
                .AsNoTracking()
                .AnyAsync(e => e.OrganizationId == id && e.IsDeleted == false);
            if (hasEmployees)
            {
                throw new EntityAlreadyExistsException("Không thể xóa đơn vị vì đang có nhân viên thuộc đơn vị này.");
            }

            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }
        public async Task<OrganizationDto> GetById(int id)
        {
            var entity = await GetOrganizationAndCheckExist(id);

            var entity2 = await _dbContext.Organizations.AsNoTracking()
               .Include( o => o.OrganizationChildren).ThenInclude( o => o.OrganizationChildren)
               .Include(o => o.OrganizationType)    
               .Include(o => o.OrganizationLeaders)
               .ThenInclude(ol => ol.Employee)
               .FirstOrDefaultAsync(o => o.Id == id);
            var entityReturn = _mapper.Map<OrganizationDto>(entity2);

            //var childrend = await this.GetAllChildrenAsync(id);
            //entityReturn.OrganizationChildren =_mapper.Map<List<OrganizationDto>>(childrend);
            return entityReturn;
        }

        // 
        // Hàm lấy tất cả các tổ chức con
        //private async Task<List<TDto>> GetAllChildrenAsync<TDto>(int organizationId) where TDto : class
        //{

        //    var organization = await _dbContext.Organizations.AsNoTracking()
        //        .FirstOrDefaultAsync(o => o.Id == organizationId);
        //    var organizationDto = _mapper.Map<Getgo>
        //    if (organization != null)
        //    {
        //        // Gọi đệ quy để lấy các tổ chức con, bắt đầu từ tổ chức gốc
        //        await GetAllChildrenRecursiveAsync(organization.Id, organization);
        //    }
        //    var result = _mapper.Map<List<TDto>>(organization.OrganizationChildren);
        //    //// Cập nhật số lượng nhân viên cho từng tổ chức con và tất cả các cấp độ
        //    //foreach (var orgDto in result)
        //    //{
        //    //    // Dùng reflection để kiểm tra và gán giá trị cho thuộc tính TotalEmployees
        //    //    var totalEmployeesProperty = typeof(TDto).GetProperty("TotalEmployees");

        //    //    if (totalEmployeesProperty != null && totalEmployeesProperty.CanWrite)
        //    //    {
        //    //        // Đếm số lượng nhân viên trong tổ chức và tất cả các tổ chức con
        //    //        int totalEmployees = await CountEmployeesIncludingChildren(organizationId);

        //    //        // Gán số lượng nhân viên vào thuộc tính TotalEmployees của DTO
        //    //        totalEmployeesProperty.SetValue(orgDto, totalEmployees);
        //    //    }
        //    //}
        //    return result;
        //} 
        private async Task<List<GetOrganizationDto>> GetAllChildrenAsync(int organizationId) 
        {

            var organization = await _dbContext.Organizations.AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == organizationId);
            var organizationDto = _mapper.Map<GetOrganizationDto>(organization);

            if (organization != null)
            {
                // Gọi đệ quy để lấy các tổ chức con, bắt đầu từ tổ chức gốc
                await GetAllChildrenRecursiveAsync(organization.Id, organizationDto);
            }
            var result = _mapper.Map<List<GetOrganizationDto>>(organizationDto.OrganizationChildren);



            return result;
        }


        // Hàm đệ quy để đếm số lượng nhân viên cho tổ chức và tất cả các tổ chức con
        private async Task<int> CountEmployeesIncludingChildren(List<int> allMeAndDescendantIds)
        {
            // Đếm số lượng nhân viên trong tổ chức hiện tại và tất cả các tổ chức con
            var totalEmployees = await _dbContext.OrganizationLeaders
                .Where(ol => allMeAndDescendantIds.Contains(ol.OrganizationId))
                .CountAsync();

            return totalEmployees;
        }

        private async Task<List<int>> GetAllOrganizationIds(int organizationId)
        {
            var allIds = new List<int> { organizationId };

            // Lấy tất cả các tổ chức con trực tiếp của tổ chức hiện tại
            var children = await _dbContext.Organizations
                .Where(o => o.OrganizatioParentId == organizationId)
                .Select(o => o.Id)
                .ToListAsync();

            foreach (var childId in children)
            {
                // Đệ quy lấy tất cả ID của tổ chức con
                var childIds = await GetAllOrganizationIds(childId);
                allIds.AddRange(childIds);
            }

            return allIds;
        }

        private async Task AssignTotalEmployeesToOrganizationDtos(List<GetOrganizationDto> organizationDtos)
        {
            foreach (var orgDto in organizationDtos)
            {
                // Lấy tất cả các ID của tổ chức hiện tại và tổ chức con của nó
                var allOrganizationIds = await GetAllOrganizationIds(orgDto.Id);

                // Đếm số lượng nhân viên trong tổ chức và tất cả các tổ chức con
                var totalEmployees = await CountEmployeesIncludingChildren(allOrganizationIds);

                // Gán số lượng nhân viên vào thuộc tính TotalEmployees của DTO
                orgDto.TotalEmployees = totalEmployees;

                // Lặp qua các tổ chức con của tổ chức hiện tại và đệ quy gán số lượng nhân viên cho tổ chức con
                if (orgDto.OrganizationChildren != null && orgDto.OrganizationChildren.Any())
                {
                    await AssignTotalEmployeesToOrganizationDtos(orgDto.OrganizationChildren);
                }
            }
        }


        // Hàm đệ quy lấy tất cả các tổ chức con của một tổ chức cha
        private async Task GetAllChildrenRecursiveAsync(int parentId, GetOrganizationDto parentOrganization)
        {
            // Lấy các tổ chức con trực tiếp của tổ chức có parentId
            var children = await _dbContext.Organizations.AsNoTracking()
                                           .Where(o => o.OrganizatioParentId == parentId)
                                           .Include(o => o.Employees)
                                           .Include(o => o.OrganizationLeaders)
                                           .ThenInclude(ol => ol.Employee)
                                           .Include(o=> o.OrganizationType)
                                           .OrderBy(o => o.Rank)
                                           .OrderByDescending(o => o.Rank)
                                           .ToListAsync();
            var childrenDto = _mapper.Map<List<GetOrganizationDto>>(children);
            //var oranization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == parentId);
            // Nếu có tổ chức con, thêm vào danh sách OrganizationChildrens

            if (childrenDto.Any())
            {
                parentOrganization.OrganizationChildren = childrenDto;
                // Đệ quy vào từng tổ chức con để lấy các tổ chức con sâu hơn
                foreach (var child in childrenDto)
                {
                    if (child.Id == parentId) throw new BadHttpRequestException("Lỗi Id = ParentId");
                    await GetAllChildrenRecursiveAsync(child.Id, child);
                }
            }

        }

        //private Task<int> 

        public async Task Update(int id, UpdateOrganizationRequest request)
        {
            var organization = await _dbContext.Organizations
                   .Include(o => o.OrganizationLeaders)  // Bao gồm OrganizationLeaders để dễ dàng cập nhật
                   .FirstOrDefaultAsync(o => o.Id == id);

            await UpdateAsync(_mapper.Map(request, organization));
           
            if (organization == null)
            {
                throw new EntityNotFoundException(nameof(Organization), $"Id = {id}");
            }

            // Xử lý OrganizationLeaders (thêm, xóa hoặc cập nhật)
            // Xóa các OrganizationLeaders cũ nếu cần (tùy vào yêu cầu của bạn)
            var currentOrganizationLeaders = organization.OrganizationLeaders;

            // Tạo danh sách OrganizationLeaders mới từ yêu cầu
            var newOrganizationLeaders = request.OrganizationLeaders;

            // Tạo một danh sách các OrganizationLeaders cần xóa (các leaders đã bị xóa trong request)
            var leadersToRemove = currentOrganizationLeaders
                .Where(existing => !newOrganizationLeaders.Any(item => item.EmployeeId == existing.EmployeeId))
                .ToList();

            // Tạo một danh sách các OrganizationLeaders cần thêm (các leaders mới trong request)
            var leadersToAdd = newOrganizationLeaders
                .Where(newItem => !currentOrganizationLeaders.Any(existing => existing.EmployeeId == newItem.EmployeeId))
                .ToList();

            // Xóa các OrganizationLeaders không còn trong request
            _dbContext.OrganizationLeaders.RemoveRange(leadersToRemove);

            // Thêm các OrganizationLeaders mới
            foreach (var item in leadersToAdd)
            {
                var newOrganizationLeader = new OrganizationLeader
                {
                    OrganizationLeaderType = OrganizationLeaderType.Leader,
                    EmployeeId = item.EmployeeId,
                    OrganizationId = organization.Id
                };
                await _dbContext.OrganizationLeaders.AddAsync(newOrganizationLeader);
            }

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();

        }
        private async Task<Organization> GetOrganizationAndCheckExist(int organizationId)
        {
            var organization = await _dbContext.Organizations.FindAsync(organizationId);
            if (organization is null)
                throw new EntityNotFoundException(nameof(Organization), $"Id = {organizationId}");
            return organization;
        }

        public async Task<OrganizationSelectDto> GetSelect(int organizationId)
        {

            //var organizationTEst = await _dbContext.Organizations
            //.Include(o => o.OrganizationType)
            //.Include(o => o.OrganizatioParent) // Lọc trong Include
            //.Where(o => o.CompanyId == companyId)
            //.OrderByDescending(o => o.Rank)
            //.FirstOrDefaultAsync();
            var organization = await _dbContext.Organizations.AsNoTracking()
               .Include(o => o.OrganizationType)
                .Where(o => o.Id == organizationId)
                .OrderByDescending(o => o.Rank)
               .FirstOrDefaultAsync();

            if (organization is null)
                throw new EntityNotFoundException(nameof(Organization), $"organizationId = {organizationId}");

            var entityReturn = _mapper.Map<OrganizationSelectDto>(organization);

            var chilDto = await this.GetAllChildrenAsync(organization.Id);
            var organizationChildDto = _mapper.Map<List<OrganizationSelectDto>>(chilDto);


            entityReturn.OrganizationChildren = organizationChildDto;
            return entityReturn;
        }
        //public async Task<OrganizationSelectDto> GetSelectV2(int companyId)
        //{
        //    //var organization = await _dbContext.Organizations
        //    //   .Include(o => o.OrganizationType)
        //    //   .Include(o=> o.OrganizatioParent)
        //    //    .Where(o => o.CompanyId == companyId)
        //    //    .OrderByDescending(o => o.Rank)
        //    //   .FirstOrDefaultAsync();
        //    var organization = await _dbContext.Organizations
        //    .Include(o => o.OrganizationType)
        //    .Include(o => o.OrganizatioParent) // Lọc trong Include
        //    .Where(o => o.CompanyId == companyId)
        //    .OrderByDescending(o => o.Rank)
        //    .FirstOrDefaultAsync();


        //    if (organization is null)
        //        throw new EntityNotFoundException(nameof(Organization), $"CompanyId = {companyId}");

        //    var entityReturn = _mapper.Map<OrganizationSelectDto>(organization);
        //    var organizationChildDto = await this.GetAllChildrenAsync<OrganizationSelectDto>(organization.Id);

        //    entityReturn.OrganizationChildren = organizationChildDto;
        //    return entityReturn;
        //}

        public async Task<PagingResult<GetOrganizationDto>> GetAll(string? keyWord, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {

            var query  =  _dbContext.Organizations
                .Where(o=> o.OrganizatioParentId == null)
                .Include( o => o.Employees)
               .Include(o => o.OrganizationType)
               .Include(o => o.OrganizationLeaders.Where(ol=> ol.OrganizationId == o.Id))
               .ThenInclude(ol => ol.Employee).AsQueryable();
            if( !string.IsNullOrWhiteSpace(keyWord) )
            {
                query = query.Where( o=> o.OrganizationName.Contains(keyWord) || o.OrganizationCode.Contains(keyWord) || o.Abbreviation.Contains(keyWord));
            }
            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var organizationDtos = await _mapper.ProjectTo<GetOrganizationDto>(query).ToListAsync();

            // Lấy tất cả các tổ chức con cho mỗi tổ chức và số lượng con :))))))))))))

            foreach (var orgDto in organizationDtos) 
            {
                orgDto.OrganizationChildren = await GetAllChildrenAsync(orgDto.Id);
                //orgDto.TotalEmployees = await CountEmployeesIncludingChildren();
            }
            //xử lý đếm số nhân viên từng tổ chức 

            await AssignTotalEmployeesToOrganizationDtos(organizationDtos);

            var result = new PagingResult<GetOrganizationDto>(organizationDtos, pageIndex, pageSize, sortBy, orderBy, total);
            return result;
        }

        public async Task<PagingResult<GetOrganizationDto>> Paging(
            string? keyWord,
            string? sortBy,
            string? orderBy,
            int pageIndex = 1,
            int pageSize = 10,
            int? organizationId = null)
        {
            // =====================================================
            // 1. LOAD FULL DATA (KHÔNG SEARCH – KHÔNG PAGING)
            // =====================================================
            var organizations = await _dbContext.Organizations
                .AsNoTracking()
                .Include(o => o.Employees)
                .Include(o => o.OrganizationType)
                .Include(o => o.OrganizationLeaders)
                    .ThenInclude(ol => ol.Employee)
                .ToListAsync();

            // =====================================================
            // 2. MAP → NODE PHẲNG
            // =====================================================
            var nodes = organizations.Select(o => new OrgNode
            {
                Id = o.Id,
                ParentId = o.OrganizatioParentId,
                OrganizationCode = o.OrganizationCode,
                OrganizationName = o.OrganizationName,
                Abbreviation = o.Abbreviation,
                DirectEmployees = o.Employees.Count,
                OrganizationType = o.OrganizationType,
                Leaders = o.OrganizationLeaders
                    .Where(x => x.OrganizationLeaderType == OrganizationLeaderType.Leader)
                    .Select(x => new OrganizationLeaderDto
                    {
                        OrganizationLeaderType = x.OrganizationLeaderType,
                        Employee = new GetOrganizationLeaderDto
                        {
                            Id = x.Employee.Id,
                            FirstName = x.Employee.FirstName,
                            LastName = x.Employee.LastName
                        }
                    })
                    .ToList()
            }).ToList();

            // =====================================================
            // 3. BUILD TREE
            // =====================================================
            var nodeMap = nodes.ToDictionary(x => x.Id);

            foreach (var node in nodes)
            {
                if (node.ParentId.HasValue && nodeMap.ContainsKey(node.ParentId.Value))
                {
                    nodeMap[node.ParentId.Value].Children.Add(node);
                }
            }

            var roots = nodes.Where(x => x.ParentId == null).ToList();

            // Nếu truyền OrganizationId thì chỉ lấy cây con bắt đầu từ node đó
            if (organizationId.HasValue && nodeMap.TryGetValue(organizationId.Value, out var orgRoot))
            {
                roots = new List<OrgNode> { orgRoot };
            }

            // =====================================================
            // 4. SEARCH LOGIC (THEO YÊU CẦU MỚI)
            // =====================================================
            HashSet<int> matchedIds = new();
            Dictionary<int, string> nodeTypes = new(); // "Root", "Unit", "Department"

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                var matched = nodes
                    .Where(x =>
                        x.OrganizationName.Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                        || x.OrganizationCode.Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                        || (x.Abbreviation != null &&
                            x.Abbreviation.Contains(keyWord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                foreach (var m in matched)
                {
                    matchedIds.Add(m.Id);

                    // Xác định loại node
                    if (m.ParentId == null)
                    {
                        nodeTypes[m.Id] = "Root";
                    }
                    else if (m.OrganizationType?.OrganizationTypeName == "Unit")
                    {
                        nodeTypes[m.Id] = "Unit";
                    }
                    else
                    {
                        nodeTypes[m.Id] = "Department";
                    }
                }
            }

            List<OrgNode> filteredRoots;

            if (matchedIds.Any())
            {
                filteredRoots = new List<OrgNode>();

                foreach (var root in roots)
                {
                    var clonedRoot = CloneNode(root);
                    if (FilterTree(clonedRoot, matchedIds, nodeTypes))
                    {
                        filteredRoots.Add(clonedRoot);
                    }
                }
            }
            else
            {
                filteredRoots = roots;
            }

            // =====================================================
            // 5. SORT + PAGING ROOT
            // =====================================================
            int totalRecords = filteredRoots.Count;

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                filteredRoots = orderBy?.ToLower() == "desc"
                    ? filteredRoots.OrderByDescending(x => GetPropertyValue(x, sortBy)).ToList()
                    : filteredRoots.OrderBy(x => GetPropertyValue(x, sortBy)).ToList();
            }

            var pagedRoots = filteredRoots
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // =====================================================
            // 6. TOTAL EMPLOYEES (ĐỆ QUY ĐÚNG)
            // =====================================================
            foreach (var root in pagedRoots)
            {
                CalculateTotalEmployees(root);
            }

            // =====================================================
            // 7. MAP → DTO
            // =====================================================
            var items = pagedRoots.Select(MapToDto).ToList();

            return new PagingResult<GetOrganizationDto>(
                items,
                pageIndex,
                pageSize,
                sortBy,
                orderBy,
                totalRecords
            );
        }

        // =====================================================
        // HELPER METHODS
        // =====================================================

        private OrgNode CloneNode(OrgNode source)
        {
            return new OrgNode
            {
                Id = source.Id,
                ParentId = source.ParentId,
                OrganizationCode = source.OrganizationCode,
                OrganizationName = source.OrganizationName,
                Abbreviation = source.Abbreviation,
                DirectEmployees = source.DirectEmployees,
                TotalEmployees = source.TotalEmployees,
                OrganizationType = source.OrganizationType,
                Leaders = source.Leaders,
                Children = source.Children.Select(CloneNode).ToList()
            };
        }

        private bool FilterTree(
            OrgNode node,
            HashSet<int> matchedIds,
            Dictionary<int, string> nodeTypes)
        {
            bool currentNodeMatches = matchedIds.Contains(node.Id);

            // ===============================
            // CASE 1: NODE MATCH
            // ===============================
            if (currentNodeMatches)
            {
                string matchType = nodeTypes[node.Id];

                // ROOT hoặc UNIT match → GIỮ NGUYÊN TOÀN BỘ SUBTREE
                if (matchType == "Root" || matchType == "Unit")
                {
                    return true; // KHÔNG filter children nữa
                }

                // DEPARTMENT match → chỉ giữ node này
                if (matchType == "Department")
                {
                    node.Children.Clear();
                    return true;
                }
            }

            // ===============================
            // CASE 2: NODE KHÔNG MATCH
            // ===============================
            var keptChildren = new List<OrgNode>();

            foreach (var child in node.Children)
            {
                if (FilterTree(child, matchedIds, nodeTypes))
                {
                    keptChildren.Add(child);
                }
            }

            if (keptChildren.Any())
            {
                node.Children = keptChildren; // giữ path
                return true;
            }

            return false;
        }


        private int CalculateTotalEmployees(OrgNode node)
        {
            int total = node.DirectEmployees;
            foreach (var child in node.Children)
            {
                total += CalculateTotalEmployees(child);
            }
            node.TotalEmployees = total;
            return total;
        }

        private GetOrganizationDto MapToDto(OrgNode node)
        {
            return new GetOrganizationDto
            {
                Id = node.Id,
                OrganizationCode = node.OrganizationCode,
                OrganizationName = node.OrganizationName,
                Abbreviation = node.Abbreviation,
                TotalEmployees = node.TotalEmployees,
                OrganizationType = node.OrganizationType == null
                    ? null
                    : new OrganizationTypeDto
                    {
                        Id = node.OrganizationType.Id,
                        OrganizationTypeName = node.OrganizationType.OrganizationTypeName
                    },
                OrganizationLeaders = node.Leaders,
                OrganizationChildren = node.Children.Select(MapToDto).ToList()
            };
        }

        private object GetPropertyValue(OrgNode node, string propertyName)
        {
            var property = typeof(OrgNode).GetProperty(propertyName);
            return property?.GetValue(node) ?? string.Empty;
        }





        // Helper method để map OrgHierarchyResult sang GetOrganizationDto
        private GetOrganizationDto MapToGetOrganizationDto(
            OrgHierarchyResult data, 
            Dictionary<int, List<OrganizationLeaderDto>> leaderMap)
        {
            return new GetOrganizationDto
            {
                Id = data.Id,
                OrganizationCode = data.OrganizationCode,
                OrganizationName = data.OrganizationName,
                Abbreviation = data.Abbreviation,
                CompanyId = data.CompanyId,
                Rank = data.Rank,
                OrganizationTypeId = data.OrganizationTypeId ?? 0,
                OrganizatioParentId = data.OrganizatioParentId,
                OrganizationStatus = data.OrganizationStatus,
                TotalEmployees = data.TotalEmployees,
                OrganizationType = data.OrganizationType_Id.HasValue ? new OrganizationTypeDto
                {
                    Id = data.OrganizationType_Id.Value,
                    CompanyId = data.OrganizationType_CompanyId ?? 0,
                    OrganizationTypeName = data.OrganizationType_OrganizationTypeName ?? string.Empty
                } : null!,
                OrganizationLeaders = leaderMap.TryGetValue(data.Id, out var leaders) 
                    ? leaders 
                    : new List<OrganizationLeaderDto>(),
                Employees = new List<GetOrganizationEmployeeDto>(),
                OrganizationChildren = new List<GetOrganizationDto>()
            };
        }

        // Helper method để xây dựng cây OrganizationChildren đệ quy
        private List<GetOrganizationDto> BuildOrganizationChildren(
            int parentId,
            List<OrgHierarchyResult> allOrgs,
            Dictionary<int, List<OrganizationLeaderDto>> leaderMap,
            int parentLevel)
        {
            var children = allOrgs
                .Where(x => x.OrganizatioParentId == parentId && x.Level == parentLevel + 1)
                .OrderBy(x => x.Rank)
                .ToList();

            if (!children.Any()) return new List<GetOrganizationDto>();

            var result = new List<GetOrganizationDto>();
            foreach (var child in children)
            {
                var childDto = MapToGetOrganizationDto(child, leaderMap);
                childDto.OrganizationChildren = BuildOrganizationChildren(
                    child.Id, 
                    allOrgs, 
                    leaderMap, 
                    child.Level);
                result.Add(childDto);
            }

            return result;
        }

        //public async Task<OrganizationDto> CreateV2(CreateOrganizationRequest request)
        //{
        //    var parentOrganization = await _dbContext.OrganizationV2s.FirstOrDefaultAsync(o => o.OrganizationName == "Công ty SMO");

        //    //var newOrganization = new OrganizationV2
        //    //{
        //    //    OrganizationCode = "SMO002",
        //    //    OrganizationName = "Công ty Con 1",
        //    //    Patch = parentOrganization.Path.GetDescendant(0, 0),  // Tạo con trực tiếp của tổ chức cha
        //    ////    ParentOrganizationId = parentOrganization.Id
        //    ////};

        //    //_dbContext.Organizations.Add(newOrganization);
        //    //await _dbContext.SaveChangesAsync();
        //    return _mapper.Map<OrganizationDto>(parentOrganization);
        //}
        public int GetRootOrganizationId(int childOrganizationId)
        {
            int? currentId = childOrganizationId;
            var organizations = _dbContext.Organizations.ToList();

            while (true)
            {
                // Tìm tổ chức hiện tại
                var currentOrg = organizations.FirstOrDefault(o => o.Id == currentId);

                if (currentOrg == null)
                    throw new EntityNotFoundException("Invalid organization ID");

                // Nếu không có OrganizatioParentId, đây là tổ chức gốc
                if (currentOrg.OrganizatioParentId == null)
                    return currentOrg.Id;

                // Chuyển sang ParentOrganizationId
                currentId = currentOrg.OrganizatioParentId;
            }
        }

        public async Task<List<GetOrganizationDto>> Export(
            string? keyWord,
            int? organizationId,
            string? sortBy,
            string? orderBy)
        {
            // =====================================================
            // 1. LOAD FULL DATA (KHÔNG SEARCH – KHÔNG PAGING)
            // =====================================================
            var organizations = await _dbContext.Organizations
                .AsNoTracking()
                .Include(o => o.Employees)
                .Include(o => o.OrganizationType)
                .Include(o => o.OrganizationLeaders)
                    .ThenInclude(ol => ol.Employee)
                .ToListAsync();

            // =====================================================
            // 2. MAP → NODE PHẲNG
            // =====================================================
            var nodes = organizations.Select(o => new OrgNode
            {
                Id = o.Id,
                ParentId = o.OrganizatioParentId,
                OrganizationCode = o.OrganizationCode,
                OrganizationName = o.OrganizationName,
                Abbreviation = o.Abbreviation,
                DirectEmployees = o.Employees.Count,
                OrganizationType = o.OrganizationType,
                Leaders = o.OrganizationLeaders
                    .Where(x => x.OrganizationLeaderType == OrganizationLeaderType.Leader)
                    .Select(x => new OrganizationLeaderDto
                    {
                        OrganizationLeaderType = x.OrganizationLeaderType,
                        Employee = new GetOrganizationLeaderDto
                        {
                            Id = x.Employee.Id,
                            FirstName = x.Employee.FirstName,
                            LastName = x.Employee.LastName
                        }
                    })
                    .ToList()
            }).ToList();

            // =====================================================
            // 3. BUILD TREE
            // =====================================================
            var nodeMap = nodes.ToDictionary(x => x.Id);

            foreach (var node in nodes)
            {
                if (node.ParentId.HasValue && nodeMap.ContainsKey(node.ParentId.Value))
                {
                    nodeMap[node.ParentId.Value].Children.Add(node);
                }
            }

            var roots = nodes.Where(x => x.ParentId == null).ToList();

            // Nếu truyền OrganizationId thì chỉ lấy cây con bắt đầu từ node đó
            if (organizationId.HasValue && nodeMap.TryGetValue(organizationId.Value, out var orgRoot))
            {
                roots = new List<OrgNode> { orgRoot };
            }

            // =====================================================
            // 4. SEARCH LOGIC (THEO YÊU CẦU MỚI)
            // =====================================================
            HashSet<int> matchedIds = new();
            Dictionary<int, string> nodeTypes = new(); // "Root", "Unit", "Department"

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                var matched = nodes
                    .Where(x =>
                        x.OrganizationName.Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                        || x.OrganizationCode.Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                        || (x.Abbreviation != null &&
                            x.Abbreviation.Contains(keyWord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                foreach (var m in matched)
                {
                    matchedIds.Add(m.Id);

                    // Xác định loại node
                    if (m.ParentId == null)
                    {
                        nodeTypes[m.Id] = "Root";
                    }
                    else if (m.OrganizationType?.OrganizationTypeName == "Unit")
                    {
                        nodeTypes[m.Id] = "Unit";
                    }
                    else
                    {
                        nodeTypes[m.Id] = "Department";
                    }
                }
            }

            List<OrgNode> filteredRoots;

            if (matchedIds.Any())
            {
                filteredRoots = new List<OrgNode>();

                foreach (var root in roots)
                {
                    var clonedRoot = CloneNode(root);
                    if (FilterTree(clonedRoot, matchedIds, nodeTypes))
                    {
                        filteredRoots.Add(clonedRoot);
                    }
                }
            }
            else
            {
                filteredRoots = roots;
            }

            // =====================================================
            // 5. SORT (KHÔNG PAGING)
            // =====================================================
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                filteredRoots = orderBy?.ToLower() == "desc"
                    ? filteredRoots.OrderByDescending(x => GetPropertyValue(x, sortBy)).ToList()
                    : filteredRoots.OrderBy(x => GetPropertyValue(x, sortBy)).ToList();
            }

            // =====================================================
            // 6. TOTAL EMPLOYEES (ĐỆ QUY ĐÚNG)
            // =====================================================
            foreach (var root in filteredRoots)
            {
                CalculateTotalEmployees(root);
            }

            // =====================================================
            // 7. MAP → DTO (KHÔNG BỌC PAGING)
            // =====================================================
            var items = filteredRoots.Select(MapToDto).ToList();
            return items;
        }

    }
}
