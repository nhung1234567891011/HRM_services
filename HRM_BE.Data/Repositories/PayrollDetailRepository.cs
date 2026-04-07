using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;

namespace HRM_BE.Data.Repositories
{
    public class PayrollDetailRepository : RepositoryBase<PayrollDetail, int>, IPayrollDetailRepository
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PayrollDetailRepository> _logger;

        public PayrollDetailRepository(
            HrmContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PayrollDetailRepository> logger) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PayrollDetailDto> GetById(int id)
        {
            var entity = await GetPayrollDetailAndCheckExist(id);
            return _mapper.Map<PayrollDetailDto>(entity);
        }

        public async Task<PagingResult<PayrollDetailDto>> Paging(int? organizationId, string? name, int? payrollId, int? employeeId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            // Lấy SummaryTimesheetNameIds liên kết với payroll (nếu có) để kiểm tra xác nhận
            List<int?> summaryTimesheetNameIds = new List<int?>();
            DateTime? periodStartDate = null;
            DateTime? periodEndDate = null;
            
            if (payrollId.HasValue)
            {
                summaryTimesheetNameIds = _dbContext.PayrollSummaryTimesheets
                    .Where(pst => pst.PayrollId == payrollId)
                    .Select(pst => pst.SummaryTimesheetNameId)
                    .ToList();

                // Xác định khoảng thời gian kỳ lương
                if (summaryTimesheetNameIds.Any())
                {
                    var payroll = _dbContext.Payrolls.FirstOrDefault(p => p.Id == payrollId.Value);
                    var period = _dbContext.SummaryTimesheetNames
                        .Where(s => summaryTimesheetNameIds.Contains(s.Id))
                        .Select(s => new
                        {
                            MinStartDate = s.SummaryTimesheetNameDetailTimesheetNames
                                .Min(d => d.DetailTimesheetName.StartDate),
                            MaxEndDate = s.SummaryTimesheetNameDetailTimesheetNames
                                .Max(d => d.DetailTimesheetName.EndDate)
                        })
                        .FirstOrDefault();

                    if (period != null)
                    {
                        var fallback = payroll?.CreatedAt ?? DateTime.Now;
                        periodStartDate = (period.MinStartDate ?? fallback).Date;
                        periodEndDate = (period.MaxEndDate ?? fallback).Date;
                    }
                }
            }

            var now = DateTime.Now;

            var query = _dbContext.PayrollDetails
                .Where(p => p.IsDeleted != true)
                // Hợp đồng còn hạn - nếu có period thì kiểm tra contract trong khoảng thời gian đó
                .Where(p => p.Employee.Contracts.Any(c => 
                    c.ExpiredStatus == false &&
                    (!periodStartDate.HasValue || !periodEndDate.HasValue ||
                     (c.EffectiveDate.HasValue && c.EffectiveDate.Value <= periodEndDate.Value &&
                      (!c.ExpiryDate.HasValue || c.ExpiryDate.Value >= periodStartDate.Value)))))
                // Đã xác nhận bảng chấm công HOẶC quá ngày xác nhận
                .Where(p => !summaryTimesheetNameIds.Any() ||
                    p.Employee.SummaryTimesheetNameEmployeeConfirms.Any(s =>
                        summaryTimesheetNameIds.Contains(s.SummaryTimesheetNameId) &&
                        (s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Confirm ||
                         (s.Date != null && s.Date < now))))
                .Include(p => p.Employee)
                .ThenInclude(e => e.Deductions)
                .AsQueryable();

            if (payrollId.HasValue)
            {
                query = query.Where(p => p.PayrollId == payrollId);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(p => p.EmployeeId == employeeId);
            }

            if (organizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == organizationId);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                var pattern = $"%{keyword}%";

                query = query.Where(p =>
                    (p.FullName != null && EF.Functions.Like(p.FullName, pattern)) ||
                    (p.EmployeeCode != null && EF.Functions.Like(p.EmployeeCode, pattern)) ||
                    (p.Employee != null && (
                        (p.Employee.PhoneNumber != null && EF.Functions.Like(p.Employee.PhoneNumber, pattern)) ||
                        (p.Employee.WorkPhoneNumber != null && EF.Functions.Like(p.Employee.WorkPhoneNumber, pattern)) ||
                        (p.Employee.StaffPosition != null && p.Employee.StaffPosition.PositionName != null && EF.Functions.Like(p.Employee.StaffPosition.PositionName, pattern)) ||
                        (p.Employee.StaffTitle != null && p.Employee.StaffTitle.StaffTitleName != null && EF.Functions.Like(p.Employee.StaffTitle.StaffTitleName, pattern))
                    ))
                );
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<PayrollDetailDto>(query).ToListAsync();

            var result = new PagingResult<PayrollDetailDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<List<PayrollDetailDto>> GetExportData(ExportPayrollDetailRequest request)
        {
            var query = _dbContext.PayrollDetails
                .Where(p => p.IsDeleted != true)
                .AsQueryable();

            if (request.OrganizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == request.OrganizationId);
            }

            if (request.EmployeeId.HasValue)
            {
                query = query.Where(p => p.EmployeeId == request.EmployeeId);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var keyword = request.Name.Trim();
                var pattern = $"%{keyword}%";

                query = query.Where(p =>
                    (p.FullName != null && EF.Functions.Like(p.FullName, pattern)) ||
                    (p.EmployeeCode != null && EF.Functions.Like(p.EmployeeCode, pattern)));
            }

            if (request.DetailTimesheetId.HasValue)
            {
                var detailTimesheetId = request.DetailTimesheetId.Value;
                query = query.Where(p =>
                    p.PayrollId.HasValue &&
                    _dbContext.PayrollSummaryTimesheets.Any(ps =>
                        ps.PayrollId == p.PayrollId &&
                        ps.SummaryTimesheetNameId.HasValue &&
                        _dbContext.SummaryTimesheetNameDetailTimesheetNames.Any(sd =>
                            sd.SummaryTimesheetNameId == ps.SummaryTimesheetNameId &&
                            sd.DetailTimesheetNameId == detailTimesheetId)));
            }

            if (request.StartDate.HasValue || request.EndDate.HasValue)
            {
                var startDate = request.StartDate?.Date ?? DateTime.MinValue.Date;
                var endDate = request.EndDate?.Date ?? DateTime.MaxValue.Date;

                query = query.Where(p =>
                    p.PayrollId.HasValue &&
                    _dbContext.PayrollSummaryTimesheets.Any(ps =>
                        ps.PayrollId == p.PayrollId &&
                        ps.SummaryTimesheetNameId.HasValue &&
                        _dbContext.SummaryTimesheetNameDetailTimesheetNames.Any(sd =>
                            sd.SummaryTimesheetNameId == ps.SummaryTimesheetNameId &&
                            sd.DetailTimesheetName.StartDate.HasValue &&
                            sd.DetailTimesheetName.EndDate.HasValue &&
                            sd.DetailTimesheetName.StartDate.Value.Date <= endDate &&
                            sd.DetailTimesheetName.EndDate.Value.Date >= startDate)));
            }

            query = query.ApplySorting(request.SortBy ?? "Id", request.OrderBy ?? "desc");

            return await _mapper.ProjectTo<PayrollDetailDto>(query).ToListAsync();
        }


        private async Task<PayrollDetail> GetPayrollDetailAndCheckExist(int payrollDetailId)
        {
            var payrollDetail = await _dbContext.PayrollDetails.FindAsync(payrollDetailId);
            if (payrollDetail is null)
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {payrollDetailId}");
            return payrollDetail;
        }

        /// <summary>
        /// Tính và lưu PayrollDetail cho từng nhân viên. Giờ tiêu chuẩn được lấy theo ShiftCatalog.WorkingHours của từng ca;
        /// OvertimeAmount được tính riêng theo hệ số 200%.
        /// Khi dữ liệu chấm công thay đổi hoặc logic tính ngày công/OT thay đổi, cần gọi lại phương thức này cho kỳ lương tương ứng để cập nhật.
        /// </summary>
        public async Task CalculateAndSavePayrollDetails(int payrollId)
        {
            // Lấy Payroll
            var payroll = _dbContext.Payrolls.FirstOrDefault(p => p.Id == payrollId);
            if (payroll == null)
            {
                throw new Exception($"Payroll có Id = {payrollId} không tồn tại!");
            }

            // Lấy danh sách SummaryTimesheetNameId liên kết với Payroll này
            var summaryTimesheetNameIds = _dbContext.PayrollSummaryTimesheets
                .Where(pst => pst.PayrollId == payrollId)
                .Select(pst => pst.SummaryTimesheetNameId)
                .ToList();

            // Xác định khoảng thời gian kỳ lương dựa trên các bảng công tổng hợp đã gắn với Payroll.
            // Nếu không có bảng công nào, fallback về tháng/năm của CreatedAt như cũ.
            DateTime periodStartDate;
            DateTime periodEndDate;
            List<(DateTime Start, DateTime End)> selectedDateRanges;

            if (summaryTimesheetNameIds.Any())
            {
                selectedDateRanges = _dbContext.SummaryTimesheetNameDetailTimesheetNames
                    .Where(x => summaryTimesheetNameIds.Contains(x.SummaryTimesheetNameId)
                             && x.DetailTimesheetName.StartDate.HasValue
                             && x.DetailTimesheetName.EndDate.HasValue)
                    .Select(x => new
                    {
                        Start = x.DetailTimesheetName.StartDate!.Value.Date,
                        End = x.DetailTimesheetName.EndDate!.Value.Date
                    })
                    .ToList()
                    .Select(x => (x.Start, x.End))
                    .Distinct()
                    .ToList();

                var period = _dbContext.SummaryTimesheetNames
                    .Where(s => summaryTimesheetNameIds.Contains(s.Id))
                    .Select(s => new
                    {
                        MinStartDate = s.SummaryTimesheetNameDetailTimesheetNames
                            .Min(d => d.DetailTimesheetName.StartDate),
                        MaxEndDate = s.SummaryTimesheetNameDetailTimesheetNames
                            .Max(d => d.DetailTimesheetName.EndDate)
                    })
                    .FirstOrDefault();

                var fallback = payroll.CreatedAt ?? DateTime.Now;
                periodStartDate = (period?.MinStartDate ?? fallback).Date;
                periodEndDate = (period?.MaxEndDate ?? fallback).Date;

                if (!selectedDateRanges.Any())
                {
                    selectedDateRanges = new List<(DateTime Start, DateTime End)>
                    {
                        (periodStartDate, periodEndDate)
                    };
                }
            }
            else
            {
                var payrollMonth = payroll.CreatedAt ?? DateTime.Now;
                periodStartDate = new DateTime(payrollMonth.Year, payrollMonth.Month, 1);
                periodEndDate = periodStartDate.AddMonths(1).AddDays(-1);
                selectedDateRanges = new List<(DateTime Start, DateTime End)>
                {
                    (periodStartDate, periodEndDate)
                };
            }

            bool IsDateInSelectedRanges(DateTime date) => selectedDateRanges.Any(r => date >= r.Start && date <= r.End);
            bool IsOverlapWithSelectedRanges(DateTime start, DateTime end) => selectedDateRanges.Any(r => start < r.End && end > r.Start);

            var now = DateTime.Now;
            var selectedRangesText = string.Join(", ", selectedDateRanges.Select(r => $"{r.Start:yyyy-MM-dd}->{r.End:yyyy-MM-dd}"));
            _logger.LogInformation(
                "Start payroll detail calculation. PayrollId={PayrollId}, OrganizationId={OrganizationId}, PeriodStart={PeriodStart}, PeriodEnd={PeriodEnd}, SelectedRanges={SelectedRanges}",
                payrollId,
                payroll.OrganizationId,
                periodStartDate.ToString("yyyy-MM-dd"),
                periodEndDate.ToString("yyyy-MM-dd"),
                selectedRangesText);

            // 1. Lấy danh sách nhân viên thuộc tổ chức của Payroll
            // Điều kiện:
            //   - Có hợp đồng còn hiệu lực trong thời gian của bảng tổng hợp (EffectiveDate <= periodEndDate và (ExpiryDate >= periodStartDate hoặc ExpiryDate == null))
            //   - Hợp đồng chưa hết hạn (ExpiredStatus == false)
            //   - Đã xác nhận bảng chấm công tổng hợp (Status == Confirm)
            //     HOẶC đã quá ngày xác nhận (Date < now)
            var employees = _dbContext.Employees
                .Where(e => e.OrganizationId == payroll.OrganizationId)
                .Where(e => e.Contracts.Any(c => 
                    c.EmployeeId == e.Id && 
                    c.ExpiredStatus == false &&
                    c.EffectiveDate.HasValue && c.EffectiveDate.Value <= periodEndDate &&
                    (!c.ExpiryDate.HasValue || c.ExpiryDate.Value >= periodStartDate)))
                .Where(e => e.SummaryTimesheetNameEmployeeConfirms.Any(s =>
                    summaryTimesheetNameIds.Contains(s.SummaryTimesheetNameId) &&
                    (s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Confirm ||
                     (s.Date != null && s.Date < now))))
                .Include(e => e.StaffPosition)
                .ToList();

            _logger.LogInformation(
                "Payroll detail calculation context. PayrollId={PayrollId}, EmployeeCount={EmployeeCount}",
                payrollId,
                employees.Count);

            var standardWorkDays = _dbContext.ShiftWorks
                .Where(sw => sw.OrganizationId == payroll.OrganizationId)
                .Sum(sw => sw.TotalWork);

            var holidaysInPeriod = _dbContext.Holidays
                .Where(h => h.OrganizationId == payroll.OrganizationId
                         && h.FromDate.Date <= periodEndDate
                         && h.ToDate.Date >= periodStartDate
                         && h.IsDeleted != true)
                .Select(h => new { h.Id, h.FromDate, h.ToDate })
                .ToList();

            var holidayDates = new HashSet<DateTime>();
            foreach (var holiday in holidaysInPeriod)
            {
                var fromDate = holiday.FromDate.Date < periodStartDate ? periodStartDate : holiday.FromDate.Date;
                var toDate = holiday.ToDate.Date > periodEndDate ? periodEndDate : holiday.ToDate.Date;

                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    holidayDates.Add(date);
                }
            }

            var payrollDetails = new List<PayrollDetail>();

            foreach (var employee in employees)
            {
                // Lấy contract còn hiệu lực trong khoảng thời gian của bảng tổng hợp
                // Nếu có nhiều contract, ưu tiên contract có EffectiveDate gần nhất
                var contract = _dbContext.Contracts
                    .Where(c => c.EmployeeId == employee.Id && 
                                c.ExpiredStatus == false &&
                                c.EffectiveDate.HasValue && c.EffectiveDate.Value <= periodEndDate &&
                                (!c.ExpiryDate.HasValue || c.ExpiryDate.Value >= periodStartDate))
                    .OrderByDescending(c => c.EffectiveDate)
                    .FirstOrDefault();
                var timesheet = _dbContext.Timesheets.Where(t => t.EmployeeId == employee.Id);

                var employeeTimesheetsInPeriod = timesheet
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodStartDate
                                && t.Date.Value.Date <= periodEndDate)
                    .ToList()
                    .Where(t => t.Date.HasValue && IsDateInSelectedRanges(t.Date.Value.Date))
                    .ToList();

                var shiftWorkIds = employeeTimesheetsInPeriod
                    .Where(t => t.ShiftWorkId.HasValue)
                    .Select(t => t.ShiftWorkId!.Value)
                    .Distinct()
                    .ToList();

                var shiftSalaryConfigById = _dbContext.ShiftWorks
                    .Include(sw => sw.ShiftCatalog)
                    .Where(sw => shiftWorkIds.Contains(sw.Id))
                    .ToDictionary(
                        sw => sw.Id,
                        sw => (
                            StandardHours: ResolveShiftStandardHours(sw.ShiftCatalog),
                            RegularMultiplier: (decimal)(sw.ShiftCatalog.RegularMultiplier > 0 ? sw.ShiftCatalog.RegularMultiplier.Value : 1.0),
                            HolidayMultiplier: (decimal)(sw.ShiftCatalog.HolidayMultiplier > 0 ? sw.ShiftCatalog.HolidayMultiplier.Value : 2.0)
                        ));

                // Lấy bảng KpiDetail theo kỳ lương (theo khoảng ngày periodStartDate/periodEndDate)
                var kpiDetail = _dbContext.KpiTableDetails.Include(k => k.KpiTable)
                    .FirstOrDefault(k =>
                        k.EmployeeId == employee.Id &&
                        k.KpiTable != null &&
                        k.KpiTable.ToDate.HasValue &&
                        k.KpiTable.ToDate.Value.Date >= periodStartDate &&
                        k.KpiTable.ToDate.Value.Date <= periodEndDate);

                // Lấy các khoản khấu trừ
                var deductions = _dbContext.Deductions.Where(d => d.EmployeeId == employee.Id).ToList();
                var totalDeductions = deductions.Sum(d => d.Value); // Tổng khấu trừ

                // 2. Tính toán các thành phần lương
                var baseSalary = contract?.SalaryAmount ?? 0;

                // Số ngày đi làm thực tế: chỉ tính khi có chấm ra để tránh cộng sai ngày chỉ chấm vào.
                var workingDayDates = employeeTimesheetsInPeriod
                    .Where(t =>
                        t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None &&
                        t.EndTime.HasValue &&
                        (
                            (t.NumberOfWorkingHour ?? 0) > 0
                            // Trường hợp giờ làm chưa tính nhưng đã có đủ check-in/check-out trong ngày
                            || (t.NumberOfWorkingHour == null && t.StartTime.HasValue)
                        ))
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .ToList();
                // attendanceDays: chỉ tính những ngày thật sự đi làm (không bao gồm ngày nghỉ hưởng lương)
                var attendanceDays = workingDayDates.Count;

                // Số ngày nghỉ nhưng vẫn được hưởng lương (đơn nghỉ phép đã duyệt, SalaryPercentage > 0 hoặc OnPaidLeaveStatus = Yes)
                var paidLeaveDaysCount = _dbContext.LeaveApplications
                    .Where(l =>
                        l.EmployeeId == employee.Id &&
                        l.Status == LeaveApplicationStatus.Approved &&
                        l.StartDate.HasValue &&
                        l.EndDate.HasValue &&
                        l.StartDate.Value.Date < periodEndDate &&
                        l.EndDate.Value.Date > periodStartDate &&
                        (l.SalaryPercentage > 0 || l.OnPaidLeaveStatus == OnPaidLeaveStatus.Yes))
                    .Select(l => new
                    {
                        StartDate = l.StartDate!.Value,
                        EndDate = l.EndDate!.Value,
                        NumberOfDays = l.NumberOfDays ?? 0
                    })
                    .ToList()
                    .Where(l => IsOverlapWithSelectedRanges(l.StartDate.Date, l.EndDate.Date))
                    .Sum(l => l.NumberOfDays);

                // Tập ngày đã có đi làm để loại khỏi nhóm nghỉ lễ (tránh cộng trùng khi dữ liệu chấm công thiếu giờ ra).
                var attendanceDatesForHolidayOff = employeeTimesheetsInPeriod
                    .Where(t =>
                        t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None
                        && t.Date.HasValue
                        && (
                            (t.NumberOfWorkingHour ?? 0) > 0
                            || (t.StartTime.HasValue && t.EndTime.HasValue)
                        ))
                    .Select(t => t.Date!.Value.Date)
                    .ToHashSet();

                // Ngày lễ trong kỳ mà nhân viên KHÔNG đi làm (không có chấm công đi làm) → vẫn được tính 1 ngày công.
                var holidayDaysOffDates = holidayDates
                    .Where(d => d >= periodStartDate
                             && d <= periodEndDate
                             && IsDateInSelectedRanges(d)
                             && !attendanceDatesForHolidayOff.Contains(d))
                    .ToList();

                var holidayDaysOff = holidayDaysOffDates.Count;

                // ActualWorkDays hiển thị trên phiếu lương: ngày đi làm thực tế + ngày nghỉ hưởng lương + ngày nghỉ lễ tết
                var actualWorkDaysAll = attendanceDays + paidLeaveDaysCount + holidayDaysOff;

                // Ngày công quy đổi từ giờ làm tiêu chuẩn theo từng ca (ShiftCatalog.WorkingHours), KHÔNG bao gồm giờ tăng ca.
                // Giờ tăng ca được tính riêng trong OvertimeAmount với hệ số 200%.
                var dailySalaryForReceivedSalary = standardWorkDays > 0 ? baseSalary / (decimal)standardWorkDays : 0;

                // Lương trong giờ hành chính theo từng ca: lương ngày / giờ chuẩn ca.
                var receivedSalaryFromAttendance = employeeTimesheetsInPeriod
                    .Sum(t =>
                    {
                        var shiftConfig = GetTimesheetShiftConfig(t, shiftSalaryConfigById);
                        if (shiftConfig.StandardHours <= 0m) return 0m;

                        var workedHours = Math.Max((decimal)(t.NumberOfWorkingHour ?? 0), 0m);
                        var normalHours = Math.Min(workedHours, shiftConfig.StandardHours);
                        var hourlyRateByShift = dailySalaryForReceivedSalary / shiftConfig.StandardHours;
                        return normalHours * hourlyRateByShift * shiftConfig.RegularMultiplier;
                    });

                var receivedSalary = receivedSalaryFromAttendance +
                                     ((decimal)paidLeaveDaysCount * dailySalaryForReceivedSalary);
                var kpi = contract?.KpiSalary ?? 0;
                var kpiPercentage = kpiDetail?.CompletionRate ?? 0;
                var kpiSalary = (decimal)kpi * ((decimal)kpiPercentage / 100);
                var bonus = kpiDetail?.Bonus ?? 0;

                // Lương 1 ngày
                var dailySalary = standardWorkDays > 0 ? baseSalary / (decimal)standardWorkDays : 0;

                // === Các khoản sau đều lấy từ Salary Component (bảng SalaryComponents) theo ComponentCode ===
                // - Thu nhập: ALLOWANCE_MEAL_TRAVEL, PARKING (và Hoa hồng qua chính sách doanh thu)
                // - Khấu trừ: BHXH, BHTN, BHYT, QUY_CONG_DOAN
                // Nếu không tìm thấy component (đúng OrganizationId + Code + Status=Tracking) thì dùng defaultValue.
                var organizationIdForComponent = payroll.OrganizationId ?? employee.OrganizationId;

                // SalaryRate hiển thị theo hệ số ngày thường của ca (quy đổi %),
                // nhưng KHONG dùng để nhân lại vào coreSalary nhằm tránh nhân đôi.
                var salaryRate = employeeTimesheetsInPeriod
                    .Where(t => t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None
                             && (t.NumberOfWorkingHour ?? 0) > 0)
                    .Select(t => GetTimesheetShiftConfig(t, shiftSalaryConfigById).RegularMultiplier)
                    .DefaultIfEmpty(1m)
                    .Average() * 100m;

                var allowanceMealTravel = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "ALLOWANCE_MEAL_TRAVEL",
                    defaultValue: 1_000_000m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                var parkingAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "PARKING",
                    defaultValue: 3_000m * (decimal)attendanceDays,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Giờ tăng ca: lấy từ OvertimeHour nếu đã có, nếu không fallback theo giờ chuẩn của ca.
                // Tiền OT = giờ OT quy đổi * 2 * lương giờ theo ca.
                var overtimeAmount = employeeTimesheetsInPeriod
                    .Sum(t =>
                    {
                        var shiftConfig = GetTimesheetShiftConfig(t, shiftSalaryConfigById);
                        if (shiftConfig.StandardHours <= 0m) return 0m;

                        var overtimeHours = (decimal)(t.OvertimeHour ?? 0);
                        if (overtimeHours <= 0m)
                        {
                            overtimeHours = Math.Max((decimal)(t.NumberOfWorkingHour ?? 0) - shiftConfig.StandardHours, 0m);
                        }

                        if (overtimeHours <= 0m) return 0m;

                        var roundedOtHours = RoundOtHours(overtimeHours);
                        var hourlyRateByShift = dailySalary / shiftConfig.StandardHours;
                        return roundedOtHours * 2m * hourlyRateByShift;
                    });

                // ===== Lương phụ trội ngày nghỉ lễ =====
                decimal holidayWorkAmount = 0m;
                decimal holidayWorkedAmount = 0m;
                decimal holidayOffAmount = 0m;
                if (holidayDates.Any())
                {
                    // Dùng ca phổ biến nhất của nhân viên làm fallback cho ngày lễ không có timesheet.
                    var defaultShiftConfigForHolidayOff = employeeTimesheetsInPeriod
                        .Where(t => t.ShiftWorkId.HasValue && shiftSalaryConfigById.ContainsKey(t.ShiftWorkId.Value))
                        .GroupBy(t => t.ShiftWorkId!.Value)
                        .OrderByDescending(g => g.Count())
                        .Select(g => shiftSalaryConfigById[g.Key])
                        .FirstOrDefault();

                    if (defaultShiftConfigForHolidayOff.StandardHours <= 0m)
                    {
                        defaultShiftConfigForHolidayOff = (8m, 1m, 2m);
                    }

                    // 1) Ngày lễ có đi làm: tính theo số giờ thực tế của timesheet.
                    var holidayWorkingTimesheets = employeeTimesheetsInPeriod
                        .Where(t => t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None
                                 && t.Date.HasValue
                                 && holidayDates.Contains(t.Date.Value.Date)
                                 && (t.NumberOfWorkingHour ?? 0) > 0)
                        .ToList();

                    if (holidayWorkingTimesheets.Any())
                    {
                        holidayWorkedAmount = holidayWorkingTimesheets.Sum(t =>
                        {
                            if (!t.Date.HasValue) return 0m;

                            var shiftConfig = GetTimesheetShiftConfig(t, shiftSalaryConfigById);
                            if (shiftConfig.StandardHours <= 0m) return 0m;

                            var effectiveHolidayMultiplier = shiftConfig.HolidayMultiplier > 0m
                                ? shiftConfig.HolidayMultiplier
                                : 2.0m;
                            if (effectiveHolidayMultiplier <= 0m) return 0m;

                            var workedHours = Math.Max((decimal)(t.NumberOfWorkingHour ?? 0), 0m);
                            var hourlyRateByShift = dailySalary / shiftConfig.StandardHours;
                            return workedHours * hourlyRateByShift * effectiveHolidayMultiplier;
                        });
                    }

                    // 2) Ngày lễ nghỉ (không có chấm công đi làm): lấy giờ chuẩn 1 ngày công theo ShiftCatalog để tính phụ trội lễ.
                    if (holidayDaysOffDates.Any())
                    {
                        var holidayTimesheetByDate = employeeTimesheetsInPeriod
                            .Where(t => t.Date.HasValue)
                            .GroupBy(t => t.Date!.Value.Date)
                            .ToDictionary(g => g.Key, g => g.FirstOrDefault());

                        holidayOffAmount = holidayDaysOffDates.Sum(holidayDate =>
                        {
                            var hasTimesheetOnDate = holidayTimesheetByDate.TryGetValue(holidayDate, out var holidayTimesheetOnDate)
                                && holidayTimesheetOnDate != null;

                            var shiftConfig = hasTimesheetOnDate
                                ? GetTimesheetShiftConfig(holidayTimesheetOnDate!, shiftSalaryConfigById)
                                : defaultShiftConfigForHolidayOff;

                            if (shiftConfig.StandardHours <= 0m) return 0m;

                            var effectiveHolidayMultiplier = shiftConfig.HolidayMultiplier > 0m
                                ? shiftConfig.HolidayMultiplier
                                : 2.0m;
                            if (effectiveHolidayMultiplier <= 0m) return 0m;

                            // Nghỉ lễ không đi làm: tính theo lương 1 ngày * hệ số ngày lễ của ca.
                            var workedHours = shiftConfig.StandardHours;
                            var hourlyRateByShift = dailySalary / shiftConfig.StandardHours;
                            return workedHours * hourlyRateByShift * effectiveHolidayMultiplier;
                        });
                    }

                    holidayWorkAmount = holidayWorkedAmount + holidayOffAmount;
                }

                // Hoa hồng doanh thu (cấu hình theo bậc)
                var revenue = kpiDetail?.Revenue ?? 0m;
                var payrollDate = periodEndDate;
                var commissionAmount = ResolveRevenueCommissionAmount(
                    organizationIdForComponent,
                    employee?.StaffPosition?.PositionCode,
                    revenue,
                    payrollDate);

                // BHXH: 100% từ Salary Component (ComponentCode = "BHXH"), thường % lương đóng BH
                //var bhxhAmount = ResolveSalaryComponentWithRule(
                //    organizationIdForComponent,
                //    "BHXH",
                //    defaultValue: 0m,
                //    contractSalaryAmount: baseSalary,
                //    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                //    receivedSalary: receivedSalary,
                //    attendanceDays: attendanceDays);

                var bhxhAmount = (contract?.SalaryInsurance ?? 0m) > 0m
                    ? contract!.SalaryInsurance!.Value
                    : baseSalary;

                // BHTN: 100% từ Salary Component (ComponentCode = "BHTN")
                var bhtnAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "BHTN",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // BHYT: 100% từ Salary Component (ComponentCode = "BHYT")
                var bhytAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "BHYT",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Gộp BHXH + BHTN + BHYT vào một cột BhxhAmount (tránh thêm cột DB)
                var totalInsuranceDeduction = bhxhAmount + bhtnAmount + bhytAmount;

                // Quỹ công đoàn: 100% từ Salary Component (ComponentCode = "QUY_CONG_DOAN")
                var unionFeeAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "QUY_CONG_DOAN",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Công thức lương lõi: chỉ gồm lương theo công + KPI + thưởng.
                var coreSalary = (decimal)receivedSalary + (decimal)kpiSalary + (decimal)bonus;

                // Tổng lương (trước trừ BHXH, Quỹ công đoàn): Lương cứng + Phụ cấp đi lại ăn trưa + Tiền gửi xe + Hoa hồng + Lương tăng ca + Phụ trội ngày lễ
                var totalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount + holidayWorkAmount + commissionAmount;

                // Tổng thực nhận = Tổng lương - (BHXH+BHTN+BHYT) - Quỹ công đoàn - Khấu trừ khác
                var totalReceivedSalary = totalSalary - (totalDeductions + totalInsuranceDeduction + unionFeeAmount);

                _logger.LogInformation(
                    "Payroll detail calculated. PayrollId={PayrollId}, EmployeeId={EmployeeId}, EmployeeCode={EmployeeCode}, FullName={FullName}, BaseSalary={BaseSalary}, StandardWorkDays={StandardWorkDays}, AttendanceDays={AttendanceDays}, PaidLeaveDays={PaidLeaveDays}, HolidayDaysOff={HolidayDaysOff}, ActualWorkDays={ActualWorkDays}, ReceivedSalary={ReceivedSalary}, KPI={KPI}, KpiPercent={KpiPercent}, KpiSalary={KpiSalary}, Bonus={Bonus}, SalaryRate={SalaryRate}, AllowanceMealTravel={AllowanceMealTravel}, ParkingAmount={ParkingAmount}, OvertimeAmount={OvertimeAmount}, HolidayWorkedAmount={HolidayWorkedAmount}, HolidayOffAmount={HolidayOffAmount}, HolidayWorkAmount={HolidayWorkAmount}, CommissionAmount={CommissionAmount}, InsuranceDeduction={InsuranceDeduction}, UnionFee={UnionFee}, OtherDeductions={OtherDeductions}, TotalSalary={TotalSalary}, TotalReceivedSalary={TotalReceivedSalary}",
                    payrollId,
                    employee.Id,
                    employee.EmployeeCode,
                    (employee.LastName + " " + employee.FirstName).Trim(),
                    baseSalary,
                    standardWorkDays,
                    attendanceDays,
                    paidLeaveDaysCount,
                    holidayDaysOff,
                    actualWorkDaysAll,
                    receivedSalary,
                    kpi,
                    kpiPercentage,
                    kpiSalary,
                    bonus,
                    salaryRate,
                    allowanceMealTravel,
                    parkingAmount,
                    overtimeAmount,
                    holidayWorkedAmount,
                    holidayOffAmount,
                    holidayWorkAmount,
                    commissionAmount,
                    totalInsuranceDeduction,
                    unionFeeAmount,
                    totalDeductions,
                    totalSalary,
                    totalReceivedSalary);

                // 3. Lưu vào bảng PayrollDetail
                payrollDetails.Add(new PayrollDetail
                {
                    OrganizationId = employee.OrganizationId,
                    PayrollId = payrollId,
                    EmployeeId = employee.Id,
                    ContractId = contract?.Id,
                    EmployeeCode = employee.EmployeeCode,
                    FullName = employee.LastName + " " + employee.FirstName,
                    ContractTypeStatus = contract?.ContractTypeStatus ?? ContractTypeStatus.Official,
                    BaseSalary = baseSalary,
                    StandardWorkDays = standardWorkDays,
                    // Ngày công thực tế hiển thị trên phiếu lương: ngày đi làm + ngày nghỉ hưởng lương
                    ActualWorkDays = actualWorkDaysAll,
                    ReceivedSalary = receivedSalary,
                    KPI = (decimal)kpi,
                    KpiPercentage = (decimal)kpiPercentage,
                    KpiSalary = (decimal)kpiSalary,
                    Bonus = (decimal)bonus,

                    SalaryRate = salaryRate,
                    AllowanceMealTravel = allowanceMealTravel,
                    ParkingAmount = parkingAmount,
                    OvertimeAmount = overtimeAmount,
                    HolidayWorkAmount = holidayWorkAmount,
                    CommissionAmount = commissionAmount,
                    BhxhAmount = totalInsuranceDeduction,
                    UnionFeeAmount = unionFeeAmount,
                    TotalSalary = (decimal)totalSalary,
                    TotalReceivedSalary = (decimal)totalReceivedSalary,
                    ConfirmationStatus = PayrollConfirmationStatusEmployee.NotSent
                });

            }

            await CreateRangeAsync(payrollDetails);

            _logger.LogInformation(
                "Finished payroll detail calculation. PayrollId={PayrollId}, DetailCount={DetailCount}",
                payrollId,
                payrollDetails.Count);
        }

        private decimal ResolveSalaryComponentWithRule(
            int? organizationId,
            string componentCode,
            decimal defaultValue,
            decimal contractSalaryAmount,
            decimal contractSalaryInsurance,
            decimal receivedSalary,
            int attendanceDays)
        {
            if (!organizationId.HasValue) return defaultValue;

            var component = _dbContext.SalaryComponents
                .FirstOrDefault(c =>
                    c.OrganizationId == organizationId.Value &&
                    c.ComponentCode == componentCode &&
                    c.IsDeleted != true &&
                    c.Status == Status.Tracking);

            if (component == null) return defaultValue;

            switch (component.CalcType)
            {
                case SalaryComponentCalcType.FixedAmount:
                {
                    if (component.FixedAmount.HasValue) return component.FixedAmount.Value;
                    return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
                }
                case SalaryComponentCalcType.PerAttendanceDay:
                {
                    var unit = component.UnitAmount ?? TryParseLegacyValueFormula(component.ValueFormula, 0m);
                    if (unit <= 0m) return defaultValue;
                    return unit * attendanceDays;
                }
                case SalaryComponentCalcType.PercentOfBase:
                {
                    var ratePercent = component.RatePercent ?? TryParseLegacyValueFormula(component.ValueFormula, 0m);
                    if (ratePercent <= 0m) return defaultValue;

                    var baseAmount = component.BaseSource switch
                    {
                        SalaryComponentBaseSource.ContractSalaryInsurance => contractSalaryInsurance,
                        SalaryComponentBaseSource.ReceivedSalary => receivedSalary,
                        _ => contractSalaryAmount
                    };

                    if (component.CapAmount.HasValue && component.CapAmount.Value > 0m)
                    {
                        baseAmount = Math.Min(baseAmount, component.CapAmount.Value);
                    }

                    return baseAmount * (ratePercent / 100m);
                }
                default:
                    return defaultValue;
            }
        }

        private static decimal MapMinutesToOtDecimal(int minutes)
        {
            if (minutes <= 5) return 0.00m;
            if (minutes <= 10) return 0.10m;
            if (minutes <= 15) return 0.25m;
            if (minutes <= 20) return 0.30m;
            if (minutes <= 30) return 0.50m;
            if (minutes <= 45) return 0.75m;
            return 1.00m; // 46-59
        }

        private static decimal RoundOtHours(decimal overtimeHours)
        {
            var rawOtMinutesRounded = (int)Math.Round(overtimeHours * 60m, MidpointRounding.AwayFromZero);
            if (rawOtMinutesRounded < 0) rawOtMinutesRounded = 0;

            var wholeHours = rawOtMinutesRounded / 60;
            var remainingMinutes = rawOtMinutesRounded % 60;
            var decimalPart = MapMinutesToOtDecimal(remainingMinutes);

            return wholeHours + decimalPart;
        }

        private static (decimal StandardHours, decimal RegularMultiplier, decimal HolidayMultiplier) GetTimesheetShiftConfig(
            Timesheet timesheet,
            IReadOnlyDictionary<int, (decimal StandardHours, decimal RegularMultiplier, decimal HolidayMultiplier)> shiftSalaryConfigById)
        {
            if (timesheet.ShiftWorkId.HasValue
                && shiftSalaryConfigById.TryGetValue(timesheet.ShiftWorkId.Value, out var shiftConfig)
                && shiftConfig.StandardHours > 0m)
            {
                return shiftConfig;
            }

            // Fallback to 8h for old/missing shift data.
            return (8m, 1m, 2m);
        }

        private static decimal ResolveShiftStandardHours(ShiftCatalog? shiftCatalog)
        {
            if (shiftCatalog?.WorkingHours > 0)
            {
                return (decimal)shiftCatalog.WorkingHours.Value;
            }

            if (shiftCatalog?.StartTime.HasValue == true && shiftCatalog.EndTime.HasValue)
            {
                var startTime = shiftCatalog.StartTime.Value;
                var endTime = shiftCatalog.EndTime.Value;
                var duration = endTime >= startTime
                    ? endTime - startTime
                    : (TimeSpan.FromDays(1) - startTime) + endTime;

                if (duration.TotalHours > 0)
                {
                    return (decimal)duration.TotalHours;
                }
            }

            return 8m;
        }

        private decimal GetSalaryComponentAmount(int? organizationId, string componentCode, decimal defaultValue)
        {
            if (!organizationId.HasValue)
            {
                return defaultValue;
            }

            var component = _dbContext.SalaryComponents
                .FirstOrDefault(c =>
                    c.OrganizationId == organizationId.Value &&
                    c.ComponentCode == componentCode &&
                    c.IsDeleted != true &&
                    c.Status == Status.Tracking);

            if (component == null)
            {
                return defaultValue;
            }

            // Backward-compatible: ưu tiên field mới, fallback về ValueFormula
            return ResolveComponentValue(component, defaultValue);
        }

        private decimal ResolveComponentValue(SalaryComponent component, decimal defaultValue)
        {
            // 1) Field mới (FixedAmount/UnitAmount/RatePercent) nếu có
            if (component.CalcType == SalaryComponentCalcType.FixedAmount)
            {
                if (component.FixedAmount.HasValue) return component.FixedAmount.Value;
                return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
            }

            // Các CalcType còn lại cần context -> nếu gọi nhầm thì fallback về default/legacy
            return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
        }

        private static decimal TryParseLegacyValueFormula(string? valueFormula, decimal defaultValue)
        {
            if (string.IsNullOrWhiteSpace(valueFormula)) return defaultValue;

            // Ưu tiên parse theo invariant, fallback sang vi-VN
            if (decimal.TryParse(valueFormula, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            var viCulture = CultureInfo.GetCultureInfo("vi-VN");
            if (decimal.TryParse(valueFormula, NumberStyles.Any, viCulture, out value))
            {
                return value;
            }

            return defaultValue;
        }

        public async Task RecalculateAndSavePayrollDetails(int payrollId)
        {
            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
            }

            // Nếu đã khóa thì không cho cập nhật (tính lại) phiếu lương
            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể cập nhật phiếu lương.");
            }

            // Soft delete các bản ghi cũ để tránh bị trùng dữ liệu
            var existingDetails = await _dbContext.PayrollDetails
                .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                .ToListAsync();

            if (existingDetails.Any())
            {
                foreach (var item in existingDetails)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.Now;
                }
                await UpdateRangeAsync(existingDetails);
            }

            // Tính lại và lưu mới
            await CalculateAndSavePayrollDetails(payrollId);

            // Cập nhật trạng thái xác nhận của bảng lương tổng (reset về NotSent vì tất cả PayrollDetail mới tạo đều có status NotSent)
            await UpdatePayrollConfirmationStatus(payrollId);

            // Khi quản lý cập nhật lại phiếu lương, các thắc mắc đang chờ xử lý trong kỳ lương này được chuyển sang đã xử lý.
            await ResolvePendingPayrollInquiries(payrollId: payrollId);
        }

        public async Task Update(int id, UpdatePayrollDetailRequest request)
        {
            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {id}");
            }

            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollDetail.PayrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollDetail.PayrollId}");
            }

            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể sửa phiếu lương.");
            }

            var shouldRecalcReceivedSalary = false;
            var shouldRecalcKpiSalary = false;
            var shouldRecalcTotalSalary = false;

            if (request.BaseSalary.HasValue)
            {
                payrollDetail.BaseSalary = request.BaseSalary.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.StandardWorkDays.HasValue)
            {
                payrollDetail.StandardWorkDays = request.StandardWorkDays.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.ActualWorkDays.HasValue)
            {
                payrollDetail.ActualWorkDays = request.ActualWorkDays.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.KPI.HasValue)
            {
                payrollDetail.KPI = request.KPI.Value;
                shouldRecalcKpiSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.KpiPercentage.HasValue)
            {
                payrollDetail.KpiPercentage = request.KpiPercentage.Value;
                shouldRecalcKpiSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.Bonus.HasValue)
            {
                payrollDetail.Bonus = request.Bonus.Value;
                shouldRecalcTotalSalary = true;
            }

            if (request.SalaryRate.HasValue)
            {
                payrollDetail.SalaryRate = request.SalaryRate.Value;
            }

            if (shouldRecalcReceivedSalary)
            {
                var baseSalary = payrollDetail.BaseSalary ?? 0;
                var standardWorkDays = payrollDetail.StandardWorkDays ?? 0;
                var actualWorkDays = payrollDetail.ActualWorkDays ?? 0;
                var salaryRate = payrollDetail.SalaryRate ?? 100m;

                payrollDetail.ReceivedSalary = standardWorkDays > 0
                    ? (baseSalary / standardWorkDays) * (decimal)actualWorkDays * (salaryRate / 100m)
                    : 0;
            }

            if (shouldRecalcKpiSalary)
            {
                var kpi = payrollDetail.KPI ?? 0;
                var kpiPercentage = payrollDetail.KpiPercentage ?? 0;
                payrollDetail.KpiSalary = kpi * (kpiPercentage / 100);
            }

            if (shouldRecalcTotalSalary)
            {
                var receivedSalary = payrollDetail.ReceivedSalary ?? 0;
                var kpiSalary = payrollDetail.KpiSalary ?? 0;
                var bonus = payrollDetail.Bonus ?? 0;

                var allowanceMealTravel = payrollDetail.AllowanceMealTravel ?? 0m;
                var parkingAmount = payrollDetail.ParkingAmount ?? 0m;
                var overtimeAmount = payrollDetail.OvertimeAmount ?? 0m;
                var holidayWorkAmount = payrollDetail.HolidayWorkAmount ?? 0m;
                var commissionAmount = payrollDetail.CommissionAmount ?? 0m;
                var totalInsuranceDeduction = payrollDetail.BhxhAmount ?? 0m;
                var unionFeeAmount = payrollDetail.UnionFeeAmount ?? 0m;

                // Công thức: Lương cứng + Phụ cấp + Gửi xe + Hoa hồng + OT + Phụ trội ngày lễ - (BHXH+BHTN+BHYT) - Quỹ công đoàn
                var coreSalary = receivedSalary + kpiSalary + bonus;
                payrollDetail.TotalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount + holidayWorkAmount + commissionAmount;

                var totalDeductions = 0m;
                if (payrollDetail.EmployeeId.HasValue)
                {
                    totalDeductions = _dbContext.Deductions
                        .Where(d => d.EmployeeId == payrollDetail.EmployeeId.Value)
                        .Sum(d => d.Value) ?? 0;
                }
                payrollDetail.TotalReceivedSalary = payrollDetail.TotalSalary - (totalInsuranceDeduction + unionFeeAmount + totalDeductions);
            }

            await UpdateAsync(payrollDetail);

            // Khi quản lý sửa phiếu lương của nhân viên, các thắc mắc pending của phiếu đó được đánh dấu đã xử lý.
            await ResolvePendingPayrollInquiries(payrollDetailId: payrollDetail.Id);
        }

        private async Task ResolvePendingPayrollInquiries(int? payrollId = null, int? payrollDetailId = null)
        {
            if (!payrollId.HasValue && !payrollDetailId.HasValue)
            {
                return;
            }

            var query = _dbContext.PayrollInquiries
                .Where(i => i.IsDeleted != true && i.Status == InquiryStatus.Pending)
                .AsQueryable();

            if (payrollDetailId.HasValue)
            {
                query = query.Where(i => i.PayrollDetailId == payrollDetailId.Value);
            }
            else if (payrollId.HasValue)
            {
                var payrollDetailIdsInPayroll = _dbContext.PayrollDetails
                    .Where(pd => pd.PayrollId == payrollId.Value)
                    .Select(pd => pd.Id);

                query = query.Where(i =>
                    i.PayrollDetailId.HasValue &&
                    payrollDetailIdsInPayroll.Contains(i.PayrollDetailId.Value));
            }

            await query.ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.Status, InquiryStatus.Resolved)
                .SetProperty(i => i.UpdatedAt, DateTime.Now));
        }

        private decimal ResolveRevenueCommissionAmount(
            int? organizationId,
            string? staffPositionCode,
            decimal revenue,
            DateTime payrollDate)
        {
            if (!organizationId.HasValue) return 0m;
            if (revenue <= 0m) return 0m;
            if (string.IsNullOrWhiteSpace(staffPositionCode)) return 0m;

            var code = staffPositionCode.Trim().ToUpperInvariant();
            RevenueCommissionTargetType? targetType = null;

            if (code == "CTV")
            {
                targetType = RevenueCommissionTargetType.Ctv;
            }
            else if (code.StartsWith("SALE"))
            {
                targetType = RevenueCommissionTargetType.Sale;
            }

            if (!targetType.HasValue) return 0m;

            var policy = _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true
                            && p.Status == Status.Tracking
                            && p.OrganizationId == organizationId.Value
                            && p.TargetType == targetType.Value
                            && (!p.EffectiveFrom.HasValue || p.EffectiveFrom.Value.Date <= payrollDate.Date)
                            && (!p.EffectiveTo.HasValue || p.EffectiveTo.Value.Date >= payrollDate.Date))
                .Include(p => p.Tiers)
                .OrderByDescending(p => p.EffectiveFrom ?? DateTime.MinValue)
                .ThenByDescending(p => p.Id)
                .FirstOrDefault();

            if (policy == null) return 0m;
            if (policy.Tiers == null || policy.Tiers.Count == 0) return 0m;

            return CalculateProgressiveCommission(revenue, policy.Tiers);
        }

        private static decimal CalculateProgressiveCommission(decimal revenue, IEnumerable<RevenueCommissionTier> tiers)
        {
            if (revenue <= 0m) return 0m;

            var ordered = tiers
                .Where(t => t.IsDeleted != true)
                .OrderBy(t => t.FromAmount)
                .ThenBy(t => t.ToAmount.HasValue ? 0 : 1)
                .ThenBy(t => t.SortOrder)
                .ToList();

            decimal total = 0m;

            foreach (var tier in ordered)
            {
                var from = tier.FromAmount;
                var to = tier.ToAmount;

                if (revenue <= from) continue;

                var upper = to.HasValue && to.Value > from ? to.Value : revenue;
                var applicable = Math.Min(revenue, upper) - from;
                if (applicable <= 0m) continue;

                var rate = tier.RatePercent;
                if (rate <= 0m) continue;

                total += applicable * (rate / 100m);
            }

            return total;
        }

        public async Task Delete(int id)
        {
            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {id}");
            }

            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollDetail.PayrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollDetail.PayrollId}");
            }

            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể xóa phiếu lương.");
            }

            var payrollId = payrollDetail.PayrollId;
            payrollDetail.IsDeleted = true;
            await UpdateAsync(payrollDetail);

            // Cập nhật trạng thái xác nhận của bảng lương tổng
            await UpdatePayrollConfirmationStatus(payrollId);
        }

        public async Task DeleteRange(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new Exception("Danh sách Id không được để trống.");
            }

            var payrollDetails = await _dbContext.PayrollDetails
                .Where(p => ids.Contains(p.Id) && p.IsDeleted != true)
                .ToListAsync();

            if (payrollDetails.Count == 0)
            {
                return;
            }

            // Nếu bất kỳ phiếu lương nào thuộc bảng lương đã khóa thì chặn
            var payrollIds = payrollDetails.Where(x => x.PayrollId.HasValue).Select(x => x.PayrollId!.Value).Distinct().ToList();
            var lockedPayrollExists = await _dbContext.Payrolls.AnyAsync(p => payrollIds.Contains(p.Id) && p.IsDeleted != true && p.PayrollStatus == PayrollStatus.Locked);
            if (lockedPayrollExists)
            {
                throw new Exception("Có bảng lương đã khóa, không thể xóa phiếu lương.");
            }

            foreach (var item in payrollDetails)
            {
                item.IsDeleted = true;
            }
            await UpdateRangeAsync(payrollDetails);

            // Cập nhật trạng thái xác nhận cho tất cả các bảng lương bị ảnh hưởng
            foreach (var payrollId in payrollIds)
            {
                await UpdatePayrollConfirmationStatus(payrollId);
            }
        }

        public async Task<List<PayrollDetailDto>> FetchPayrollDetails(int payrollId)
        {
            try
            {
                var payroll = await _dbContext.Payrolls
                    .FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);

                if (payroll == null)
                {
                    throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
                }

                // 1. Kiểm tra nếu bảng lương chi tiết đã tồn tại
                var existingDetails = await _dbContext.PayrollDetails
                    .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                    .ToListAsync();

                if (existingDetails.Any())
                {
                    return _mapper.Map<List<PayrollDetailDto>>(existingDetails);
                }

                // 2. Nếu chưa tồn tại, tính toán và lưu bảng lương chi tiết
                await CalculateAndSavePayrollDetails(payrollId);

                // 3. Lấy lại danh sách bảng lương chi tiết vừa tạo
                var newDetails = await _dbContext.PayrollDetails
                    .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                    .ToListAsync();

                return _mapper.Map<List<PayrollDetailDto>>(newDetails);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // Quản lý gửi bảng lương cho nhân viên xem
        public async Task SendPayrollDetailConfirmation(UpdateSendPayrollDetailConfirmationRequest request)
        {
            if (request.PayrollDetailIds == null || !request.PayrollDetailIds.Any())
            {
                throw new Exception($"Chọn ít nhất 1 bảng lương chi tiết để gửi");
            }

            var payrollDetailIds = request.PayrollDetailIds.Distinct().ToList();

            var payrollIds = await _dbContext.PayrollDetails
                .Where(p => payrollDetailIds.Contains(p.Id) && p.IsDeleted != true)
                .Select(p => p.PayrollId)
                .Distinct()
                .ToListAsync();

            var updatedRows = await _dbContext.PayrollDetails
                .Where(p => payrollDetailIds.Contains(p.Id) && p.IsDeleted != true)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.ConfirmationStatus, PayrollConfirmationStatusEmployee.Confirming)
                    .SetProperty(p => p.ResponseDeadline, request.ResponseDeadline));

            if (updatedRows == 0)
            {
                throw new Exception("Không tìm thấy bảng lương chi tiết");
            }

            // Cập nhật trạng thái xác nhận của các bảng lương tổng bị ảnh hưởng
            foreach (var payrollId in payrollIds.Where(id => id.HasValue).Select(id => id!.Value))
            {
                await UpdatePayrollConfirmationStatus(payrollId);
            }
        }

        // Nhân viên xác nhận bảng lương
        public async Task ConfirmPayrollDetailByEmployee(int payrollDetailId)
        {
            // Kiểm tra nếu ID âm (trường hợp gửi sai format)
            if (payrollDetailId < 0)
            {
                throw new ArgumentException("PayrollDetailId không hợp lệ.");
            }

            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == payrollDetailId && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Không tìm thấy bảng lương chi tiết với Id = {payrollDetailId}");
            }

            payrollDetail.ConfirmationStatus = PayrollConfirmationStatusEmployee.Confirmed;
            payrollDetail.ConfirmationDate = DateTime.Now;

            await UpdateAsync(payrollDetail);

            // Kiểm tra và cập nhật trạng thái xác nhận của bảng lương tổng
            await UpdatePayrollConfirmationStatus(payrollDetail.PayrollId);
        }

        public async Task<List<PayrollDetailEmailSendDto>> GetPayrollDetailEmailSendData(List<int> payrollDetailIds)
        {
            if (payrollDetailIds == null || !payrollDetailIds.Any())
            {
                return new List<PayrollDetailEmailSendDto>();
            }

            return await _dbContext.PayrollDetails
                .Where(p => payrollDetailIds.Contains(p.Id) && p.IsDeleted != true)
                .Select(p => new PayrollDetailEmailSendDto
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    EmployeeId = p.EmployeeId,
                    EmployeeCode = p.EmployeeCode,
                    FullName = p.FullName,
                    Email = p.Employee != null ? p.Employee.PersonalEmail : null,
                    BaseSalary = p.BaseSalary,
                    StandardWorkDays = p.StandardWorkDays,
                    ActualWorkDays = p.ActualWorkDays,
                    ReceivedSalary = p.ReceivedSalary,
                    KpiSalary = p.KpiSalary,
                    Bonus = p.Bonus,
                    TotalSalary = p.TotalSalary,
                    TotalReceivedSalary = p.TotalReceivedSalary,
                    ResponseDeadline = p.ResponseDeadline
                })
                .ToListAsync();
        }

        /// <summary>
        /// Cập nhật trạng thái xác nhận của bảng lương tổng dựa trên trạng thái của các nhân viên thuộc phạm vi được tính payroll
        /// </summary>
        private async Task UpdatePayrollConfirmationStatus(int? payrollId)
        {
            if (!payrollId.HasValue) return;

            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollId.Value && p.IsDeleted != true);

            if (payroll == null) return;

            // Lấy danh sách SummaryTimesheetNameId liên kết với Payroll này
            var summaryTimesheetNameIds = await _dbContext.PayrollSummaryTimesheets
                .Where(pst => pst.PayrollId == payrollId)
                .Select(pst => pst.SummaryTimesheetNameId)
                .ToListAsync();

            // Xác định khoảng thời gian kỳ lương dựa trên các bảng công tổng hợp
            DateTime periodStartDate;
            DateTime periodEndDate;

            if (summaryTimesheetNameIds.Any())
            {
                var period = _dbContext.SummaryTimesheetNames
                    .Where(s => summaryTimesheetNameIds.Contains(s.Id))
                    .Select(s => new
                    {
                        MinStartDate = s.SummaryTimesheetNameDetailTimesheetNames
                            .Min(d => d.DetailTimesheetName.StartDate),
                        MaxEndDate = s.SummaryTimesheetNameDetailTimesheetNames
                            .Max(d => d.DetailTimesheetName.EndDate)
                    })
                    .FirstOrDefault();

                var fallback = payroll.CreatedAt ?? DateTime.Now;
                periodStartDate = (period?.MinStartDate ?? fallback).Date;
                periodEndDate = (period?.MaxEndDate ?? fallback).Date;
            }
            else
            {
                var payrollMonth = payroll.CreatedAt ?? DateTime.Now;
                periodStartDate = new DateTime(payrollMonth.Year, payrollMonth.Month, 1);
                periodEndDate = periodStartDate.AddMonths(1).AddDays(-1);
            }

            var now = DateTime.Now;

            // Lấy các PayrollDetail của bảng lương này, giới hạn đúng tập nhân viên giống logic tạo payroll details:
            // - Có hợp đồng còn hiệu lực trong thời gian của bảng tổng hợp (EffectiveDate <= periodEndDate và (ExpiryDate >= periodStartDate hoặc ExpiryDate == null))
            // - Hợp đồng chưa hết hạn (ExpiredStatus == false)
            // - Đã confirm bảng chấm công tổng hợp (Status == Confirm) hoặc đã quá ngày confirm (Date < now)
            var allPayrollDetails = await _dbContext.PayrollDetails
                .Where(pd => pd.PayrollId == payrollId.Value && pd.IsDeleted != true)
                .Where(pd => pd.Employee.Contracts.Any(c => 
                    c.ExpiredStatus == false &&
                    c.EffectiveDate.HasValue && c.EffectiveDate.Value <= periodEndDate &&
                    (!c.ExpiryDate.HasValue || c.ExpiryDate.Value >= periodStartDate)))
                .Where(pd => pd.Employee.SummaryTimesheetNameEmployeeConfirms.Any(s =>
                    summaryTimesheetNameIds.Contains(s.SummaryTimesheetNameId) &&
                    (s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Confirm ||
                     (s.Date != null && s.Date < now))))
                .Select(pd => pd.ConfirmationStatus)
                .ToListAsync();

            if (!allPayrollDetails.Any())
            {
                payroll.PayrollConfirmationStatus = PayrollConfirmationStatus.NotSent;
            }
            else if (allPayrollDetails.All(status => status == PayrollConfirmationStatusEmployee.Confirmed))
            {
                // Tất cả đều đã xác nhận
                payroll.PayrollConfirmationStatus = PayrollConfirmationStatus.Confirmed;
            }
            else if (allPayrollDetails.Any(status => status == PayrollConfirmationStatusEmployee.Confirming))
            {
                // Có ít nhất 1 employee đang trong trạng thái Confirming
                payroll.PayrollConfirmationStatus = PayrollConfirmationStatus.Confirming;
            }
            else if (allPayrollDetails.All(status => status == PayrollConfirmationStatusEmployee.NotSent))
            {
                // Tất cả đều chưa gửi
                payroll.PayrollConfirmationStatus = PayrollConfirmationStatus.NotSent;
            }
            else
            {
                // Các trường hợp khác (có thể có người Rejected)
                payroll.PayrollConfirmationStatus = PayrollConfirmationStatus.NotConfirmed;
            }

            _dbContext.Payrolls.Update(payroll);
            await _dbContext.SaveChangesAsync();
        }

    }
}
