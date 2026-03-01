using AutoMapper;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Salary.KpiTableDetail;
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
    public class KpiTableDetailRepository : RepositoryBase<KpiTableDetail, int>, IKpiTableDetailRepository
    {
        private readonly IMapper _mapper;
        public KpiTableDetailRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<KpiTableDetailDto> GetById(int KpiTableDetailId)
        {
            var KpiTableDetail = await GetKpiTableDetailAndCheckExist(KpiTableDetailId);
            return _mapper.Map<KpiTableDetailDto>(KpiTableDetail);
        }

        public async Task<PagingResult<KpiTableDetailDto>> Paging(GetKpiTableDetailRequest request, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.KpiTableDetails
                .Include(e => e.Employee)
                .ThenInclude(e => e.StaffPosition)
                .AsNoTracking();
            if (request.KpiTableId.HasValue)
            {
                query = query.Where(c => c.KpiTableId == request.KpiTableId);
            }
            if (!string.IsNullOrEmpty(request.EmployeeName))
            {
                query = query.Where(c => c.EmployeeName.Contains(request.EmployeeName));
            }

            if (request.OrganizationId.HasValue)
            {
                var organizationDescendantIds = await GetAllChildOrganizationIds(request.OrganizationId.Value);
                organizationDescendantIds.Add(request.OrganizationId.Value);
                query = query.Where(c => organizationDescendantIds.Contains(c.Employee.OrganizationId.Value));
            }

            if(request.StaffPositionId.HasValue)
            {
                query = query.Where(d => d.Employee.StaffPositionId.Value == request.StaffPositionId);

            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<KpiTableDetailDto>(query).ToListAsync();

            var result = new PagingResult<KpiTableDetailDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }
        private async Task<List<int>> GetAllChildOrganizationIds(int parentId)
        {
            // Lấy tất cả các tổ chức
            var allOrganizations = await _dbContext.Organizations.AsNoTracking().ToListAsync();

            // Gọi hàm đệ quy để tìm tất cả các Id con
            var result = new List<int>();
            GetChildIdsRecursive(parentId, allOrganizations, result);
            return result;
        }

        private void GetChildIdsRecursive(int parentId, List<Organization> allOrganizations, List<int> result)
        {
            // Lấy tất cả các con trực tiếp của parentId
            var children = allOrganizations.Where(o => o.OrganizatioParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child.Id); // Thêm Id của con vào danh sách kết quả
                GetChildIdsRecursive(child.Id, allOrganizations, result); // Gọi đệ quy cho các con
            }
        }
        public async Task<KpiTableDetailDto> Create(CreateKpiTableDetailRequest request)
        {
            var KpiTableDetail = _mapper.Map<KpiTableDetail>(request);
            var KpiTableDetailReturn = await CreateAsync(KpiTableDetail);
            return _mapper.Map<KpiTableDetailDto>(KpiTableDetailReturn);
        }

        public async Task Delete(int KpiTableDetailId)
        {
            var KpiTableDetail = await GetKpiTableDetailAndCheckExist(KpiTableDetailId);
            KpiTableDetail.IsDeleted = true;
            await UpdateAsync(KpiTableDetail);

        }

        public async Task<KpiTableDetailDto> GetByShiftWorkId(int shiftWorkId)
        {
            var KpiTableDetail = await _dbContext.ShiftWorks.FirstOrDefaultAsync(sw => sw.Id == shiftWorkId);
            if (KpiTableDetail is null)
                throw new EntityNotFoundException(nameof(KpiTableDetail), $"ShiftWorkId = {shiftWorkId}");
            return _mapper.Map<KpiTableDetailDto>(KpiTableDetail);
        }

        public async Task Update(int KpiTableDetailId, UpdateKpiTableDetailRequest request)
        {
            var KpiTableDetail = await GetKpiTableDetailAndCheckExist(KpiTableDetailId);
            await UpdateAsync(_mapper.Map(request, KpiTableDetail));
        }
        private async Task<KpiTableDetail> GetKpiTableDetailAndCheckExist(int KpiTableDetailId)
        {
            var KpiTableDetail = await _dbContext.KpiTableDetails.SingleOrDefaultAsync(s => s.Id == KpiTableDetailId);
            if (KpiTableDetail is null)
                throw new EntityNotFoundException(nameof(KpiTableDetail), $"Id = {KpiTableDetailId}");
            return KpiTableDetail;
        }
    }
}
