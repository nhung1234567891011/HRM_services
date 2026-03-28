using AutoMapper;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Profile;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DataContract = HRM_BE.Core.Data.Profile.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Helpers;
using HRM_BE.Core.Models.Company;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Models.Profile.ContractType;
using HRM_BE.Core.Models.Contract;

namespace HRM_BE.Data.Repositories
{
    public class ContractRepository : RepositoryBase<DataContract, int>, IContractRepository
    {
        public IMapper _mapper;

        public ContractRepository( HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<List<ContractDTO>> GetAll(string? nameEmployee, string? unit, int? unitId, bool? expiredStatus, string? sortBy, string? orderBy)
        {
            var query = _dbContext.Contracts
                .Include(x => x.Unit)
                .Include(p => p.ContractType)
                .Include(p => p.ContractDuration)
                .Include(p => p.Employee)
                .AsNoTracking()
                .Where(c => c.IsDeleted != true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nameEmployee))
                query = query.Where(p => p.NameEmployee != null && p.NameEmployee.Contains(nameEmployee));
            if (expiredStatus.HasValue)
                query = query.Where(p => p.ExpiredStatus == expiredStatus);
            if (!string.IsNullOrEmpty(unit))
                query = query.Where(p => p.Unit != null && p.Unit.OrganizationName.Contains(unit));
            if (unitId.HasValue)
                query = query.Where(p => p.UnitId != null && p.UnitId == unitId);

            query = query.ApplySorting(sortBy, orderBy);

            return await _mapper.ProjectTo<ContractDTO>(query).ToListAsync();
        }

        public async Task<PagingResult<ContractDTO>> Paging(string? nameEmployee,string? unit, int? unitId,bool? expiredStatus, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contracts
                .Include(x => x.Unit)
                .Include(p => p.ContractType)
                .Include(p => p.ContractDuration)
                .Include(p => p.Employee)
                .AsNoTracking()
                .Where(c => c.IsDeleted != true)
                .AsQueryable();

            // Lọc theo tên nhân viên 
            if (!string.IsNullOrEmpty(nameEmployee))
            {
                query = query.Where(p => p.NameEmployee != null && p.NameEmployee.Contains(nameEmployee));
            }
            if (expiredStatus.HasValue)
            {
                query = query.Where(p => p.ExpiredStatus == expiredStatus);

            }
            if (!string.IsNullOrEmpty(unit))
            {
                query = query.Where(p => p.Unit != null && p.Unit.OrganizationName.Contains(unit));
            }
            if (unitId.HasValue)
            {
                query = query.Where(p => p.UnitId != null && p.UnitId == unitId);
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);

            // Tính tổng số bản ghi
            int totalRecords = await query.CountAsync();

            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            // Lấy dữ liệu và chuyển sang DTO
            var data = await _mapper.ProjectTo<ContractDTO>(query).ToListAsync();


            // Kết quả phân trang
            var result = new PagingResult<ContractDTO>(data, pageIndex, pageSize, sortBy, orderBy, totalRecords);

            return result;
        }

        public async Task<ContractDTO> Create(CreateContractRequest request)
        {
            await CheckExpireStatus(request.EmployeeId.Value);
            var entity = _mapper.Map<DataContract>(request);
            entity.Code =request.Code;
            await CreateAsync(entity);
            return _mapper.Map<ContractDTO>(entity);
        }

        
        public async Task Update(int id, UpdateContractRequest request)
        {
            var entity = await GetContractAndCheckExsit(id);
            EnsureContractCanBeEdited(entity);
            await UpdateAsync(_mapper.Map(request, entity));
        }

        public async Task UpdateExpiredStatus(int id, UpdateContractExpiredStatusRequest request)
        {
            var entity = await GetContractAndCheckExsit(id);

            if (entity.ExpiredStatus == true)
                throw new BadHttpRequestException("Hợp đồng đã ở trạng thái hết hiệu lực, không thể chấm dứt lại.");

            await UpdateAsync(_mapper.Map(request, entity));
        }

        private static void EnsureContractCanBeEdited(DataContract contract)
        {
            if (contract.ExpiredStatus == true)
                throw new BadHttpRequestException("Hợp đồng đã chấm dứt/hết hiệu lực, không được chỉnh sửa thông tin chính.");
        }

        private async Task<DataContract> GetContractAndCheckExsit(int contractId)
        {
            var contract = await _dbContext.Contracts.FindAsync(contractId);
            if (contract is null)
                throw new EntityNotFoundException(nameof(Contract), $"Id = {contractId}");
            return contract;
        }
        private async Task CheckExpireStatus( int employeeId)
        {
            var contract = await _dbContext.Contracts.Where(c =>c.EmployeeId == employeeId && c.ExpiredStatus == false)
                .FirstOrDefaultAsync();
            if (contract is not null)
                throw new EntityAlreadyExistsException("Nhân viên đã có hợp đồng đang hoạt động ");
        }
        public async Task<ContractDTO> GetById(int id)
        {
            var entity = await GetContractAndCheckExsit(id);
            return _mapper.Map<ContractDTO>(entity);
        }

        public async Task Delete(int id)
        {
            var entity = await GetContractAndCheckExsit(id);
            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }

        public async Task DeleteRange(ListEntityIdentityRequest<int> ids)
        {
            var listEntity = await _dbContext.Contracts.Where( c => ids.Ids.Contains( c.Id ) ).ToListAsync();
            listEntity.ForEach(c=> c.IsDeleted = true);
            await UpdateRangeAsync(listEntity);
        }

        public async Task<bool> CheckEmployeeHaveContractValid(int employeeId)
        {
            var currentDate = DateTime.Today;

            // Kiểm tra xem nhân viên có hợp đồng hợp lệ và còn hạn không
            var hasValidContract = await _dbContext.Contracts
                .AsNoTracking()
                .AnyAsync(c =>
                    c.EmployeeId == employeeId &&
                    c.EffectiveDate <= currentDate &&
                    (c.ExpiryDate == null || c.ExpiryDate >= currentDate));

            return hasValidContract;
        }
    }
}
