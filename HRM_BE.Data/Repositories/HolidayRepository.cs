using AutoMapper;
using HRM_BE.Core.Constants.Contract;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Contract;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
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
    public class HolidayRepository : RepositoryBase<Holiday, int>, IHolidayRepository
    {
        private readonly IMapper _mapper;

        public HolidayRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HolidayDto> Create(CreateHolidayRequest request)
        {
            var entity = _mapper.Map<Holiday>(request);
            await CreateAsync(entity);

            await CreateDefaultWorkFactors(entity);
            return _mapper.Map<HolidayDto>(entity);
        }

        public async Task<HolidayDto> GetById(int id)
        {
            var entity = await GetHolidayAndCheckExist(id);
            return _mapper.Map<HolidayDto>(entity);
        }

        public async Task<PagingResult<HolidayDto>> Paging(int? organizationId, string? name, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Holidays.Where(g => g.IsDeleted != true).AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(g => g.OrganizationId == organizationId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(g => g.Name.Contains(name));
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<HolidayDto>(query).ToListAsync();

            var result = new PagingResult<HolidayDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task Update(int id, UpdateHolidayRequest request)
        {
            var entity = await GetHolidayAndCheckExist(id);
            await UpdateAsync(_mapper.Map(request, entity));
        }

        public async Task Delete(int id)
        {
            var entity = await GetHolidayAndCheckExist(id);

            // Xóa các WorkFactor liên quan đến Holiday này
            var workFactors = await _dbContext.WorkFactors.Where(wf => wf.HolidayId == id).ToListAsync();
            _dbContext.WorkFactors.RemoveRange(workFactors);
            await DeleteAsync(entity);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Holiday> GetHolidayAndCheckExist(int holidayId)
        {
            var holiday = await _dbContext.Holidays.FindAsync(holidayId);
            if (holiday is null)
                throw new EntityNotFoundException(nameof(Holiday), $"Id = {holidayId}");
            return holiday;
        }

        private async Task CreateDefaultWorkFactors(Holiday holiday)
        {
            var startYear = holiday.FromDate.Year;
            var endYear = holiday.ToDate.Year;

            for (int year = startYear; year <= endYear; year++)
            {
                var workFactor = new WorkFactor
                {
                    HolidayId = holiday.Id,
                    Year = year,
                    Factor = 1,
                    IsFixed = false
                };

                await _dbContext.WorkFactors.AddAsync(workFactor);
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<double> GetNumberHoliday(DateTime startDate,DateTime endDate,int organizationId)
        {
            var holidays = await _dbContext.Holidays.Where(h => h.FromDate >= startDate && h.ToDate <= endDate && h.OrganizationId == organizationId).ToListAsync();
            var total = 0;
            foreach( var holiday in holidays)
            {
                total += (holiday.ToDate - holiday.FromDate).Days + 1;
            }
            return total;
        }

        public async Task<List<DateTime>> GetDayHoliday(DateTime startDate, DateTime endDate,int employeeId)
        {
            List<DateTime> holidayByDayDtos = new List<DateTime>();
            var organizationId = await _dbContext.Employees.Where(x => x.Id == employeeId).Select(x => x.OrganizationId).FirstOrDefaultAsync();
            var contract = await _dbContext.Contracts.Where(c => c.EmployeeId == employeeId && c.ExpiredStatus != true && c.SignStatus==Core.Data.Profile.SignStatus.Signed && c.IsDeleted!=true).FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(contract?.ContractName))
            {
                return holidayByDayDtos;
            }

            var contractName = contract.ContractName.ToLower().Replace(" ", "");
            var officialContract = ContractConstant.OfficalContract.ToLower().Replace(" ", "");

            if (contractName.Contains(officialContract)) {

                var holidays = await _dbContext.Holidays.Where(h => h.FromDate.Date >= startDate.Date && h.ToDate.Date <= endDate.Date && h.OrganizationId == organizationId).ToListAsync();
                if (holidays.Count() <= 0)
                {
                    return holidayByDayDtos;
                }
                foreach (var holiday in holidays)
                {
                    //total += (holiday.ToDate - holiday.FromDate).Days + 1;
                    for (DateTime day = holiday.FromDate.Date; day <= holiday.ToDate.Date; day = day.AddDays(1))
                    {
                        holidayByDayDtos.Add(day);
                    }
                }
            }

            return holidayByDayDtos;
           
        }
    }
}
