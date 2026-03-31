using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.IServices;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Models.ShiftCatalog;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRM_BE.Data.Repositories
{
    public class TimesheetRepository : RepositoryBase<Timesheet, int>, ITimesheetRepository
    {
        private readonly IMapper _mapper;
        private readonly ITimesheetCalculationService _calculationService;

        public TimesheetRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, ITimesheetCalculationService calculationService) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
            _calculationService = calculationService;
        }

        public async Task<PagingResult<TimesheetDto>> Paging(int? employeeId, DateTime? startDate, DateTime? endDate, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Timesheets.Where(g => g.IsDeleted != true).AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(ts => ts.EmployeeId == employeeId);
            }

            if (startDate.HasValue)
            {
                query = query.Where(ts => ts.Date >= startDate);
            }

            if (endDate.HasValue) {
                query = query.Where(ts => ts.Date <= endDate);
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(orderBy ?? "Id", sortBy ?? "asc");
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<TimesheetDto>(query).ToListAsync();

            var result = new PagingResult<TimesheetDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<TimesheetDto> GetById(int id)
        {
            var entity = await GetTimesheetAndCheckExist(id);
            
            return _mapper.Map<TimesheetDto>(entity);
        }

        public async Task Update(int id, UpdateTimesheetRequest request)
        {
            var entity = await GetTimesheetAndCheckExist(id);

            _mapper.Map(request, entity);

            entity.NumberOfWorkingHour = await _calculationService.CalculateWorkingHours(entity);
            entity.OvertimeHour = await _calculationService.CalculateOvertimeHours(entity);

            if (entity.NumberOfWorkingHour > 0)
            {
                entity.TimeKeepingLeaveStatus = TimeKeepingLeaveStatus.None;
            }

            await UpdateAsync(entity);
        }

        private async Task<Timesheet> GetTimesheetAndCheckExist(int timeSheetId)
        {
            var timesheet = await _dbContext.Timesheets.FindAsync(timeSheetId);
            if (timesheet is null)
                throw new EntityNotFoundException(nameof(Timesheet), $"Id = {timeSheetId}");
            return timesheet;
        }

        public async Task AddOrUpdateTimesheetAsync(Timesheet timesheet)
        {
            var existingTimesheet = await _dbContext.Timesheets
                .FirstOrDefaultAsync(ts => ts.EmployeeId == timesheet.EmployeeId && ts.Date == timesheet.Date);

            if (existingTimesheet == null)
            {
                timesheet.NumberOfWorkingHour = await _calculationService.CalculateWorkingHours(timesheet);
                timesheet.OvertimeHour = await _calculationService.CalculateOvertimeHours(timesheet);
                await CreateAsync(timesheet);
            }
            else
            {
                existingTimesheet.StartTime = timesheet.StartTime ?? existingTimesheet.StartTime;
                existingTimesheet.EndTime = timesheet.EndTime ?? existingTimesheet.EndTime;

                existingTimesheet.NumberOfWorkingHour = await _calculationService.CalculateWorkingHours(existingTimesheet);
                existingTimesheet.OvertimeHour = await _calculationService.CalculateOvertimeHours(existingTimesheet);
                
                await UpdateAsync(existingTimesheet);
            }
        }

        public async Task<Timesheet?> GetByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            return await _dbContext.Timesheets
                .FirstOrDefaultAsync(ts => ts.EmployeeId == employeeId && ts.Date == date);
        }

        public async Task<TimesheetDurationLateOrEarlyDto> GetTimesheetDurationLateOrEarly(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _dbContext.Timesheets.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date <= endDate);
            }

            query = query.Where(x => x.EmployeeId == employeeId);

            var timesheets = await query.ToListAsync();

            var result = new TimesheetDurationLateOrEarlyDto
            {
                EmployeeId = employeeId,
                LateDurationCount = timesheets.Count(x => x.LateDuration > 0),
                EarlyLeaveDurationCount = timesheets.Count(x => x.EarlyLeaveDuration > 0),
                LateDuration = timesheets.Where(x => x.LateDuration > 0).Sum(x => x.LateDuration) ?? 0,
                EarlyLeaveDuration = timesheets.Where(x => x.EarlyLeaveDuration > 0).Sum(x => x.EarlyLeaveDuration) ?? 0,
                LateOrEarlyLeaveDurationCount= timesheets.Count(x => x.EarlyLeaveDuration > 0 || x.LateDuration > 0),
            };

            return result;
        }

        // Lấy thông tin ShiftCatalog từ ShiftWorkId
        public async Task<ShiftCatalogDto?> GetShiftCatalogByShiftWorkId(int shiftWorkId)
        {
            // Truy vấn thông tin ShiftCatalog thông qua ShiftWorkId
            var shiftWork = await _dbContext.ShiftWorks
                                .Include(s => s.ShiftCatalog)
                                .FirstOrDefaultAsync(sw => sw.Id == shiftWorkId);

            if (shiftWork == null)
            {
                return null;
            }

            return _mapper.Map<ShiftCatalogDto>(shiftWork.ShiftCatalog);
        }

        // Lấy thông tin chấm công theo nhân viên và ca làm việc cho màn chấm công GPS vào ra
        public async Task<Timesheet?> GetByEmployeeAndShiftAsync(int employeeId, int shiftWorkId, DateTime date)
        {
            return await _dbContext.Timesheets
                .FirstOrDefaultAsync(ts => ts.EmployeeId == employeeId && ts.ShiftWorkId == shiftWorkId && ts.Date.HasValue && ts.Date.Value.Date == date.Date);
        }

        public async Task<int> CreateTimesheet(CreateTimesheetRequest request)
        {
            var entity = new Timesheet
            {
                EmployeeId = request.EmployeeId,
                ShiftWorkId = request.ShiftWorkId,
                Date = request.Date.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TimeKeepingLeaveStatus = request.TimeKeepingLeaveStatus,
                LateDuration = request.LateDuration,
                EarlyLeaveDuration = request.EarlyLeaveDuration,
            };

            entity.NumberOfWorkingHour = await _calculationService.CalculateWorkingHours(entity);
            entity.OvertimeHour = await _calculationService.CalculateOvertimeHours(entity);

            await CreateAsync(entity);
            return entity.Id;
        }

        public async Task<int> RecalculateAllTimesheets(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Timesheets
                .Include(t => t.ShiftWork)
                    .ThenInclude(sw => sw.ShiftCatalog)
                .Where(t => t.IsDeleted != true);

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate);
            }

            var timesheets = await query.ToListAsync();

            int updatedCount = 0;

            foreach (var timesheet in timesheets)
            {
                var oldValue = timesheet.NumberOfWorkingHour;
                var newValue = await _calculationService.CalculateWorkingHours(timesheet);

                if (Math.Abs((oldValue ?? 0) - newValue) > 0.01)
                {
                    timesheet.NumberOfWorkingHour = newValue;
                    timesheet.OvertimeHour = await _calculationService.CalculateOvertimeHours(timesheet);

                    if (newValue > 0)
                    {
                        timesheet.TimeKeepingLeaveStatus = Core.Data.Payroll_Timekeeping.TimekeepingRegulation.TimeKeepingLeaveStatus.None;
                    }

                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _dbContext.SaveChangesAsync();
            }

            return updatedCount;
        }
    }
}
