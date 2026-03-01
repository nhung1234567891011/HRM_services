using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Core.IRepositories
{
    public interface IRevenueCommissionPolicyRepository : IRepositoryBase<RevenueCommissionPolicy, int>
    {
        Task<PagingResult<RevenueCommissionPolicyDto>> Paging(PagingRevenueCommissionPolicyRequest request);
        Task<RevenueCommissionPolicyDto> GetById(int id);
        Task<RevenueCommissionPolicyDto> Create(CreateRevenueCommissionPolicyRequest request);
        Task Update(int id, UpdateRevenueCommissionPolicyRequest request);
        Task UpdateStatus(int id, Status status);
        Task Delete(int id);
    }
}

