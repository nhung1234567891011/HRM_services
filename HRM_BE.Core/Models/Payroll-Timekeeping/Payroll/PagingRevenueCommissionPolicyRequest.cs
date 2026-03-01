using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Models.Common;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class PagingRevenueCommissionPolicyRequest : PagingRequest
    {
        public int? OrganizationId { get; set; }
        public RevenueCommissionTargetType? TargetType { get; set; }
        public Status? Status { get; set; }
    }
}

