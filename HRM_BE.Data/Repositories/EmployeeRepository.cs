using AutoMapper;
using AutoMapper.Configuration.Annotations;
using AutoMapper.QueryableExtensions;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.Helpers;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Company;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee, int>, IEmployeeRepository
    {
        public IMapper _mapper;
        public EmployeeRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Create(CreateEmployeeRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            await CheckEmailExist(request.AccountEmail);
            await CheckEmailPersonExist(request.PersonalEmail);
            await CheckPhoneNumberlExist(request.PhoneNumber);

            var employee = _mapper.Map<Employee>(request);

            var employeeCode = request.EmployeeCode;
            employee.EmployeeCode = employeeCode;
            var entityReturn = await CreateAsync(employee);

            await _dbContext.ProfileInfos.AddAsync(new Core.Data.ProfileEntity.ProfileInfo
            {
                EmployeeId = employee.Id,
            });
            await _dbContext.ContactInfos.AddAsync(new Core.Data.ProfileEntity.ContactInfo
            {
                EmployeeId = employee.Id,
            });
            await _dbContext.InfoJobs.AddAsync(new Core.Data.ProfileEntity.JobInfo
            {
                EmployeeId = employee.Id,
            });
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<EmployeeDto>(entityReturn);
        }

        public async Task Delete(int id)
        {
            // Không cho phép xóa nhân viên nếu đang có hợp đồng còn hiệu lực
            var currentTime = DateTime.Now;
            var hasValidContract = await _dbContext.Contracts
                .AsNoTracking()
                .AnyAsync(c =>
                    c.EmployeeId == id &&
                    c.EffectiveDate <= currentTime &&
                    (c.ExpiryDate == null || c.ExpiryDate >= currentTime));

            if (hasValidContract)
            {
                throw new EntityAlreadyExistsException("Nhân viên đang có hợp đồng còn hiệu lực, không thể xóa.");
            }

            var entity = await GetEmployeeAndCheckExsit(id);
            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }

        public async Task DeleteRange(ListEntityIdentityRequest<int> request)
        {
            var entities = await _dbContext.Employees.Where(x => request.Ids.Contains(x.Id)).ToListAsync();
            entities.ForEach(x => x.IsDeleted = true);
            await SaveChangesAsync();
        }

        public async Task<EmployeeDto> GetById(int id)
        {
            var checkExsit = await GetEmployeeAndCheckExsit(id);
            var employee = await _dbContext.Employees.
                Where( e => e.Id == id)
                .Include(e => e.StaffPosition)
                .Include(e => e.StaffTitle)
                .Include(e => e.ManagerIndirect)
                .Include(e => e.OrganizationLeaders).ThenInclude(ol => ol.Organization)
                .Include(e => e.Contracts)
                .ThenInclude(c => c.ContractType)
                .SingleOrDefaultAsync();
            //var employee2 = await _mapper.ProjectTo<EmployeeDto>(_dbContext.Employees).SingleOrDefaultAsync();
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<PagingResult<EmployeeDto>> Paging(string? keyWord, int? organizationId,int? leaderOrganizationId, int? positionId, WorkingStatus? workingStatus,AccountStatus? accountStatus,int? cityId,int? districtId,int? wardId,int? streetId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Employees.Where(e => e.IsDeleted == false)
                //.Include(e => e.ManagerDirect)
                //.Include(e => e.StaffPosition)
                //.Include(e => e.StaffTitle)
                //.Include(e => e.OrganizationLeaders
                //    .Where(o => o.OrganizationLeaderType == OrganizationLeaderType.Member))
                //.Include(e => e.Contracts.Where( c => c.ExpiredStatus == false))
                .AsNoTracking();
            
            if (!string.IsNullOrEmpty(keyWord))
            {
                keyWord = keyWord.Trim(); // Loại bỏ khoảng trắng thừa đầu và cuối từ khóa
                
                // Tách từ khóa thành các từ riêng biệt và loại bỏ khoảng trắng thừa
                var searchTerms = keyWord.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(term => term.Trim())
                    .Where(term => !string.IsNullOrEmpty(term))
                    .ToList();

                if (searchTerms.Any())
                {
                    // Tạo điều kiện tìm kiếm: (tên chứa tất cả các từ) OR (số điện thoại chứa toàn bộ từ khóa)
                    // Áp dụng từng điều kiện cho tên để tạo AND logic (tất cả các từ phải xuất hiện)
                    // Sau đó thêm OR với điều kiện số điện thoại
                    
                    // Bước 1: Áp dụng điều kiện cho từ đầu tiên kèm theo điều kiện số điện thoại
                    var firstTerm = searchTerms[0];
                    query = query.Where(c =>
                        ((c.LastName ?? "").Trim() + " " + (c.FirstName ?? "").Trim()).Contains(firstTerm) ||
                        ((c.FirstName ?? "").Trim() + " " + (c.LastName ?? "").Trim()).Contains(firstTerm) ||
                        (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(keyWord)) ||
                        (!string.IsNullOrEmpty(c.WorkPhoneNumber) && c.WorkPhoneNumber.Contains(keyWord)) ||
                        (c.StaffPosition != null && !string.IsNullOrEmpty(c.StaffPosition.PositionName) && c.StaffPosition.PositionName.Contains(firstTerm)) ||
                        (c.StaffTitle != null && !string.IsNullOrEmpty(c.StaffTitle.StaffTitleName) && c.StaffTitle.StaffTitleName.Contains(firstTerm)));
                    
                    // Bước 2: Áp dụng điều kiện cho các từ còn lại (chỉ kiểm tra tên, không kiểm tra số điện thoại nữa)
                    for (int i = 1; i < searchTerms.Count; i++)
                    {
                        var term = searchTerms[i];
                        query = query.Where(c =>
                            ((c.LastName ?? "").Trim() + " " + (c.FirstName ?? "").Trim()).Contains(term) ||
                            ((c.FirstName ?? "").Trim() + " " + (c.LastName ?? "").Trim()).Contains(term) ||
                            (c.StaffPosition != null && !string.IsNullOrEmpty(c.StaffPosition.PositionName) && c.StaffPosition.PositionName.Contains(term)) ||
                            (c.StaffTitle != null && !string.IsNullOrEmpty(c.StaffTitle.StaffTitleName) && c.StaffTitle.StaffTitleName.Contains(term)));
                    }
                }
            }

            if (organizationId.HasValue)
            {
                var organizationDescendantIds = await GetAllChildOrganizationIds(organizationId.Value);
                organizationDescendantIds.Add(organizationId.Value);
                query = query.Where(e => organizationDescendantIds.Contains(e.OrganizationId.Value));
            }
            if (accountStatus.HasValue)
            {
                query = query.Where(e => e.AccountStatus == accountStatus.Value);
            }
            if (leaderOrganizationId.HasValue)
            {
                query = query.Where(e => e.OrganizationLeaders.Where(or => or.OrganizationId == leaderOrganizationId).Any());
            }
            if (positionId.HasValue)
            {
                query = query.Where(e => e.StaffPositionId == positionId);
            }
            if (workingStatus.HasValue)
            {
                query = query.Where(e => e.WorkingStatus == workingStatus);
            }
            if (cityId.HasValue)
            {
                query = query.Where(e => e.CityId == cityId);
            }
            if (districtId.HasValue)
            {
                query = query.Where(e => e.DistrictId == districtId);
            }
            if (wardId.HasValue)
            {
                query = query.Where(e => e.WardId == wardId);
            }
            if (streetId.HasValue)
            {
                query = query.Where(e => e.StreetId == streetId);
            }
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<EmployeeDto>(query).ToListAsync();

            var result = new PagingResult<EmployeeDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task Update(int id, UpdateEmployeeRequest request)
        {
            var entity = await GetEmployeeAndCheckExsit(id);
            var employeeUpdate = _mapper.Map(request, entity);
            await UpdateAsync(employeeUpdate);
        }

        private async Task<Employee> GetEmployeeAndCheckExsit(int employeeId)
        {
            var employee = await _dbContext.Employees.FindAsync(employeeId);
            if (employee is null)
                throw new EntityNotFoundException(nameof(Employee), $"Id = {employeeId}");
            return employee;
        }
        public async Task CheckEmailExist(string email)
        {
            bool emailExists = await _dbContext.Employees.AnyAsync(e => e.AccountEmail == email);
            if (emailExists) throw new EntityAlreadyExistsException($"Email {email} đã tồn tại");
        }    
        public async Task CheckEmailPersonExist(string email)
        {
            bool emailPersonExists = await _dbContext.Employees.AnyAsync(e => e.PersonalEmail == email);
            if (emailPersonExists) throw new EntityAlreadyExistsException($"Email cá nhân {email} đã tồn tại");
        }
        public async Task CheckPhoneNumberlExist(string email)
        {
            bool phoneNumberExists = await _dbContext.Employees.AnyAsync(e => e.PhoneNumber == email);
            if (phoneNumberExists) throw new EntityAlreadyExistsException($"Số điện thoại {email} đã tồn tại");
        }
        public async Task UpdateAccountStatus(int employeeId, UpdateAccountStatusRequest request)
        {
            var employee = await GetEmployeeAndCheckExsit(employeeId);
            employee.AccountStatus = request.AccountStatus;
            await UpdateAsync(employee);
        }

        public async Task<(UpdateEmployeeRequest employeePath, Employee employeeEntity)> GetEmployeeForPatch(int id)
        {
            var employeeEntity = await _dbContext.Employees.FindAsync(id);

            if (employeeEntity is null)
                throw new EntityNotFoundException(nameof(Employee), $"Id = {id}");

            var employeeToPatch = _mapper.Map<UpdateEmployeeRequest>(employeeEntity);

            return (employeeToPatch,employeeEntity);
            
        }
        public async Task SaveChangesForPatch(UpdateEmployeeRequest emlpoyeePatch, Employee employeeEntity)
        {
            _mapper.Map(emlpoyeePatch,employeeEntity);
            await SaveChangesAsync();
        }

        public async Task UpdateRangeAccountStatus(List<int> ids, AccountStatus accountStatus)
        {
            var entity = await _dbContext.Employees.Where( e => ids.Contains(e.Id)).ToListAsync();
            entity.ForEach( entity => entity.AccountStatus = accountStatus);
            await UpdateRangeAsync(entity);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<GetEmployeeProfileDto> GetProfileInfoByEmployeeId(int employeeId)
        {

            var checkExsit = await GetEmployeeAndCheckExsit(employeeId);

            var employee = await _dbContext.Employees.Where(e => e.Id == employeeId)
               .Include(e => e.ContactInfo)
               .Include(e => e.ProfileInfo)
               .Include(e => e.JobInfo)
               .Include(e => e.StaffPosition)
               .Include(e => e.StaffTitle)
               .Include(e => e.OrganizationLeaders).ThenInclude(e => e.Organization)
               .Include(e => e.Contracts.Where(c => c.ExpiredStatus == false)).ThenInclude(c => c.Allowances)
               .Include(e => e.Deductions)
               .SingleOrDefaultAsync();
            var query = _dbContext.Employees.Where(e => e.Id == employeeId && e.Contracts.Any(c => !c.ExpiredStatus.HasValue)).AsQueryable();
            
            var employee2 = await _mapper.ProjectTo<GetEmployeeProfileDto>(query).SingleOrDefaultAsync();
            return _mapper.Map<GetEmployeeProfileDto>(employee);
        }
        //public async Task<GetEmployeeProfileDto> GetProfileInfoByEmployeeId(int employeeId)
        //{
        //    // Kiểm tra sự tồn tại của nhân viên trước khi truy vấn
        //    var checkExsit = await GetEmployeeAndCheckExsit(employeeId);

        //    // Sử dụng ProjectTo để trực tiếp ánh xạ dữ liệu từ database vào DTO
        //    var employeeProfileDto = await _dbContext.Employees
        //        .Where(e => e.Id == employeeId)
        //        .ProjectTo<GetEmployeeProfileDto>(_mapper.ConfigurationProvider) // ProjectTo ánh xạ trực tiếp vào DTO
        //        .SingleOrDefaultAsync();  // Lấy dữ liệu của nhân viên

        //    return employeeProfileDto;
        //}
        public async Task<List<int>> GetAllChildOrganizationIds(int parentId)
        {
            // Chỉ tải trường cần thiết để giảm I/O khi lọc theo cây tổ chức
            var allOrganizations = await _dbContext.Organizations
                .AsNoTracking()
                .Where(o => o.IsDeleted != true)
                .Select(o => new Organization
                {
                    Id = o.Id,
                    OrganizatioParentId = o.OrganizatioParentId
                })
                .ToListAsync();

            // Gọi hàm đệ quy để tìm tất cả các Id con
            var result = new List<int>();
            GetChildIdsRecursive(parentId, allOrganizations, result);
            return result;
        }

        private void GetChildIdsRecursive(int parentId, List<Organization> allOrganizations, List<int> result)
        {
            // Lấy tất cả các con trực tiếp của parentId
            var children = allOrganizations.Where(o => o.OrganizatioParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child.Id); // Thêm Id của con vào danh sách kết quả
                GetChildIdsRecursive(child.Id, allOrganizations, result); // Gọi đệ quy cho các con
            }
        }
    }
}
