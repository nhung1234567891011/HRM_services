namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class RevenueCommissionTierRequest
    {
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal RatePercent { get; set; }
        public int SortOrder { get; set; }
    }
}

