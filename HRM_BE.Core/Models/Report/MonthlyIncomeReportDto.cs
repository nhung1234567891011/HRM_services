namespace HRM_BE.Core.Models.Report
{
    public class MonthlyIncomeReportDto
    {
        public List<MonthlyIncomeSummary> MonthlySummaries { get; set; } = new();
        public List<PositionIncome> PositionIncomes { get; set; } = new();
    }

    public class MonthlyIncomeSummary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalBaseSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalOvertimePay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PositionIncome
    {
        public int StaffPositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public decimal TotalBaseSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalOvertimePay { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AverageSalary { get; set; }
        public int EmployeeCount { get; set; }
    }
}
