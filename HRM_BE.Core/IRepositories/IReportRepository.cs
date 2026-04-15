using HRM_BE.Core.Models.Report;

namespace HRM_BE.Core.IRepositories;

public interface IReportRepository
{
    Task<HrDistributionReportDto> GetHrDistributionReport(int? organizationId);
    Task<MonthlyIncomeReportDto> GetMonthlyIncomeReport(int? organizationId, int year);
    Task<PerformanceReportDto> GetPerformanceReport(
        int? organizationId,
        int? year,
        int? month,
        int? fromYear,
        int? fromMonth,
        int? toYear,
        int? toMonth);
    Task<AttendanceReportDto> GetAttendanceReport(int? organizationId, int year, int? month);
}
