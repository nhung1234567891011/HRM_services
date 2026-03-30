using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation
{
    public class CreateTimesheetRequest
    {
        public int EmployeeId { get; set; } // Nhân viên

        public int? ShiftWorkId { get; set; } // Ca làm việc (tuỳ chọn)

        public DateTime Date { get; set; } // Ngày chấm công

        public TimeSpan? StartTime { get; set; } // Giờ vào

        public TimeSpan? EndTime { get; set; } // Giờ ra

        public TimeKeepingLeaveStatus TimeKeepingLeaveStatus { get; set; } = TimeKeepingLeaveStatus.None;

        public double? LateDuration { get; set; } = 0;

        public double? EarlyLeaveDuration { get; set; } = 0;
    }
}
