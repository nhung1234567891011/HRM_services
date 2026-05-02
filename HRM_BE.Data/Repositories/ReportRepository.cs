using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Helpers;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Report;
using Microsoft.EntityFrameworkCore;

namespace HRM_BE.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly HrmContext _dbContext;

        public ReportRepository(HrmContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Phiếu lương chỉ tính báo cáo khi thuộc bảng lương cha chưa xóa (xóa Payroll không cascade trước đây).
        /// </summary>
        private IQueryable<PayrollDetail> PayrollDetailsForReports()
        {
            return _dbContext.PayrollDetails
                .Where(pd => pd.IsDeleted == false &&
                       (pd.Employee == null || pd.Employee.IsDeleted == false) &&
                       pd.Payroll != null &&
                       pd.Payroll.IsDeleted != true)
                .AsNoTracking();
        }

        /// <summary>
        /// Tháng/năm hạch toán lương: theo ngày bắt đầu nhỏ nhất của các bảng chi tiết gắn bảng tổng hợp (cùng quy tắc CalculateAndSavePayrollDetails).
        /// Không dùng CreatedAt của phiếu để không lệch tháng khi tạo phiếu trễ.
        /// </summary>
        private async Task<Dictionary<int, (int Year, int Month)>> BuildPayrollAccountingYearMonthLookupAsync(
            ICollection<int> payrollIds)
        {
            var result = new Dictionary<int, (int Year, int Month)>();
            if (payrollIds == null || payrollIds.Count == 0)
                return result;

            var idSet = payrollIds.Distinct().ToHashSet();

            var accountingStarts = await _dbContext.PayrollSummaryTimesheets
                .AsNoTracking()
                .Where(pst => pst.PayrollId != null && idSet.Contains(pst.PayrollId.Value))
                .Where(pst => pst.SummaryTimesheetName != null)
                .SelectMany(
                    pst => pst.SummaryTimesheetName!.SummaryTimesheetNameDetailTimesheetNames
                        .Where(l => l.DetailTimesheetName != null && l.DetailTimesheetName.StartDate != null),
                    (pst, link) => new
                    {
                        PayrollId = pst.PayrollId!.Value,
                        StartDate = link.DetailTimesheetName!.StartDate!.Value
                    })
                .ToListAsync();

            foreach (var grp in accountingStarts.GroupBy(x => x.PayrollId))
            {
                var minDate = grp.Min(x => x.StartDate.Date);
                result[grp.Key] = (minDate.Year, minDate.Month);
            }

            var missingIds = idSet.Where(id => !result.ContainsKey(id)).ToList();
            if (missingIds.Count > 0)
            {
                var fallbacks = await _dbContext.Payrolls.AsNoTracking()
                    .Where(p => missingIds.Contains(p.Id))
                    .Select(p => new { p.Id, p.CreatedAt })
                    .ToListAsync();

                foreach (var p in fallbacks)
                {
                    var anchor = (p.CreatedAt ?? DateTimeHelper.BusinessNow).Date;
                    result[p.Id] = (anchor.Year, anchor.Month);
                }
            }

            foreach (var id in idSet.Where(id => !result.ContainsKey(id)))
            {
                var now = DateTimeHelper.BusinessNow.Date;
                result[id] = (now.Year, now.Month);
            }

            return result;
        }

        public async Task<HrDistributionReportDto> GetHrDistributionReport(int? organizationId)
        {
            var query = _dbContext.Employees
                .Where(e => e.IsDeleted == false)
                .AsNoTracking();

            if (organizationId.HasValue)
                query = query.Where(e => e.OrganizationId == organizationId.Value);

            var employees = await query
                .Select(e => new
                {
                    e.Id,
                    e.OrganizationId,
                    OrganizationName = e.Organization != null ? e.Organization.OrganizationName : "Chưa phân bổ",
                    e.StaffPositionId,
                    PositionName = e.StaffPosition != null ? e.StaffPosition.PositionName : "Chưa có vị trí",
                    e.WorkingStatus
                })
                .ToListAsync();

            var totalEmployees = employees.Count;
            var activeEmployees = employees.Where(e => e.WorkingStatus == WorkingStatus.Active).ToList();
            var totalActive = activeEmployees.Count;

            var departmentDistributions = activeEmployees
                .GroupBy(e => new { e.OrganizationId, e.OrganizationName })
                .Select(g => new DepartmentDistribution
                {
                    OrganizationId = g.Key.OrganizationId ?? 0,
                    OrganizationName = g.Key.OrganizationName ?? "Chưa phân bổ",
                    EmployeeCount = g.Count(),
                    Percentage = totalActive > 0 ? Math.Round((double)g.Count() / totalActive * 100, 2) : 0
                })
                .OrderByDescending(d => d.EmployeeCount)
                .ToList();

            // Thống kê theo vị trí: join StaffPositions (StaffPosition.Id == Employee.StaffPositionId)
            // rồi count theo PositionName như yêu cầu
            var positionCounts = await (
                    from e in _dbContext.Employees.AsNoTracking()
                    where e.IsDeleted == false
                          && e.WorkingStatus == WorkingStatus.Active
                          && (!organizationId.HasValue || e.OrganizationId == organizationId.Value)
                    join sp in _dbContext.StaffPositions.AsNoTracking().Where(x => x.IsDeleted == false)
                        on e.StaffPositionId equals sp.Id into spg
                    from sp in spg.DefaultIfEmpty()
                    let positionName = sp != null && sp.PositionName != null && sp.PositionName != ""
                        ? sp.PositionName
                        : "Chưa có vị trí"
                    group e by positionName
                    into g
                    select new
                    {
                        PositionName = g.Key,
                        Value = g.Count()
                    }
                )
                .OrderByDescending(x => x.Value)
                .ToListAsync();

            var positionDistributions = positionCounts
                .Select(x => new PositionDistribution
                {
                    StaffPositionId = 0,
                    PositionName = x.PositionName ?? "Chưa có vị trí",
                    EmployeeCount = x.Value,
                    Percentage = totalActive > 0 ? Math.Round(x.Value / (double)totalActive * 100, 2) : 0
                })
                .ToList();

            var statusDistributions = employees
                .GroupBy(e => e.WorkingStatus)
                .Select(g => new StatusDistribution
                {
                    Status = g.Key == WorkingStatus.Active ? "Đang làm việc" : "Nghỉ việc",
                    Count = g.Count()
                })
                .ToList();

            return new HrDistributionReportDto
            {
                TotalEmployees = totalEmployees,
                TotalActiveEmployees = totalActive,
                DepartmentDistributions = departmentDistributions,
                PositionDistributions = positionDistributions,
                StatusDistributions = statusDistributions
            };
        }

        public async Task<MonthlyIncomeReportDto> GetMonthlyIncomeReport(int? organizationId, int year)
        {
            var query = PayrollDetailsForReports();

            if (organizationId.HasValue)
                query = query.Where(pd => pd.OrganizationId == organizationId.Value);

            query = query.Where(pd => pd.PayrollId != null);

            var payrollDetailsRaw = await query
                .Select(pd => new
                {
                    pd.Id,
                    pd.PayrollId,
                    StaffPositionId = pd.Employee != null ? pd.Employee.StaffPositionId : null,
                    PositionName = pd.Employee != null && pd.Employee.StaffPosition != null 
                        ? pd.Employee.StaffPosition.PositionName : "Chưa có vị trí",
                    // "Thực nhận" theo PayrollDetail đã tính từ chấm công (ReceivedSalary/KpiSalary)
                    BaseSalary = pd.ReceivedSalary,
                    Bonus = pd.KpiSalary,
                    pd.AllowanceMealTravel,
                    pd.ParkingAmount,
                    pd.CommissionAmount,
                    pd.OvertimeAmount,
                    pd.BhxhAmount,
                    pd.UnionFeeAmount,
                    pd.TotalReceivedSalary,
                    pd.EmployeeId
                })
                .ToListAsync();

            var periodLookup =
                await BuildPayrollAccountingYearMonthLookupAsync(
                    payrollDetailsRaw.Select(pd => pd.PayrollId!.Value).Distinct().ToList());

            var payrollDetails = payrollDetailsRaw
                .Where(pd =>
                {
                    if (!periodLookup.TryGetValue(pd.PayrollId!.Value, out var acc))
                        return false;
                    return acc.Year == year;
                })
                .Select(pd =>
                {
                    var acc = periodLookup[pd.PayrollId!.Value];
                    return new
                    {
                        pd.Id,
                        pd.StaffPositionId,
                        pd.PositionName,
                        pd.BaseSalary,
                        pd.Bonus,
                        pd.AllowanceMealTravel,
                        pd.ParkingAmount,
                        pd.CommissionAmount,
                        pd.OvertimeAmount,
                        pd.BhxhAmount,
                        pd.UnionFeeAmount,
                        pd.TotalReceivedSalary,
                        pd.EmployeeId,
                        AccountingMonth = acc.Month,
                        AccountingYear = acc.Year
                    };
                })
                .ToList();

            var monthlySummaries = payrollDetails
                .GroupBy(pd => new { pd.AccountingMonth, pd.AccountingYear })
                .Select(g => new MonthlyIncomeSummary
                {
                    Month = g.Key.AccountingMonth,
                    Year = g.Key.AccountingYear,
                    TotalBaseSalary = g.Sum(x => x.BaseSalary ?? 0),
                    TotalAllowance = g.Sum(x => (x.AllowanceMealTravel ?? 0) + (x.ParkingAmount ?? 0) + (x.CommissionAmount ?? 0)),
                    TotalBonus = g.Sum(x => x.Bonus ?? 0),
                    TotalOvertimePay = g.Sum(x => x.OvertimeAmount ?? 0),
                    TotalDeductions = g.Sum(x => (x.BhxhAmount ?? 0) + (x.UnionFeeAmount ?? 0)),
                    TotalNetSalary = g.Sum(x => x.TotalReceivedSalary ?? 0),
                    EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count()
                })
                .OrderBy(m => m.Month)
                .ToList();

            var positionIncomes = payrollDetails
                .GroupBy(pd => new { pd.StaffPositionId, pd.PositionName })
                .Select(g =>
                {
                    var empCount = g.Select(x => x.EmployeeId).Distinct().Count();
                    var totalSalary = g.Sum(x => x.TotalReceivedSalary ?? 0);
                    return new PositionIncome
                    {
                        StaffPositionId = g.Key.StaffPositionId ?? 0,
                        PositionName = g.Key.PositionName ?? "Chưa có vị trí",
                        TotalBaseSalary = g.Sum(x => x.BaseSalary ?? 0),
                        TotalAllowance = g.Sum(x => (x.AllowanceMealTravel ?? 0) + (x.ParkingAmount ?? 0) + (x.CommissionAmount ?? 0)),
                        TotalBonus = g.Sum(x => x.Bonus ?? 0),
                        TotalOvertimePay = g.Sum(x => x.OvertimeAmount ?? 0),
                        TotalSalary = totalSalary,
                        AverageSalary = empCount > 0 ? Math.Round(totalSalary / empCount, 0) : 0,
                        EmployeeCount = empCount
                    };
                })
                .OrderByDescending(d => d.TotalSalary)
                .ToList();

            return new MonthlyIncomeReportDto
            {
                MonthlySummaries = monthlySummaries,
                PositionIncomes = positionIncomes
            };
        }

        public async Task<PerformanceReportDto> GetPerformanceReport(
            int? organizationId,
            int? year,
            int? month,
            int? fromYear,
            int? fromMonth,
            int? toYear,
            int? toMonth)
        {
            var payrollQuery = PayrollDetailsForReports()
                .Where(pd => pd.PayrollId != null);

            if (organizationId.HasValue)
                payrollQuery = payrollQuery.Where(pd => pd.OrganizationId == organizationId.Value);

            var hasValidRange =
                fromYear.HasValue &&
                fromMonth.HasValue &&
                toYear.HasValue &&
                toMonth.HasValue &&
                fromMonth.Value >= 1 && fromMonth.Value <= 12 &&
                toMonth.Value >= 1 && toMonth.Value <= 12;

            DateTime? rangeStartInclusive = null;
            DateTime? rangeEndExclusive = null;

            if (hasValidRange)
            {
                var fromDate = new DateTime(fromYear!.Value, fromMonth!.Value, 1);
                var toDate = new DateTime(toYear!.Value, toMonth!.Value, 1);

                rangeStartInclusive = fromDate <= toDate ? fromDate : toDate;
                rangeEndExclusive = (fromDate <= toDate ? toDate : fromDate).AddMonths(1);
            }

            var payrollDataRaw = await payrollQuery
                .Select(pd => new
                {
                    pd.PayrollId,
                    pd.EmployeeId,
                    pd.FullName,
                    StaffPositionId = pd.Employee != null ? pd.Employee.StaffPositionId : null,
                    PositionName = pd.Employee != null && pd.Employee.StaffPosition != null
                        ? pd.Employee.StaffPosition.PositionName : "Chưa có vị trí",
                    CommissionAmount = pd.CommissionAmount ?? 0m
                })
                .ToListAsync();

            var periodLookup =
                await BuildPayrollAccountingYearMonthLookupAsync(
                    payrollDataRaw.Select(pd => pd.PayrollId!.Value).Distinct().ToList());

            bool AccountingInRequestedWindow((int yy, int mm) acc)
            {
                if (rangeStartInclusive.HasValue && rangeEndExclusive.HasValue)
                {
                    var accPeriodStart = new DateTime(acc.yy, acc.mm, 1);
                    return accPeriodStart >= rangeStartInclusive.Value &&
                           accPeriodStart < rangeEndExclusive.Value;
                }

                if (!year.HasValue && !month.HasValue)
                    return true;

                if (year.HasValue && acc.yy != year.Value)
                    return false;
                return !month.HasValue || acc.mm == month.Value;
            }

            var payrollData = payrollDataRaw
                .Where(pd =>
                    periodLookup.TryGetValue(pd.PayrollId!.Value, out var acc) &&
                    AccountingInRequestedWindow(acc))
                .Select(pd => new
                {
                    pd.EmployeeId,
                    pd.FullName,
                    pd.StaffPositionId,
                    pd.PositionName,
                    pd.CommissionAmount
                })
                .ToList();

            // Top 10 nhân viên có hoa hồng cao
            var employeePerformances = payrollData
                .GroupBy(pd => pd.EmployeeId)
                .Select(g =>
                {
                    var first = g.First();
                    var totalCommission = g.Sum(x => x.CommissionAmount);
                    return new EmployeePerformance
                    {
                        EmployeeId = first.EmployeeId ?? 0,
                        FullName = first.FullName ?? "",
                        Position = first.PositionName ?? "Chưa có vị trí",
                        // Dùng KpiScore để chứa "lương hoa hồng" (CommissionAmount)
                        KpiScore = totalCommission,
                        KpiPercentage = 0,
                        ActualWorkDays = 0,
                        StandardWorkDays = 0,
                        WorkEfficiency = 0
                    };
                })
                .OrderByDescending(e => e.KpiScore)
                .Take(10)
                .ToList();

            // Tổng hoa hồng theo vị trí (dùng cho dashboard)
            var positionPerformances = payrollData
                .GroupBy(pd => new { pd.StaffPositionId, pd.PositionName })
                .Select(g =>
                {
                    var employeeCount = g.Select(x => x.EmployeeId).Distinct().Count();
                    var totalCommission = g.Sum(x => x.CommissionAmount);
                    var avgCommission = employeeCount > 0 ? Math.Round(totalCommission / employeeCount, 2) : 0;

                    return new PositionPerformance
                    {
                        StaffPositionId = g.Key.StaffPositionId ?? 0,
                        PositionName = g.Key.PositionName ?? "Chưa có vị trí",
                        // Dùng AverageKpi để chứa "tổng hoa hồng theo vị trí"
                        AverageKpi = Math.Round(totalCommission, 2),
                        // MedianKpi giữ "hoa hồng trung bình theo nhân viên" (nếu cần)
                        MedianKpi = avgCommission,
                        EmployeeCount = employeeCount,
                        HighPerformers = 0,
                        LowPerformers = 0
                    };
                })
                .OrderByDescending(p => p.AverageKpi)
                .ToList();

            return new PerformanceReportDto
            {
                EmployeePerformances = employeePerformances,
                PositionPerformances = positionPerformances,
                KpiDistributions = new List<KpiDistribution>()
            };
        }

        public async Task<AttendanceReportDto> GetAttendanceReport(int? organizationId, int year, int? month)
        {
            var timesheetQuery = _dbContext.Timesheets
                .Where(t => t.IsDeleted == false && 
                       t.Date.HasValue && 
                       t.Date.Value.Year == year &&
                       (t.Employee == null || t.Employee.IsDeleted == false))
                .AsNoTracking();

            if (organizationId.HasValue)
                timesheetQuery = timesheetQuery.Where(t =>
                    t.Employee != null && t.Employee.OrganizationId == organizationId.Value);

            if (month.HasValue)
                timesheetQuery = timesheetQuery.Where(t => t.Date.Value.Month == month.Value);

            var timesheets = await timesheetQuery
                .Select(t => new
                {
                    t.EmployeeId,
                    FullName = t.Employee != null ? (t.Employee.LastName + " " + t.Employee.FirstName) : "",
                    StaffPositionId = t.Employee != null ? t.Employee.StaffPositionId : null,
                    PositionName = t.Employee != null && t.Employee.StaffPosition != null
                        ? t.Employee.StaffPosition.PositionName : "Chưa có vị trí",
                    Month = t.Date.HasValue ? t.Date.Value.Month : 0,
                    Year = t.Date.HasValue ? t.Date.Value.Year : 0,
                    t.NumberOfWorkingHour,
                    t.LateDuration,
                    t.EarlyLeaveDuration,
                    t.TimeKeepingLeaveStatus
                })
                .ToListAsync();

            var monthlyAttendances = timesheets
                .GroupBy(t => new { t.Month, t.Year })
                .Select(g =>
                {
                    var totalRecords = g.Count();
                    var workDays = g.Sum(x => (x.NumberOfWorkingHour ?? 0) > 0 ? 1.0 : 0);
                    var lateDays = g.Count(x => (x.LateDuration ?? 0) > 0);
                    var earlyLeaveDays = g.Count(x => (x.EarlyLeaveDuration ?? 0) > 0);
                    var leaveDays = g.Count(x => x.TimeKeepingLeaveStatus != Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var absentDays = g.Count(x => (x.NumberOfWorkingHour ?? 0) == 0
                        && x.TimeKeepingLeaveStatus == Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var overtimeHours = g.Sum(x => Math.Max((x.NumberOfWorkingHour ?? 0) - 8.0, 0));

                    return new MonthlyAttendance
                    {
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        TotalWorkDays = workDays,
                        TotalLateDays = lateDays,
                        TotalEarlyLeaveDays = earlyLeaveDays,
                        TotalAbsentDays = absentDays,
                        TotalLeaveDays = leaveDays,
                        TotalOvertimeHours = Math.Round(overtimeHours, 2),
                        AttendanceRate = totalRecords > 0 ? Math.Round(workDays / totalRecords * 100, 2) : 0
                    };
                })
                .OrderBy(m => m.Month)
                .ToList();

            var employeeAttendances = timesheets
                .GroupBy(t => new { t.EmployeeId, t.FullName, t.PositionName })
                .Select(g =>
                {
                    var totalRecords = g.Count();
                    var workDays = g.Sum(x => (x.NumberOfWorkingHour ?? 0) > 0 ? 1.0 : 0);
                    var lateDays = g.Count(x => (x.LateDuration ?? 0) > 0);
                    var earlyLeaveDays = g.Count(x => (x.EarlyLeaveDuration ?? 0) > 0);
                    var leaveDays = g.Count(x => x.TimeKeepingLeaveStatus != Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var absentDays = g.Count(x => (x.NumberOfWorkingHour ?? 0) == 0
                        && x.TimeKeepingLeaveStatus == Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var overtimeHours = g.Sum(x => Math.Max((x.NumberOfWorkingHour ?? 0) - 8.0, 0));
                    var totalLateDur = g.Sum(x => x.LateDuration ?? 0);
                    var totalEarlyDur = g.Sum(x => x.EarlyLeaveDuration ?? 0);

                    return new EmployeeAttendance
                    {
                        EmployeeId = g.Key.EmployeeId ?? 0,
                        FullName = g.Key.FullName ?? "",
                        Position = g.Key.PositionName ?? "Chưa có vị trí",
                        WorkDays = workDays,
                        LateDays = lateDays,
                        EarlyLeaveDays = earlyLeaveDays,
                        AbsentDays = absentDays,
                        LeaveDays = leaveDays,
                        OvertimeHours = Math.Round(overtimeHours, 2),
                        TotalLateDuration = Math.Round(totalLateDur, 2),
                        TotalEarlyLeaveDuration = Math.Round(totalEarlyDur, 2),
                        AttendanceRate = totalRecords > 0 ? Math.Round(workDays / totalRecords * 100, 2) : 0
                    };
                })
                .OrderByDescending(e => e.AttendanceRate)
                .ToList();

            var positionAttendances = timesheets
                .GroupBy(t => new { t.StaffPositionId, t.PositionName })
                .Select(g =>
                {
                    var totalRecords = g.Count();
                    var workDays = g.Sum(x => (x.NumberOfWorkingHour ?? 0) > 0 ? 1.0 : 0);
                    var lateDays = g.Count(x => (x.LateDuration ?? 0) > 0);
                    var earlyLeaveDays = g.Count(x => (x.EarlyLeaveDuration ?? 0) > 0);
                    var absentDays = g.Count(x => (x.NumberOfWorkingHour ?? 0) == 0
                        && x.TimeKeepingLeaveStatus == Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var overtimeHours = g.Sum(x => Math.Max((x.NumberOfWorkingHour ?? 0) - 8.0, 0));

                    return new PositionAttendance
                    {
                        StaffPositionId = g.Key.StaffPositionId ?? 0,
                        PositionName = g.Key.PositionName ?? "Chưa có vị trí",
                        TotalWorkDays = workDays,
                        TotalLateDays = lateDays,
                        TotalEarlyLeaveDays = earlyLeaveDays,
                        TotalAbsentDays = absentDays,
                        TotalOvertimeHours = Math.Round(overtimeHours, 2),
                        AttendanceRate = totalRecords > 0 ? Math.Round(workDays / totalRecords * 100, 2) : 0,
                        EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count()
                    };
                })
                .OrderByDescending(d => d.TotalOvertimeHours)
                .ToList();

            var otPayrollQuery = PayrollDetailsForReports()
                .Where(pd => pd.PayrollId != null);

            if (organizationId.HasValue)
                otPayrollQuery = otPayrollQuery.Where(pd => pd.OrganizationId == organizationId.Value);

            var otPayrollRaw = await otPayrollQuery
                .Select(pd => new { pd.PayrollId, pd.OvertimeAmount })
                .ToListAsync();

            var otPayrollLookup = await BuildPayrollAccountingYearMonthLookupAsync(
                otPayrollRaw.Select(pd => pd.PayrollId!.Value).Distinct().ToList());

            var otPayrollData = otPayrollRaw
                .Where(pd =>
                    otPayrollLookup.TryGetValue(pd.PayrollId!.Value, out var acc) &&
                    acc.Year == year &&
                    (!month.HasValue || acc.Month == month.Value))
                .Select(pd =>
                {
                    var acc = otPayrollLookup[pd.PayrollId!.Value];
                    return new
                    {
                        pd.OvertimeAmount,
                        AccountingMonth = acc.Month,
                        AccountingYear = acc.Year
                    };
                })
                .ToList();

            var overtimeSummaries = otPayrollData
                .GroupBy(pd => new { pd.AccountingMonth, pd.AccountingYear })
                .Select(g =>
                {
                    var monthData = monthlyAttendances.FirstOrDefault(m =>
                        m.Month == g.Key.AccountingMonth && m.Year == g.Key.AccountingYear);
                    return new OvertimeSummary
                    {
                        Month = g.Key.AccountingMonth,
                        Year = g.Key.AccountingYear,
                        TotalOvertimeHours = monthData?.TotalOvertimeHours ?? 0,
                        TotalOvertimePay = g.Sum(x => x.OvertimeAmount ?? 0)
                    };
                })
                .OrderBy(o => o.Month)
                .ToList();

            var leaveQuery = _dbContext.LeaveApplications
                .Where(la => la.IsDeleted == false
                    && la.Status == Core.Data.Official_Form.LeaveApplicationStatus.Approved
                    && la.StartDate.HasValue && la.StartDate.Value.Year == year
                    && (la.Employee == null || la.Employee.IsDeleted == false))
                .AsNoTracking();

            if (organizationId.HasValue)
                leaveQuery = leaveQuery.Where(la => la.OrganizationId == organizationId.Value);

            if (month.HasValue)
                leaveQuery = leaveQuery.Where(la => la.StartDate.Value.Month == month.Value);

            var leaveData = await leaveQuery
                .Select(la => new
                {
                    LeaveTypeName = la.TypeOfLeave != null ? la.TypeOfLeave.Name : "Khác",
                    la.NumberOfDays
                })
                .ToListAsync();

            var leaveTypeDistributions = leaveData
                .GroupBy(la => la.LeaveTypeName)
                .Select(g => new LeaveTypeDistribution
                {
                    LeaveType = g.Key ?? "Khác",
                    Count = g.Count(),
                    TotalDays = g.Sum(x => x.NumberOfDays ?? 0)
                })
                .OrderByDescending(l => l.Count)
                .ToList();

            return new AttendanceReportDto
            {
                MonthlyAttendances = monthlyAttendances,
                EmployeeAttendances = employeeAttendances,
                LeaveTypeDistributions = leaveTypeDistributions,
                PositionAttendances = positionAttendances,
                OvertimeSummaries = overtimeSummaries
            };
        }
    }
}
