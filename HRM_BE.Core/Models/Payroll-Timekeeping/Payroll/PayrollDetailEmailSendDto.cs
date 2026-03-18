using System;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class PayrollDetailEmailSendDto
    {
        public int Id { get; set; }
        public int? PayrollId { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }

        public decimal? BaseSalary { get; set; }
        public int? StandardWorkDays { get; set; }
        public double? ActualWorkDays { get; set; }
        public decimal? ReceivedSalary { get; set; }
        public decimal? KpiSalary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? TotalReceivedSalary { get; set; }
        public DateTime? ResponseDeadline { get; set; }
    }
}

