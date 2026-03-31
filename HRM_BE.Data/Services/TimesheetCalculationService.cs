using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.IServices;
using Microsoft.EntityFrameworkCore;

namespace HRM_BE.Data.Services
{
    public class TimesheetCalculationService : ITimesheetCalculationService
    {
        private readonly HrmContext _dbContext;

        public TimesheetCalculationService(HrmContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<double> CalculateWorkingHours(Timesheet timesheet)
        {
            if (!timesheet.StartTime.HasValue || !timesheet.EndTime.HasValue)
            {
                return 0;
            }

            if (!timesheet.ShiftWorkId.HasValue)
            {
                return 0;
            }

            var shift = await _dbContext.ShiftWorks
                .Include(s => s.ShiftCatalog)
                .FirstOrDefaultAsync(s => s.Id == timesheet.ShiftWorkId);

            if (shift?.ShiftCatalog == null)
            {
                return 0;
            }

            var date = timesheet.Date ?? DateTime.Today;

            var checkInTime = date.Date.Add(timesheet.StartTime.Value);
            var checkOutTime = date.Date.Add(timesheet.EndTime.Value);

            var start = checkInTime;
            var end = checkOutTime;

            if (end <= start)
            {
                return 0;
            }

            double workingHours = (end - start).TotalHours;

            if (shift.ShiftCatalog.TakeABreak == true 
                && shift.ShiftCatalog.StartTakeABreak.HasValue 
                && shift.ShiftCatalog.EndTakeABreak.HasValue)
            {
                var breakStart = date.Date.Add(shift.ShiftCatalog.StartTakeABreak.Value);
                var breakEnd = date.Date.Add(shift.ShiftCatalog.EndTakeABreak.Value);

                if (start < breakEnd && end > breakStart)
                {
                    var overlapStart = start > breakStart ? start : breakStart;
                    var overlapEnd = end < breakEnd ? end : breakEnd;

                    workingHours -= (overlapEnd - overlapStart).TotalHours;
                }
            }

            return Math.Max(0, Math.Round(workingHours, 2));
        }

        public async Task<double> CalculateOvertimeHours(Timesheet timesheet)
        {
            var workingHours = await CalculateWorkingHours(timesheet);

            if (!timesheet.ShiftWorkId.HasValue)
            {
                return 0;
            }

            var shift = await _dbContext.ShiftWorks
                .Include(s => s.ShiftCatalog)
                .FirstOrDefaultAsync(s => s.Id == timesheet.ShiftWorkId);

            if (shift?.ShiftCatalog == null)
            {
                return 0;
            }

            var standardHours = shift.ShiftCatalog.WorkingHours ?? 8.0;

            if (workingHours <= standardHours)
            {
                return 0;
            }

            return Math.Round(workingHours - standardHours, 2);
        }

        public Task<double> CalculateWorkDays(double workingHours)
        {
            return Task.FromResult(workingHours > 0 ? 1.0 : 0.0);
        }
    }
}
