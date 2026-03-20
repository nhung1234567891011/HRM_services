using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.ShiftCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.IRepositories
{
    public interface IShiftCatalogRepository:IRepositoryBase<ShiftCatalog,int>
    {
        Task<ShiftCatalogDto> GetById(int shiftCatalogId);
        Task<PagingResult<ShiftCatalogDto>> Paging(string? name,int? organizationId,string? sortBy,string? orderBy,int pageIndex = 1, int pageSize = 10);
        Task<ShiftCatalogDto> Create(CreateShiftCatalogRequest request);
        Task Update(int shiftCatalogId,UpdateShiftCatalogRequest request);
        Task Delete(int shiftCatalogId);
        Task DeleteRange(ListEntityIdentityRequest<int> request);
        Task<ShiftCatalogDto> GetByShiftWorkId(int shiftWorkId);


    }
}
