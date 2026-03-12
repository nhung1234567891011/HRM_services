using HRM_BE.Core.Data.Staff;
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

            var positionDistributions = activeEmployees
                .GroupBy(e => new { e.StaffPositionId, e.PositionName })
                .Select(g => new PositionDistribution
                {
                    StaffPositionId = g.Key.StaffPositionId ?? 0,
                    PositionName = g.Key.PositionName ?? "Chưa có vị trí",
                    EmployeeCount = g.Count()
                })
                .OrderByDescending(p => p.EmployeeCount)
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
                DepartmentDistributions = departmentDistributions,
                PositionDistributions = positionDistributions,
                StatusDistributions = statusDistributions
            };
        }

        public async Task<MonthlyIncomeReportDto> GetMonthlyIncomeReport(int? organizationId, int year)
        {
            var query = _dbContext.PayrollDetails
                .Where(pd => pd.IsDeleted == false)
                .AsNoTracking();

            if (organizationId.HasValue)
                query = query.Where(pd => pd.OrganizationId == organizationId.Value);

            // Filter by year using CreatedAt
            query = query.Where(pd => pd.CreatedAt.HasValue && pd.CreatedAt.Value.Year == year);

            var payrollDetails = await query
                .Select(pd => new
                {
                    pd.Id,
                    pd.OrganizationId,
                    OrganizationName = pd.Organization != null ? pd.Organization.OrganizationName : "Chưa phân bổ",
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
                    CreatedMonth = pd.CreatedAt.HasValue ? pd.CreatedAt.Value.Month : 0,
                    CreatedYear = pd.CreatedAt.HasValue ? pd.CreatedAt.Value.Year : 0
                })
                .ToListAsync();

            var monthlySummaries = payrollDetails
                .GroupBy(pd => new { pd.CreatedMonth, pd.CreatedYear })
                .Select(g => new MonthlyIncomeSummary
                {
                    Month = g.Key.CreatedMonth,
                    Year = g.Key.CreatedYear,
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

            var departmentIncomes = payrollDetails
                .GroupBy(pd => new { pd.OrganizationId, pd.OrganizationName })
                .Select(g =>
                {
                    var empCount = g.Select(x => x.EmployeeId).Distinct().Count();
                    var totalSalary = g.Sum(x => x.TotalReceivedSalary ?? 0);
                    return new DepartmentIncome
                    {
                        OrganizationId = g.Key.OrganizationId ?? 0,
                        OrganizationName = g.Key.OrganizationName ?? "Chưa phân bổ",
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
                DepartmentIncomes = departmentIncomes
            };
        }

        public async Task<PerformanceReportDto> GetPerformanceReport(int? organizationId, int? year, int? month)
        {
            // Query PayrollDetails for performance data
            var payrollQuery = _dbContext.PayrollDetails
                .Where(pd => pd.IsDeleted == false)
                .AsNoTracking();

            if (organizationId.HasValue)
                payrollQuery = payrollQuery.Where(pd => pd.OrganizationId == organizationId.Value);

            if (year.HasValue)
                payrollQuery = payrollQuery.Where(pd => pd.CreatedAt.HasValue && pd.CreatedAt.Value.Year == year.Value);

            if (month.HasValue)
                payrollQuery = payrollQuery.Where(pd => pd.CreatedAt.HasValue && pd.CreatedAt.Value.Month == month.Value);

            var payrollData = await payrollQuery
                .Select(pd => new
                {
                    pd.EmployeeId,
                    pd.FullName,
                    pd.Department,
                    pd.OrganizationId,
                    OrganizationName = pd.Organization != null ? pd.Organization.OrganizationName : "Chưa phân bổ",
                    pd.KPI,
                    pd.KpiPercentage,
                    pd.ActualWorkDays,
                    pd.StandardWorkDays
                })
                .ToListAsync();

            var employeePerformances = payrollData
                .GroupBy(pd => pd.EmployeeId)
                .Select(g =>
                {
                    var first = g.First();
                    var kpiValues = g.Where(x => x.KPI.HasValue).Select(x => x.KPI!.Value).ToList();
                    var kpiPctValues = g.Where(x => x.KpiPercentage.HasValue).Select(x => x.KpiPercentage!.Value).ToList();
                    var avgKpi = kpiValues.Count > 0 ? Math.Round(kpiValues.Average(), 2) : 0;
                    var avgKpiPct = kpiPctValues.Count > 0 ? Math.Round(kpiPctValues.Average(), 2) : 0;
                    var totalActual = g.Sum(x => x.ActualWorkDays ?? 0);
                    var totalStandard = g.Sum(x => (double)(x.StandardWorkDays ?? 0));
                    return new EmployeePerformance
                    {
                        EmployeeId = first.EmployeeId ?? 0,
                        FullName = first.FullName ?? "",
                        Department = first.Department ?? first.OrganizationName ?? "",
                        KpiScore = avgKpi,
                        KpiPercentage = avgKpiPct,
                        ActualWorkDays = totalActual,
                        StandardWorkDays = totalStandard,
                        WorkEfficiency = totalStandard > 0 ? Math.Round(totalActual / totalStandard * 100, 2) : 0
                    };
                })
                .OrderByDescending(e => e.KpiScore)
                .ToList();

            var departmentPerformances = payrollData
                .GroupBy(pd => new { pd.OrganizationId, pd.OrganizationName })
                .Select(g =>
                {
                    var kpiValues = g.Where(x => x.KPI.HasValue).Select(x => x.KPI!.Value).ToList();
                    var avgKpi = kpiValues.Count > 0 ? Math.Round(kpiValues.Average(), 2) : 0;
                    var sortedKpi = kpiValues.OrderBy(x => x).ToList();
                    var medianKpi = sortedKpi.Count > 0
                        ? (sortedKpi.Count % 2 == 0
                            ? Math.Round((sortedKpi[sortedKpi.Count / 2 - 1] + sortedKpi[sortedKpi.Count / 2]) / 2, 2)
                            : sortedKpi[sortedKpi.Count / 2])
                        : 0;
                    // Aggregate KpiPercentage per employee (average), then apply thresholds
                    var empKpiPcts = g.GroupBy(x => x.EmployeeId)
                        .Select(eg => eg.Where(x => x.KpiPercentage.HasValue).Select(x => x.KpiPercentage!.Value).DefaultIfEmpty(0).Average())
                        .ToList();
                    return new DepartmentPerformance
                    {
                        OrganizationId = g.Key.OrganizationId ?? 0,
                        OrganizationName = g.Key.OrganizationName ?? "Chưa phân bổ",
                        AverageKpi = avgKpi,
                        MedianKpi = medianKpi,
                        EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                        HighPerformers = empKpiPcts.Count(x => x >= 80),
                        LowPerformers = empKpiPcts.Count(x => x < 50)
                    };
                })
                .OrderByDescending(d => d.AverageKpi)
                .ToList();

            // KPI distribution histogram (based on averaged KpiPercentage per employee)
            var employeeKpiPcts = employeePerformances.Select(e => (double)e.KpiPercentage).ToList();
            var totalEmpCount = employeeKpiPcts.Count;
            var kpiDistributions = new List<KpiDistribution>
            {
                new KpiDistribution { RangeLabel = "0-20%", Count = employeeKpiPcts.Count(x => x < 20), Percentage = totalEmpCount > 0 ? Math.Round(employeeKpiPcts.Count(x => x < 20) / (double)totalEmpCount * 100, 2) : 0 },
                new KpiDistribution { RangeLabel = "20-40%", Count = employeeKpiPcts.Count(x => x >= 20 && x < 40), Percentage = totalEmpCount > 0 ? Math.Round(employeeKpiPcts.Count(x => x >= 20 && x < 40) / (double)totalEmpCount * 100, 2) : 0 },
                new KpiDistribution { RangeLabel = "40-60%", Count = employeeKpiPcts.Count(x => x >= 40 && x < 60), Percentage = totalEmpCount > 0 ? Math.Round(employeeKpiPcts.Count(x => x >= 40 && x < 60) / (double)totalEmpCount * 100, 2) : 0 },
                new KpiDistribution { RangeLabel = "60-80%", Count = employeeKpiPcts.Count(x => x >= 60 && x < 80), Percentage = totalEmpCount > 0 ? Math.Round(employeeKpiPcts.Count(x => x >= 60 && x < 80) / (double)totalEmpCount * 100, 2) : 0 },
                new KpiDistribution { RangeLabel = "80-100%", Count = employeeKpiPcts.Count(x => x >= 80), Percentage = totalEmpCount > 0 ? Math.Round(employeeKpiPcts.Count(x => x >= 80) / (double)totalEmpCount * 100, 2) : 0 },
            };

            return new PerformanceReportDto
            {
                EmployeePerformances = employeePerformances,
                DepartmentPerformances = departmentPerformances,
                KpiDistributions = kpiDistributions
            };
        }

        public async Task<AttendanceReportDto> GetAttendanceReport(int? organizationId, int year, int? month)
        {
            // Query timesheets
            var timesheetQuery = _dbContext.Timesheets
                .Where(t => t.IsDeleted == false && t.Date.HasValue && t.Date.Value.Year == year)
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
                    Department = t.Employee != null && t.Employee.Organization != null
                        ? t.Employee.Organization.OrganizationName : "Chưa phân bổ",
                    OrganizationId = t.Employee != null ? t.Employee.OrganizationId : null,
                    Month = t.Date.HasValue ? t.Date.Value.Month : 0,
                    Year = t.Date.HasValue ? t.Date.Value.Year : 0,
                    t.NumberOfWorkingHour,
                    t.LateDuration,
                    t.EarlyLeaveDuration,
                    t.TimeKeepingLeaveStatus
                })
                .ToListAsync();

            // Monthly attendance
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

            // Employee attendance
            var employeeAttendances = timesheets
                .GroupBy(t => new { t.EmployeeId, t.FullName, t.Department })
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
                        Department = g.Key.Department ?? "",
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

            // Department attendance aggregation
            var departmentAttendances = timesheets
                .GroupBy(t => new { t.OrganizationId, t.Department })
                .Select(g =>
                {
                    var totalRecords = g.Count();
                    var workDays = g.Sum(x => (x.NumberOfWorkingHour ?? 0) > 0 ? 1.0 : 0);
                    var lateDays = g.Count(x => (x.LateDuration ?? 0) > 0);
                    var earlyLeaveDays = g.Count(x => (x.EarlyLeaveDuration ?? 0) > 0);
                    var absentDays = g.Count(x => (x.NumberOfWorkingHour ?? 0) == 0
                        && x.TimeKeepingLeaveStatus == Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None);
                    var overtimeHours = g.Sum(x => Math.Max((x.NumberOfWorkingHour ?? 0) - 8.0, 0));

                    return new DepartmentAttendance
                    {
                        OrganizationId = g.Key.OrganizationId ?? 0,
                        OrganizationName = g.Key.Department ?? "Chưa phân bổ",
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

            // OT summary from PayrollDetail (money + hours derived from timesheet)
            var otPayrollQuery = _dbContext.PayrollDetails
                .Where(pd => pd.IsDeleted == false && pd.CreatedAt.HasValue && pd.CreatedAt.Value.Year == year)
                .AsNoTracking();

            if (organizationId.HasValue)
                otPayrollQuery = otPayrollQuery.Where(pd => pd.OrganizationId == organizationId.Value);

            if (month.HasValue)
                otPayrollQuery = otPayrollQuery.Where(pd => pd.CreatedAt.HasValue && pd.CreatedAt.Value.Month == month.Value);

            var otPayrollData = await otPayrollQuery
                .Select(pd => new
                {
                    pd.OvertimeAmount,
                    CreatedMonth = pd.CreatedAt.HasValue ? pd.CreatedAt.Value.Month : 0,
                    CreatedYear = pd.CreatedAt.HasValue ? pd.CreatedAt.Value.Year : 0
                })
                .ToListAsync();

            var overtimeSummaries = otPayrollData
                .GroupBy(pd => new { pd.CreatedMonth, pd.CreatedYear })
                .Select(g =>
                {
                    var monthData = monthlyAttendances.FirstOrDefault(m => m.Month == g.Key.CreatedMonth);
                    return new OvertimeSummary
                    {
                        Month = g.Key.CreatedMonth,
                        Year = g.Key.CreatedYear,
                        TotalOvertimeHours = monthData?.TotalOvertimeHours ?? 0,
                        TotalOvertimePay = g.Sum(x => x.OvertimeAmount ?? 0)
                    };
                })
                .OrderBy(o => o.Month)
                .ToList();

            // Leave type distribution
            var leaveQuery = _dbContext.LeaveApplications
                .Where(la => la.IsDeleted == false
                    && la.Status == Core.Data.Official_Form.LeaveApplicationStatus.Approved
                    && la.StartDate.HasValue && la.StartDate.Value.Year == year)
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
                DepartmentAttendances = departmentAttendances,
                OvertimeSummaries = overtimeSummaries
            };
        }
    }
}
