using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRM_BE.Core.Constants.System;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.CheckInCheckOut;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRM_BE.Data.Repositories
{
    public class CheckInCheckOutApplicationRepository
        : RepositoryBase<CheckInCheckOutApplication, int>, ICheckInCheckOutApplicationRepository
    {
        private readonly IMapper _mapper;

        public CheckInCheckOutApplicationRepository(
            HrmContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        ) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagingResult<CheckInCheckOutApplicationDto>> GetPaging(
            int? organizationId,
            int? employeeId,
            int currentEmployeeId,
            bool isAdmin,
            bool forApproval,
            string? keyWord,
            DateTime? startDate,
            DateTime? endDate,
            int? status,
            string? sortBy,
            string? orderBy,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _dbContext.CheckInCheckOutApplications
                .Where(x => x.IsDeleted != true)
                .Include(x => x.Employee)
                .Include(x => x.Approver)
                .Include(x => x.ShiftCatalog)
                .AsQueryable();

            if (forApproval)
            {
                query = query.Where(x => x.ApproverId == currentEmployeeId);
            }
            else if (!isAdmin)
            {
                query = query.Where(x => x.EmployeeId == currentEmployeeId);
            }

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId);

            if (organizationId.HasValue)
                query = query.Where(x => x.Employee.OrganizationId == organizationId);

            if (startDate.HasValue)
                query = query.Where(x => x.Date >= startDate);

            if (endDate.HasValue)
                query = query.Where(x => x.Date <= endDate);

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                var lower = keyWord.ToLower();
                query = query.Where(x =>
                    x.Employee.EmployeeCode.ToLower().Contains(lower) ||
                    (x.Employee.LastName + " " + x.Employee.FirstName).ToLower().Contains(lower));
            }

            if (status.HasValue)
                query = query.Where(x => x.CheckInCheckOutStatus == status);

            if (string.IsNullOrEmpty(orderBy) && string.IsNullOrEmpty(sortBy))
            {
                query = query.OrderByDescending(x => x.Id);
            }
            else if (string.IsNullOrEmpty(orderBy))
            {
                query = sortBy == SortByConstant.Asc
                    ? query.OrderBy(x => x.Id)
                    : query.OrderByDescending(x => x.Id);
            }
            else
            {
                query = orderBy switch
                {
                    OrderByConstant.Id when sortBy == SortByConstant.Asc => query.OrderBy(x => x.Id),
                    OrderByConstant.Id when sortBy == SortByConstant.Desc => query.OrderByDescending(x => x.Id),
                    _ => query.OrderByDescending(x => x.Id)
                };
            }

            var total = await query.CountAsync();
            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CheckInCheckOutApplicationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagingResult<CheckInCheckOutApplicationDto>(data, pageIndex, pageSize, sortBy, orderBy, total);
        }

        public async Task<CheckInCheckOutApplicationDto> GetById(int id)
        {
            var query = _dbContext.CheckInCheckOutApplications
                .Where(x => x.Id == id)
                .Include(x => x.Employee)
                .Include(x => x.Approver)
                .Include(x => x.ShiftCatalog)
                .AsQueryable();

            var data = await _mapper.ProjectTo<CheckInCheckOutApplicationDto>(query).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new EntityNotFoundException(nameof(CheckInCheckOutApplication), $"Id = {id}");
            }

            return data;
        }

        public async Task<int> CreateAsync(CreateCheckInCheckOutApplicationRequest request)
        {
            var entity = new CheckInCheckOutApplication
            {
                EmployeeId = request.EmployeeId,
                ApproverId = request.ApproverId,
                Date = request.Date,
                CheckType = request.CheckType,
                ShiftCatalogId = request.ShiftCatalogId,
                Reason = request.Reason,
                Description = request.Description,
                CheckInCheckOutStatus = 0,
                TimeCheckIn = string.IsNullOrEmpty(request.TimeCheckIn) ? null : TimeSpan.Parse(request.TimeCheckIn),
                TimeCheckOut = string.IsNullOrEmpty(request.TimeCheckOut) ? null : TimeSpan.Parse(request.TimeCheckOut)
            };

            var created = await CreateAsync(entity);
            return created.Id;
        }

        public async Task UpdateAsync(int id, UpdateCheckInCheckOutApplicationRequest request)
        {
            var entity = await _dbContext.CheckInCheckOutApplications.FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(nameof(CheckInCheckOutApplication), $"Id = {id}");

            entity.ApproverId = request.ApproverId;
            entity.Date = request.Date;
            entity.CheckType = request.CheckType;
            entity.ShiftCatalogId = request.ShiftCatalogId;
            entity.Reason = request.Reason;
            entity.Description = request.Description;
            entity.TimeCheckIn = string.IsNullOrEmpty(request.TimeCheckIn) ? null : TimeSpan.Parse(request.TimeCheckIn);
            entity.TimeCheckOut = string.IsNullOrEmpty(request.TimeCheckOut) ? null : TimeSpan.Parse(request.TimeCheckOut);

            if (request.CheckInCheckOutStatus.HasValue)
                entity.CheckInCheckOutStatus = request.CheckInCheckOutStatus.Value;

            await UpdateAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int status)
        {
            var entity = await _dbContext.CheckInCheckOutApplications.FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(nameof(CheckInCheckOutApplication), $"Id = {id}");

            entity.CheckInCheckOutStatus = status;
            await UpdateAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CheckInCheckOutApplicationDto>> GetExportData(
            int? organizationId,
            int? employeeId,
            int currentEmployeeId,
            bool isAdmin,
            bool forApproval,
            string? keyWord,
            DateTime? startDate,
            DateTime? endDate,
            int? status)
        {
            var query = _dbContext.CheckInCheckOutApplications
                .Where(x => x.IsDeleted != true)
                .Include(x => x.Employee)
                .Include(x => x.Approver)
                .Include(x => x.ShiftCatalog)
                .AsQueryable();

            if (forApproval)
            {
                query = query.Where(x => x.ApproverId == currentEmployeeId);
            }
            else if (!isAdmin)
            {
                query = query.Where(x => x.EmployeeId == currentEmployeeId);
            }

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId);

            if (organizationId.HasValue)
                query = query.Where(x => x.Employee.OrganizationId == organizationId);

            if (startDate.HasValue)
                query = query.Where(x => x.Date >= startDate);

            if (endDate.HasValue)
                query = query.Where(x => x.Date <= endDate);

            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                var lower = keyWord.ToLower();
                query = query.Where(x =>
                    x.Employee.EmployeeCode.ToLower().Contains(lower) ||
                    (x.Employee.LastName + " " + x.Employee.FirstName).ToLower().Contains(lower));
            }

            if (status.HasValue)
                query = query.Where(x => x.CheckInCheckOutStatus == status);

            return await query
                .OrderByDescending(x => x.Id)
                .ProjectTo<CheckInCheckOutApplicationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}

