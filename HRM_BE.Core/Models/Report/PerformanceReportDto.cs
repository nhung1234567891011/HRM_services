namespace HRM_BE.Core.Models.Report
{
    public class PerformanceReportDto
    {
        public List<EmployeePerformance> EmployeePerformances { get; set; } = new();
        public List<PositionPerformance> PositionPerformances { get; set; } = new();
        public List<KpiDistribution> KpiDistributions { get; set; } = new();
    }

    public class EmployeePerformance
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal KpiScore { get; set; }
        public decimal KpiPercentage { get; set; }
        public double ActualWorkDays { get; set; }
        public double StandardWorkDays { get; set; }
        public double WorkEfficiency { get; set; }
    }

    public class PositionPerformance
    {
        public int StaffPositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public decimal AverageKpi { get; set; }
        public decimal MedianKpi { get; set; }
        public int EmployeeCount { get; set; }
        public int HighPerformers { get; set; }
        public int LowPerformers { get; set; }
    }

    public class KpiDistribution
    {
        public string RangeLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
