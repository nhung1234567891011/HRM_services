using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
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
    public class SalaryComponentRepository : RepositoryBase<SalaryComponent, int>, ISalaryComponentRepository
    {
        private readonly IMapper _mapper;

        public SalaryComponentRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        // Danh sách mặc định các thành phần lương của hệ thống
        private readonly List<SalaryComponent> _defaultSalaryComponents = new()
        {
            new SalaryComponent
            {
                ComponentName = "Tỉ lệ hưởng lương",
                ComponentCode = "TI_LE_HUONG_LUONG",
                Nature = Nature.Other,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Tỉ lệ hưởng lương theo quy định của công ty"
            },
            new SalaryComponent
            {
                ComponentName = "Ngày công chuẩn",
                ComponentCode = "NGAY_CONG_CHUAN",
                Nature = Nature.Other,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Số ngày công chuẩn trong tháng"
            },
            new SalaryComponent
            {
                ComponentName = "Ngày công thực tế",
                ComponentCode = "NGAY_CONG_THUC_TE",
                Nature = Nature.Other,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Số ngày công thực tế trong tháng"
            },
            new SalaryComponent
            {
                ComponentName = "KPI đạt",
                ComponentCode = "KPI_DAT",
                Nature = Nature.Other,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Thưởng KPI đạt theo từng tháng"
            },
            new SalaryComponent
            {
                ComponentName = "Lương KPI",
                ComponentCode = "LUONG_KPI",
                Nature = Nature.Earning,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Lương KPI theo từng tháng"
            },
            new SalaryComponent
            {
                ComponentName = "Lương thưởng",
                ComponentCode = "LUONG_THUONG",
                Nature = Nature.Earning,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.FixedAmount,
                Description = "Lương thưởng theo từng tháng"
            },
            new SalaryComponent
            {
                ComponentName = "BHXH",
                ComponentCode = "BHXH",
                Nature = Nature.Deduction,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.PercentOfBase,
                BaseSource = SalaryComponentBaseSource.ContractSalaryInsurance,
                Description = "Khấu trừ BHXH (8% lương đóng BH)"
            },
            new SalaryComponent
            {
                ComponentName = "BHTN",
                ComponentCode = "BHTN",
                Nature = Nature.Deduction,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.PercentOfBase,
                BaseSource = SalaryComponentBaseSource.ContractSalaryInsurance,
                Description = "Khấu trừ BHTN (1% lương đóng BH)"
            },
            new SalaryComponent
            {
                ComponentName = "BHYT",
                ComponentCode = "BHYT",
                Nature = Nature.Deduction,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.PercentOfBase,
                BaseSource = SalaryComponentBaseSource.ContractSalaryInsurance,
                Description = "Khấu trừ BHYT (1.5% lương đóng BH)"
            },
            new SalaryComponent
            {
                ComponentName = "Quỹ công đoàn",
                ComponentCode = "QUY_CONG_DOAN",
                Nature = Nature.Deduction,
                Characteristic = Characteristic.Fixed,
                CalcType = SalaryComponentCalcType.PercentOfBase,
                BaseSource = SalaryComponentBaseSource.ContractSalaryInsurance,
                Description = "Quỹ công đoàn"
            }
        };

        public async Task<PagingResult<SalaryComponentDto>> Paging(string? name, Status? status, int? organizationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.SalaryComponents.Where(s => s.IsDeleted != true).Include(s => s.Organization).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.ComponentName.Contains(name) || s.ComponentCode.Contains(name));
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status);
            }

            if (organizationId.HasValue)
            {
                query = query.Where(s => s.OrganizationId == organizationId);
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<SalaryComponentDto>(query).ToListAsync();

            var result = new PagingResult<SalaryComponentDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<List<SalaryComponentDto>> Create(List<CreateSalaryComponentRequest> requests)
        {
            // Map danh sách request sang danh sách entity
            var entities = _mapper.Map<List<SalaryComponent>>(requests);

            // Thêm danh sách entity vào db
            await CreateRangeAsync(entities);

            // Map danh sách entity sang danh sách DTO
            return _mapper.Map<List<SalaryComponentDto>>(entities);
        }

        public async Task Update(int id, UpdateSalaryComponentRequest request)
        {
            var entity = await GetSalaryComponentAndCheckExist(id);
            await UpdateAsync(_mapper.Map(request, entity));
        }

        public async Task<SalaryComponentDto> GetById(int id)
        {
            var entity = await GetSalaryComponentAndCheckExist(id);
            return _mapper.Map<SalaryComponentDto>(entity);
        }

        public async Task Delete(int id)
        {
            var entity = await GetSalaryComponentAndCheckExist(id);
            entity.IsDeleted = true;
            await UpdateAsync(entity);
        }

        private async Task<SalaryComponent> GetSalaryComponentAndCheckExist(int salaryComponentId)
        {
            var salaryComponent = await _dbContext.SalaryComponents.FindAsync(salaryComponentId);
            if (salaryComponent is null)
                throw new EntityNotFoundException(nameof(SalaryComponent), $"Id = {salaryComponentId}");
            return salaryComponent;
        }


        // Lấy list mặc định các thành phần lương chưa tồn tại trong db
        public async Task<List<SalaryComponentDto>> GetDefaultSalaryComponents()
        {
            // Lấy ComponentCode hiện có trong db
            var existingCodes = await _dbContext.SalaryComponents
                .Where(s => s.IsDeleted != true)
                .Select(s => s.ComponentCode)
                .ToListAsync();

            // Lọc ra các thành phần lương mặc định chưa tồn tại trong db
            var defaultSalaryComponents = _defaultSalaryComponents
                .Where(s => !existingCodes.Contains(s.ComponentCode))
                .ToList();

            // Map sang DTO
            return _mapper.Map<List<SalaryComponentDto>>(defaultSalaryComponents);
        }

        public async Task<List<SalaryComponent>> GetByOrganizationId(int organizationId)
        {
            return await _dbContext.SalaryComponents
                .Where(s => s.OrganizationId == organizationId && s.IsDeleted != true)
                .ToListAsync();
        }

        public async Task<bool> IsFixedCharacteristic(int id)
        {
            var salaryComponent = await GetSalaryComponentAndCheckExist(id);
            return salaryComponent.Characteristic == Characteristic.Fixed;
        }
    }
}
