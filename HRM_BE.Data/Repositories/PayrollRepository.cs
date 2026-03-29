using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class PayrollRepository : RepositoryBase<Payroll, int>, IPayrollRepository
    {
        private readonly IMapper _mapper;

        public PayrollRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PayrollDto> Create(CreatePayrollRequest request)
        {
            if (request.SummaryTimesheetNameIds != null && request.SummaryTimesheetNameIds.Any())
            {
                var summaryTimesheetNameIds = request.SummaryTimesheetNameIds
                    .Distinct()
                    .ToList();

                var alreadyLinkedIds = await _dbContext.Set<PayrollSummaryTimesheet>()
                    .Where(pst => pst.SummaryTimesheetNameId != null
                                  && summaryTimesheetNameIds.Contains(pst.SummaryTimesheetNameId.Value)
                                  && pst.Payroll.IsDeleted != true)
                    .Select(pst => pst.SummaryTimesheetNameId!.Value)
                    .Distinct()
                    .ToListAsync();

                if (alreadyLinkedIds.Any())
                {
                    throw new Exception("Bảng chấm công tổng hợp này đã được chuyển sang tính lương trước đó.");
                }

                var summaryTimesheets = await _dbContext.SummaryTimesheetNames
                    .Where(stn => summaryTimesheetNameIds.Contains(stn.Id) && stn.IsDeleted != true)
                    .Select(stn => new
                    {
                        stn.Id,
                        stn.OrganizationId,
                        StaffPositionIds = stn.SummaryTimesheetNameStaffPositions
                            .Where(sp => sp.StaffPositionId.HasValue)
                            .Select(sp => sp.StaffPositionId!.Value)
                            .Distinct()
                            .ToList()
                    })
                    .ToListAsync();

                var missingSummaryIds = summaryTimesheetNameIds
                    .Except(summaryTimesheets.Select(x => x.Id))
                    .ToList();

                if (missingSummaryIds.Any())
                {
                    throw new ApiException("Không tìm thấy bảng chấm công tổng hợp để chuyển tính lương.");
                }

                foreach (var summary in summaryTimesheets)
                {
                    if (!summary.OrganizationId.HasValue)
                    {
                        throw new ApiException("Bảng chấm công tổng hợp chưa có đơn vị áp dụng.");
                    }

                    var organizationId = summary.OrganizationId.Value;
                    var organizationDescendantIds = await GetAllChildOrganizationIds(organizationId);
                    organizationDescendantIds.Add(organizationId);

                    var employeeQuery = _dbContext.Employees
                        .Where(e => e.AccountStatus == AccountStatus.Active
                                    && e.OrganizationId.HasValue
                                    && organizationDescendantIds.Contains(e.OrganizationId.Value)
                                    && e.Contracts.Count(c => c.ExpiredStatus == false) == 1);

                    if (summary.StaffPositionIds.Any())
                    {
                        employeeQuery = employeeQuery.Where(e => e.StaffPositionId.HasValue && summary.StaffPositionIds.Contains(e.StaffPositionId.Value));
                    }

                    var employeeIds = await employeeQuery
                        .Select(e => e.Id)
                        .ToListAsync();

                    if (!employeeIds.Any())
                    {
                        throw new ApiException("Bảng chấm công tổng hợp không có dữ liệu nhân sự để chuyển tính lương.");
                    }

                    var latestConfirmStatuses = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                        .Where(c => c.SummaryTimesheetNameId == summary.Id
                                    && c.EmployeeId.HasValue
                                    && employeeIds.Contains(c.EmployeeId.Value)
                                    && c.IsDeleted != true)
                        .GroupBy(c => c.EmployeeId!.Value)
                        .Select(g => new
                        {
                            EmployeeId = g.Key,
                            Status = g.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                                      .Select(x => x.Status)
                                      .FirstOrDefault()
                        })
                        .ToListAsync();

                    var hasMissingConfirmation = latestConfirmStatuses.Count != employeeIds.Count;
                    var hasNotConfirmedStatus = latestConfirmStatuses.Any(x => x.Status != SummaryTimesheetNameEmployeeConfirmStatus.Confirm);

                    if (hasMissingConfirmation || hasNotConfirmedStatus)
                    {
                        throw new ApiException("Chi tiết chấm công chưa được xác nhận hết, không thể chuyển tính lương.");
                    }
                }
            }

            var entity = _mapper.Map<Payroll>(request);

            // Thêm các bảng chấm công tổng hợp vào bảng lương
            if (request.SummaryTimesheetNameIds != null && request.SummaryTimesheetNameIds.Any())
            {
                var summaryTimesheets = await _dbContext.SummaryTimesheetNames
                    .Where(stn => request.SummaryTimesheetNameIds.Contains(stn.Id))
                    .ToListAsync();

                if (entity.PayrollSummaryTimesheets == null)
                    entity.PayrollSummaryTimesheets = new List<PayrollSummaryTimesheet>();

                foreach (var summaryTimesheet in summaryTimesheets)
                {
                    entity.PayrollSummaryTimesheets.Add(new PayrollSummaryTimesheet
                    {
                        Payroll = entity,
                        SummaryTimesheetName = summaryTimesheet
                    });
                }
            }

            // Thêm các vị trí công việc vào bảng lương
            if (request.StaffPositionIds != null && request.StaffPositionIds.Any())
            {
                var staffPositions = await _dbContext.StaffPositions
                    .Where(sp => request.StaffPositionIds.Contains(sp.Id))
                    .ToListAsync();

                if (entity.PayrollStaffPositions == null)
                    entity.PayrollStaffPositions = new List<PayrollStaffPosition>();

                foreach (var staffPosition in staffPositions)
                {
                    entity.PayrollStaffPositions.Add(new PayrollStaffPosition
                    {
                        Payroll = entity,
                        StaffPosition = staffPosition
                    });
                }
            }

            await CreateAsync(entity);
            return _mapper.Map<PayrollDto>(entity);
        }

        private async Task<List<int>> GetAllChildOrganizationIds(int parentId)
        {
            var allOrganizations = await _dbContext.Organizations.AsNoTracking().ToListAsync();
            var result = new List<int>();
            GetChildIdsRecursive(parentId, allOrganizations, result);
            return result;
        }

        private static void GetChildIdsRecursive(int parentId, List<Core.Data.Company.Organization> allOrganizations, List<int> result)
        {
            var children = allOrganizations.Where(o => o.OrganizatioParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child.Id);
                GetChildIdsRecursive(child.Id, allOrganizations, result);
            }
        }

        public async Task Delete(int id)
        {
            var entity = await GetPayrollAndCheckExist(id);

            if (entity.PayrollStatus == PayrollStatus.Locked)
            {
                throw new ApiException("Bảng lương đã khoá thì không xoá được");
            }

            if (entity.PayrollConfirmationStatus == PayrollConfirmationStatus.Confirmed)
            {
                throw new ApiException("Bảng lương đã xác nhận hết thì không xoá được");
            }

            var payrollDetails = await _dbContext.PayrollDetails
                .Where(x => x.PayrollId == id && x.IsDeleted != true)
                .Select(x => x.ConfirmationStatus)
                .ToListAsync();

            if (payrollDetails.Any() && payrollDetails.All(x => x == PayrollConfirmationStatusEmployee.Confirmed))
            {
                throw new ApiException("Bảng lương đã xác nhận hết thì không xoá được");
            }

            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }

        public async Task<PayrollDto> GetById(int id)
        {
            var entity = await _dbContext.Payrolls
                .Include(p => p.Organization)
                .Include(p => p.PayrollSummaryTimesheets)
                    .ThenInclude(pst => pst.SummaryTimesheetName)
                .Include(p => p.PayrollStaffPositions)
                    .ThenInclude(psp => psp.StaffPosition)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true);

            return entity == null ? throw new EntityNotFoundException(nameof(Payroll), $"Id = {id}") : _mapper.Map<PayrollDto>(entity);
        }

        public async Task<PagingResult<PayrollDto>> Paging(int? organizationId, string? name, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Payrolls
                .Include(p => p.Organization)
                .Include(p => p.PayrollSummaryTimesheets)
                    .ThenInclude(pst => pst.SummaryTimesheetName)
                .Include(p => p.PayrollStaffPositions)
                    .ThenInclude(psp => psp.StaffPosition)
                .Where(p => p.IsDeleted != true)
                .AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == organizationId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.PayrollName.Contains(name));
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<PayrollDto>(query).ToListAsync();

            var result = new PagingResult<PayrollDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<PagingResult<PayrollDto>> PagingForEmployee(int? organizationId, string? name, int? employeeId, int? Year, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            // Lọc payrollId từ bảng PayrollDetail theo organizationId và employeeId
            var payrollIds = await _dbContext.PayrollDetails
                .Where(pd => pd.OrganizationId == organizationId && pd.EmployeeId == employeeId)
                .Select(pd => pd.PayrollId)
                .ToListAsync();

            if (!payrollIds.Any())
            {
                throw new Exception("Không có bảng lương tương ứng với nhân viên này");
            }

            // Lọc payroll theo danh sách payrollId
            var query = _dbContext.Payrolls
                .Include(p => p.Organization)
                .Include(p => p.PayrollSummaryTimesheets)
                    .ThenInclude(pst => pst.SummaryTimesheetName)
                .Include(p => p.PayrollStaffPositions)
                    .ThenInclude(psp => psp.StaffPosition)
                .Where(p => payrollIds.Contains(p.Id) && p.IsDeleted != true && p.CreatedAt.Value.Year == Year)
                .AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == organizationId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.PayrollName.Contains(name));
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<PayrollDto>(query).ToListAsync();

            var result = new PagingResult<PayrollDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;

        }

        public async Task Update(int id, UpdatePayrollRequest request)
        {
            var entity = await GetPayrollAndCheckExist(id);
            await UpdateAsync(_mapper.Map(request, entity));
        }

        private async Task<Payroll> GetPayrollAndCheckExist(int payrollId)
        {
            var payroll = await _dbContext.Payrolls.FindAsync(payrollId);
            if (payroll is null)
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
            return payroll;
        }

        public async Task TogglePayrollStatus(int payrollId)
        {
            var payroll = await _dbContext.Payrolls.FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);
            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");

            }    

            payroll.PayrollStatus = payroll.PayrollStatus == PayrollStatus.Unlocked 
                ? PayrollStatus.Locked
                : PayrollStatus.Unlocked;

            await UpdateAsync(payroll);
        }

        public async Task<bool> IsPayrollLocked(int payrollId)
        {
            var payroll = await _dbContext.Payrolls.FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);
            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
            }

            return payroll.PayrollStatus == PayrollStatus.Locked;
        }

    }
}
