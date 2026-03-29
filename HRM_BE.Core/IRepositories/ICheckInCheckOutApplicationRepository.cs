using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.CheckInCheckOut;

namespace HRM_BE.Core.IRepositories
{
    public interface ICheckInCheckOutApplicationRepository : IRepositoryBase<CheckInCheckOutApplication, int>
    {
        Task<PagingResult<CheckInCheckOutApplicationDto>> GetPaging(
            int? organizationId,
            int? employeeId,
            int currentEmployeeId,
            bool isAdmin,
            bool forApproval,
            string? keyWord,
            DateTime? startDate,
            DateTime? endDate,
            int? status,
            string? sortBy,
            string? orderBy,
            int pageIndex = 1,
            int pageSize = 10);

        Task<CheckInCheckOutApplicationDto> GetById(int id);
        Task<int> CreateAsync(CreateCheckInCheckOutApplicationRequest request);
        Task UpdateAsync(int id, UpdateCheckInCheckOutApplicationRequest request);
        Task UpdateStatusAsync(int id, int status);
        Task<List<CheckInCheckOutApplicationDto>> GetExportData(
            int? organizationId,
            int? employeeId,
            int currentEmployeeId,
            bool isAdmin,
            bool forApproval,
            string? keyWord,
            DateTime? startDate,
            DateTime? endDate,
            int? status);
    }
}

