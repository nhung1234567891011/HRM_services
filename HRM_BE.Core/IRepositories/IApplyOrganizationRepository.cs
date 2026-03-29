using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.IRepositories
{
    public interface IApplyOrganizationRepository : IRepositoryBase<ApplyOrganization, int>
    {
        Task<PagingResult<ApplyOrganizationDto>> Paging(int? timekeepingSettingId, int? organizationId, int? timekeepingLocationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10);
        Task<ApplyOrganizationDto?> GetFirstByOrganizationId(int organizationId);
        Task<ApplyOrganizationDto> Create(CreateApplyOrganizationRequest request);
        Task Update(int id, UpdateApplyOrganizationRequest request);
        Task<ApplyOrganizationDto> GetById(int id);
        Task<List<TimekeepingLocationDto>> GetTimekeepingLocations(int organizationId);
        Task<List<ApplyOrganizationListDto>> GetApplyOrganizations(int? organizationId);
        Task DeleteApplyOrganization(int id);

    }
}
