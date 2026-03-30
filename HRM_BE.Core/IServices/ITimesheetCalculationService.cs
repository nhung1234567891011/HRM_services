using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;

namespace HRM_BE.Core.IServices
{
    public interface ITimesheetCalculationService
    {
        Task<double> CalculateWorkingHours(Timesheet timesheet);
        
        Task<double> CalculateOvertimeHours(Timesheet timesheet);
        
        Task<double> CalculateWorkDays(double workingHours);
    }
}
