namespace HRM_BE.Core.Data.Payroll_Timekeeping.Payroll
{
    public class RevenueCommissionTier : EntityBase<int>
    {
        public int? PolicyId { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal RatePercent { get; set; }
        public int SortOrder { get; set; }

        public virtual RevenueCommissionPolicy? Policy { get; set; }
    }
}

