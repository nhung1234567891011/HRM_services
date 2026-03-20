using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Company;
using HRM_BE.Core.Models.ShiftCatalog;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace HRM_BE.Data.Repositories
{
    public class ShiftCatalogRepository : RepositoryBase<ShiftCatalog, int>, IShiftCatalogRepository
    {
        private readonly IMapper _mapper;
        public ShiftCatalogRepository(HrmContext context,IMapper mapper,IHttpContextAccessor httpContextAccessor):base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<ShiftCatalogDto> GetById(int shiftCatalogId)
        {
            var shiftCatalog = await GetShiftCatalogAndCheckExist(shiftCatalogId);

            return _mapper.Map<ShiftCatalogDto>(shiftCatalog);
        }

        public async Task<PagingResult<ShiftCatalogDto>> Paging(string? name, int? organizationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.ShiftCatalogs.Include(s => s.Organization).AsNoTracking();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }
            if (organizationId.HasValue)
            {
                query = query.Where(c => c.OrganizationId == organizationId);
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<ShiftCatalogDto>(query).ToListAsync();

            var result = new PagingResult<ShiftCatalogDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }
        public async Task<ShiftCatalogDto> Create(CreateShiftCatalogRequest request)
        {
            var shiftCatalog = _mapper.Map<ShiftCatalog>(request);
            var shiftCatalogReturn = await CreateAsync(shiftCatalog);
            return _mapper.Map<ShiftCatalogDto>(shiftCatalogReturn);
        }

        public async Task Delete(int shiftCatalogId)
        {
            var shiftCatalog = await GetShiftCatalogAndCheckExist(shiftCatalogId);
            shiftCatalog.IsDeleted = true;
            await UpdateAsync(shiftCatalog);

        }

        public async Task DeleteRange(ListEntityIdentityRequest<int> request)
        {
            var entities = await _dbContext.ShiftCatalogs
                .Where(x => request.Ids.Contains(x.Id))
                .ToListAsync();

            entities.ForEach(x => x.IsDeleted = true);
            await SaveChangesAsync();
        }

        public async Task<ShiftCatalogDto> GetByShiftWorkId(int shiftWorkId)
        {
            var shiftCatalog = await _dbContext.ShiftWorks.Include(sw => sw.ShiftCatalog).FirstOrDefaultAsync(sw => sw.Id == shiftWorkId);
            if (shiftCatalog is null)
                throw new EntityNotFoundException(nameof(ShiftCatalog), $"ShiftWorkId = {shiftWorkId}");
            return _mapper.Map<ShiftCatalogDto>(shiftCatalog.ShiftCatalog);
        }


        public async Task Update(int shiftCatalogId, UpdateShiftCatalogRequest request)
        {
            var shiftCatalog = await GetShiftCatalogAndCheckExist(shiftCatalogId);
            await UpdateAsync(_mapper.Map(request,shiftCatalog));
        }
        private async Task<ShiftCatalog> GetShiftCatalogAndCheckExist(int shiftCatalogId)
        {
            var shiftCatalog = await _dbContext.ShiftCatalogs.Include( s => s.Organization).SingleOrDefaultAsync( s => s.Id == shiftCatalogId);
            if (shiftCatalog is null)
                throw new EntityNotFoundException(nameof(shiftCatalog),$"Id = {shiftCatalogId}");
            return shiftCatalog;
        }
    }
}
