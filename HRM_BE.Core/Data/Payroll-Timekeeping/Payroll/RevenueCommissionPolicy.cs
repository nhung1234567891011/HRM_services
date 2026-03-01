using HRM_BE.Core.Data.Company;

namespace HRM_BE.Core.Data.Payroll_Timekeeping.Payroll
{
    public class RevenueCommissionPolicy : EntityBase<int>
    {
        public int? OrganizationId { get; set; }
        public RevenueCommissionTargetType TargetType { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public Status Status { get; set; } = Status.Tracking;

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<RevenueCommissionTier> Tiers { get; set; } = new List<RevenueCommissionTier>();
    }

    public enum RevenueCommissionTargetType
    {
        Sale = 0,
        Ctv = 1
    }
}

