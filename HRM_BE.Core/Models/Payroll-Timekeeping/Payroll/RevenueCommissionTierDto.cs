namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class RevenueCommissionTierDto
    {
        public int Id { get; set; }
        public int? PolicyId { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal RatePercent { get; set; }
        public int SortOrder { get; set; }
    }
}

