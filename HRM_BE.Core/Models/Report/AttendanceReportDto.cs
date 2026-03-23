namespace HRM_BE.Core.Models.Report
{
    public class AttendanceReportDto
    {
        public List<MonthlyAttendance> MonthlyAttendances { get; set; } = new();
        public List<EmployeeAttendance> EmployeeAttendances { get; set; } = new();
        public List<LeaveTypeDistribution> LeaveTypeDistributions { get; set; } = new();
        public List<PositionAttendance> PositionAttendances { get; set; } = new();
        public List<OvertimeSummary> OvertimeSummaries { get; set; } = new();
    }

    public class MonthlyAttendance
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalWorkDays { get; set; }
        public int TotalLateDays { get; set; }
        public int TotalEarlyLeaveDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public double TotalLeaveDays { get; set; }
        public double TotalOvertimeHours { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class EmployeeAttendance
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public double WorkDays { get; set; }
        public int LateDays { get; set; }
        public int EarlyLeaveDays { get; set; }
        public int AbsentDays { get; set; }
        public double LeaveDays { get; set; }
        public double OvertimeHours { get; set; }
        public double TotalLateDuration { get; set; }
        public double TotalEarlyLeaveDuration { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class LeaveTypeDistribution
    {
        public string LeaveType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalDays { get; set; }
    }

    public class PositionAttendance
    {
        public int StaffPositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public double TotalWorkDays { get; set; }
        public int TotalLateDays { get; set; }
        public int TotalEarlyLeaveDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public double TotalOvertimeHours { get; set; }
        public double AttendanceRate { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class OvertimeSummary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalOvertimeHours { get; set; }
        public decimal TotalOvertimePay { get; set; }
    }
}
