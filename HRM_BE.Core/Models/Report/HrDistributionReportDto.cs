namespace HRM_BE.Core.Models.Report
{
    public class HrDistributionReportDto
    {
        public int TotalEmployees { get; set; }
        public int TotalActiveEmployees { get; set; }
        public List<DepartmentDistribution> DepartmentDistributions { get; set; } = new();
        public List<PositionDistribution> PositionDistributions { get; set; } = new();
        public List<StatusDistribution> StatusDistributions { get; set; } = new();
    }

    public class DepartmentDistribution
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public double Percentage { get; set; }
    }

    public class PositionDistribution
    {
        public int StaffPositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public double Percentage { get; set; }
    }

    public class StatusDistribution
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
