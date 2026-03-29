using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class ApplyOrganizationRepository : RepositoryBase<ApplyOrganization, int>, IApplyOrganizationRepository
    {
        private readonly IMapper _mapper;

        public ApplyOrganizationRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ApplyOrganizationDto> Create(CreateApplyOrganizationRequest request)
        {
            var entity = _mapper.Map<ApplyOrganization>(request);
            await CreateAsync(entity);
            return _mapper.Map<ApplyOrganizationDto>(entity);
        }

        public async Task<ApplyOrganizationDto> GetById(int id)
        {
            var entity = await GetApplyOrganizationAndCheckExist(id);
            return _mapper.Map<ApplyOrganizationDto>(entity);
        }

        public async Task<ApplyOrganizationDto?> GetFirstByOrganizationId(int organizationId)
        {
            var query = _dbContext.ApplyOrganizations
                .AsNoTracking()
                .Where(x => x.IsDeleted != true && x.OrganizationId == organizationId)
                .OrderByDescending(x => x.Id);

            return await _mapper.ProjectTo<ApplyOrganizationDto>(query).FirstOrDefaultAsync();
        }

        public async Task<PagingResult<ApplyOrganizationDto>> Paging(int? timekeepingSettingId, int? organizationId, int? timekeepingLocationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.ApplyOrganizations
                .AsNoTracking()
                .Where(x => x.IsDeleted != true)
                .AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(x => x.OrganizationId == organizationId);
            }

            if (timekeepingSettingId.HasValue)
            {
                query = query.Where(x => x.TimekeepingSettingId == timekeepingSettingId);
            }

            if (timekeepingLocationId.HasValue)
            {
                query = query.Where(x => x.TimekeepingLocationId == timekeepingLocationId);
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<ApplyOrganizationDto>(query).ToListAsync();

            var result = new PagingResult<ApplyOrganizationDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task Update(int id, UpdateApplyOrganizationRequest request)
        {
            var entity = await GetApplyOrganizationAndCheckExist(id);
            await UpdateAsync(_mapper.Map(request, entity));
        }


        public async Task<List<TimekeepingLocationDto>> GetTimekeepingLocations(int organizationId)
        {
            var query = _dbContext.TimekeepingLocations
                .Where(x => x.OrganizationId == organizationId && x.IsDeleted != true);

            var locations = await _mapper.ProjectTo<TimekeepingLocationDto>(query).ToListAsync();
            return locations;
        }


        private async Task<ApplyOrganization> GetApplyOrganizationAndCheckExist(int applyOrganizationId)
        {
            var applyOrganization = await _dbContext.ApplyOrganizations.FindAsync(applyOrganizationId);
            if (applyOrganization is null)
                throw new EntityNotFoundException(nameof(ApplyOrganization), $"Id = {applyOrganizationId}");
            return applyOrganization;
        }

        public async Task<List<ApplyOrganizationListDto>> GetApplyOrganizations(int? organizationId)
        {
            var query = _dbContext.ApplyOrganizations
                .Include(x => x.Organization)
                .Include(x => x.TimekeepingLocation)
                .Include(x => x.ApplyEmployeeTimekeepingSettings)
                .Where(x => x.IsDeleted != true)
                .AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(x => x.OrganizationId == organizationId);
            }

            var result = await query.Select(x => new ApplyOrganizationListDto
            {
                Id = x.Id,
                OrganizationName = x.Organization.OrganizationName,
                IsForAllEmployees = !x.ApplyEmployeeTimekeepingSettings.Any(),
                AllowableRadius = x.TimekeepingLocation != null ? x.TimekeepingLocation.AllowableRadius : 0
            }).ToListAsync();

            return result;
        }

        public async Task DeleteApplyOrganization(int id)
        {
            var entity = await GetApplyOrganizationAndCheckExist(id);

            // Đánh dấu là đã xóa
            entity.IsDeleted = true;

            await UpdateAsync(entity);
        }

    }
}
