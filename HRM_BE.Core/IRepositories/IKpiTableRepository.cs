using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Salary.KpiTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.IRepositories
{

    public interface IKpiTableRepository : IRepositoryBase<KpiTable, int>
    {
        Task<KpiTableDto> GetById(int KpiTableId);
        Task<PagingResult<KpiTableDto>> Paging(string? name, int? organizationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10);
        Task<KpiTableDto> Create(CreateKpiTableRequest request);
        Task Update(int KpiTableId, UpdateKpiTableRequest request);
        Task Delete(int KpiTableId);
        Task HardDelete(int KpiTableId);
        Task<KpiTableDto> GetByShiftWorkId(int shiftWorkId);
    }
}
