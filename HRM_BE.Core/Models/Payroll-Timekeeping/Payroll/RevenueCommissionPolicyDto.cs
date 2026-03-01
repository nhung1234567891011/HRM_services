using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class RevenueCommissionPolicyDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public RevenueCommissionTargetType TargetType { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public Status Status { get; set; }

        public List<RevenueCommissionTierDto> Tiers { get; set; } = new();
    }
}

