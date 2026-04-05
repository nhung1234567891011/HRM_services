using System;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class ExportPayrollDetailRequest
    {
        public int? DetailTimesheetId { get; set; }
        public int? OrganizationId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; }
        public string? OrderBy { get; set; }
    }
}
