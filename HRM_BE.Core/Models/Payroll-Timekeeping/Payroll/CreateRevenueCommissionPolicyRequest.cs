using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class CreateRevenueCommissionPolicyRequest
    {
        public int? OrganizationId { get; set; }
        public RevenueCommissionTargetType TargetType { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public Status Status { get; set; } = Status.Tracking;
        public List<RevenueCommissionTierRequest> Tiers { get; set; } = new();
    }
}

