using AutoMapper;
using HRM_BE.Core.Constants.System;
using HRM_BE.Core.Data.Content;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Content.Banner;
using HRM_BE.Core.Models.DetailTimeSheet;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Models.SumaryTimeSheet;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class SummaryTimesheetNameEmployeeConfirmRepository : RepositoryBase<SummaryTimesheetNameEmployeeConfirm, int>, ISummaryTimesheetNameEmployeeConfirmRepository
    {
        private readonly IMapper _mapper;
        public SummaryTimesheetNameEmployeeConfirmRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public Task<PagingResult<SummaryTimesheetNameEmployeeConfirmDto>> Paging(GetSummaryTimesheetNameEmployeeConfirmRequest request)
        {
            //var query = _dbContext.SummaryTimesheetNames
            //    .Select(stn => new
            //    {
            //        stn.TimekeepingSheetName,
            //        stn.OrganizationId,
            //        StartDate = stn.SummaryTimesheetNameDetailTimesheetNames
            //                    .Select(stdtn => stdtn.DetailTimesheetName.StartDate)
            //                    .Min(),
            //        EndDate = stn.SummaryTimesheetNameDetailTimesheetNames
            //                  .Select(stdtn => stdtn.DetailTimesheetName.EndDate)
            //                  .Max()
            //    }).AsQueryable();
            throw new NotImplementedException();
        }

        public async Task<PagingResult<SummaryTimesheetNameEmployeeConfirmDto>> PagingByEmployee(int? summaryTimesheetNameId, DateTime? startDate, DateTime? endDate, SummaryTimesheetNameEmployeeConfirmStatus? status, string? Note, DateTime? date, string? sortBy, string? orderBy, int employeeId, int pageIndex = 1, int pageSize = 10)
        {
            await AutoConfirmExpiredAsync();

            var query = _dbContext.SummaryTimesheetNames
                .Where(x => x.SummaryTimesheetNameEmployeeConfirms.Any() && x.SummaryTimesheetNameEmployeeConfirms.Any(y => y.EmployeeId == employeeId && y.Status != SummaryTimesheetNameEmployeeConfirmStatus.None))
                .Select(stn => new
                {
                    SummaryTimesheetName = stn,
                    MinStartDate = stn.SummaryTimesheetNameDetailTimesheetNames.Select(dtn => dtn.DetailTimesheetName.StartDate).Min(),
                    MaxEndDate = stn.SummaryTimesheetNameDetailTimesheetNames.Select(dtn => dtn.DetailTimesheetName.EndDate).Max(),
                    SummaryTimesheetNameEmployeeConfirm = stn.SummaryTimesheetNameEmployeeConfirms.Where(s => s.EmployeeId == employeeId && s.SummaryTimesheetNameId == stn.Id).FirstOrDefault()
                }).AsQueryable();

            if (summaryTimesheetNameId.HasValue)
            {
                query = query.Where(x => x.SummaryTimesheetName.Id == summaryTimesheetNameId);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.MinStartDate >= startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(x => x.MaxEndDate <= endDate);
            }

            int total = await query.CountAsync();

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(sortBy))
            {
                query = sortBy == SortByConstant.Asc ?
                    query.OrderBy(x => x.SummaryTimesheetName.Id) :
                    query.OrderByDescending(x => x.SummaryTimesheetName.Id);
            }

            //var data = await query.Select(x => _mapper.Map<SummaryTimesheetNameEmployeeConfirmDto>(x.SummaryTimesheetName)).ToListAsync();
            var data = await query.Select(x => new SummaryTimesheetNameEmployeeConfirmDto
            {
                Id = x.SummaryTimesheetName.Id,
                OrganizationId = x.SummaryTimesheetName.OrganizationId,
                StartDate = x.MinStartDate,
                EndDate = x.MaxEndDate,
                TimekeepingSheetName = x.SummaryTimesheetName.TimekeepingSheetName,
                TimekeepingMethod = x.SummaryTimesheetName.TimekeepingMethod,
                SummaryTimesheetNameEmployeeConfirm = _mapper.Map<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(x.SummaryTimesheetNameEmployeeConfirm)
            }).ToListAsync();

            var result = new PagingResult<SummaryTimesheetNameEmployeeConfirmDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto> GetStatusByEmployee(int summaryTimesheetNameId, int employeeId)
        {
            await AutoConfirmExpiredAsync();

            var query = _dbContext.SummaryTimesheetNameEmployeeConfirms.Where(x => x.SummaryTimesheetNameId == summaryTimesheetNameId && x.EmployeeId == employeeId).AsQueryable();
            var data = await _mapper.ProjectTo<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(query).FirstOrDefaultAsync();
            return data;
        }


        #region ignore paging-by-employee
        //public async Task<PagingResult<SummaryTimesheetNameEmployeeConfirmDto>> PagingByEmployee(int? summaryTimesheetNameId, DateTime? startDate, DateTime? endDate, SummaryTimesheetNameEmployeeConfirmStatus? status, string? Note, DateTime? date, string? sortBy, string? orderBy,int employeeId, int pageIndex = 1, int pageSize = 10)
        //{


        //    //var query = _dbContext.SummaryTimesheetNames.Where(x=>x.SummaryTimesheetNameEmployeeConfirms.Any() && x.SummaryTimesheetNameEmployeeConfirms.Any(y=>y.EmployeeId==employeeId)).AsQueryable();

        //    var query = _dbContext.SummaryTimesheetNames
        //      .Where(x => x.SummaryTimesheetNameEmployeeConfirms.Any() && x.SummaryTimesheetNameEmployeeConfirms.Any(y => y.EmployeeId == employeeId))
        //      .Select(stn => new {
        //          SummaryTimesheetName = stn,
        //          MinStartDate = stn.SummaryTimesheetNameDetailTimesheetNames.Select(dtn => dtn.DetailTimesheetName.StartDate).Min(),
        //          MaxEndDate = stn.SummaryTimesheetNameDetailTimesheetNames.Select(dtn => dtn.DetailTimesheetName.EndDate).Max()
        //      }).AsQueryable();

        //    if (summaryTimesheetNameId.HasValue)
        //    {
        //        query=query.Where(x=>x.SummaryTimesheetName.Id == summaryTimesheetNameId);
        //    }

        //    int total = await query.CountAsync();

        //    if (pageIndex == null) pageIndex = 1;
        //    if (pageSize == null) pageSize = total;


        //    if (string.IsNullOrEmpty(orderBy) && string.IsNullOrEmpty(sortBy))
        //    {
        //        query = query.OrderByDescending(b => b.Id);
        //    }
        //    else if (string.IsNullOrEmpty(orderBy))
        //    {
        //        if (sortBy == SortByConstant.Asc)
        //        {
        //            query = query.OrderBy(b => b.Id);
        //        }
        //        else
        //        {
        //            query = query.OrderByDescending(b => b.Id);
        //        }
        //    }
        //    else if (string.IsNullOrEmpty(sortBy))
        //    {
        //        query = query.OrderByDescending(b => b.Id);
        //    }
        //    else
        //    {
        //        if (orderBy == OrderByConstant.Id && sortBy == SortByConstant.Asc)
        //        {
        //            query = query.OrderBy(b => b.Id);
        //        }
        //        else if (orderBy == OrderByConstant.Id && sortBy == SortByConstant.Desc)
        //        {
        //            query = query.OrderByDescending(b => b.Id);
        //        }
        //    }

        //    query = query
        //    .Skip((pageIndex - 1) * pageSize)
        //    .Take(pageSize);


        //    var data = await _mapper.ProjectTo<SummaryTimesheetNameEmployeeConfirmDto>(query).ToListAsync();


        //    var result = new PagingResult<SummaryTimesheetNameEmployeeConfirmDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

        //    return result;


        //    //if (request.EmployeeId.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}
        //    //if (request.StartDate.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}
        //    //if (request.EndDate.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}
        //    //if (request.Status.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}
        //    //if (request.Note.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}
        //    //if (request.Date.HasValue)
        //    //{
        //    //    query = query.Where(x => x.Id == request.SummaryTimesheetNameId);
        //    //}



        //}

        #endregion


        public Task<SummaryTimesheetNameEmployeeConfirmDto> CheckExist(GetSummaryTimesheetNameEmployeeConfirmRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto> CreateOrUpdate(CreateSummaryTimesheetNameEmployeeConfirmRequest request)
        {
            await AutoConfirmExpiredAsync();

            var existingRecord = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                .FirstOrDefaultAsync(x => x.SummaryTimesheetNameId == request.SummaryTimesheetNameId
                                       && x.EmployeeId == request.EmployeeId);

            if (existingRecord == null)
            {
                var newRecord = _mapper.Map<SummaryTimesheetNameEmployeeConfirm>(request);

                _dbContext.SummaryTimesheetNameEmployeeConfirms.Add(newRecord);
                await CreateAsync(newRecord);

                return _mapper.Map<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(newRecord);
            }
            else
            {
                existingRecord.Status = request.Status ?? existingRecord.Status;
                existingRecord.Note = request.Note ?? existingRecord.Note;

                await UpdateAsync(existingRecord);

                return _mapper.Map<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(existingRecord);
            }
        }

        public async Task<List<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>> CreateOrUpdateMultiple(CreateSummaryTimesheetNameEmployeeConfirmMultipleRequest request)
        {
            await AutoConfirmExpiredAsync();

            var resultList = new List<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>();

            foreach (var employeeId in request.EmployeeIds ?? new List<int>())
            {
                var existingRecord = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                    .FirstOrDefaultAsync(x => x.SummaryTimesheetNameId == request.SummaryTimesheetNameId
                                           && x.EmployeeId == employeeId && x.Status != SummaryTimesheetNameEmployeeConfirmStatus.Confirm);

                if (existingRecord == null)
                {
                    var newRecord = new SummaryTimesheetNameEmployeeConfirm
                    {
                        SummaryTimesheetNameId = request.SummaryTimesheetNameId,
                        EmployeeId = employeeId,
                        Status = request.Status ?? SummaryTimesheetNameEmployeeConfirmStatus.None,
                        Note = request.Note,
                        Date = request.Date
                    };

                    await CreateAsync(newRecord);

                    resultList.Add(_mapper.Map<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(newRecord));
                }
                else
                {
                    existingRecord.Status = request.Status ?? existingRecord.Status;
                    existingRecord.Note = request.Note ?? existingRecord.Note;

                    await UpdateAsync(existingRecord);

                    resultList.Add(_mapper.Map<SummaryTimesheetNameSummaryTimesheetNameEmployeeConfirmDto>(existingRecord));
                }
            }

            return resultList;
        }

        public async Task<List<SummaryTimesheetNameEmployeeConfirmTimeSheetDto>> GetDetail(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _dbContext.Timesheets.Where(x => x.EmployeeId == employeeId).AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date.Value.Date <= endDate.Value.Date);
            }

            var data = await _mapper.ProjectTo<SummaryTimesheetNameEmployeeConfirmTimeSheetDto>(query).ToListAsync();

            return data;

        }


        //hàm vip pro max
        public async Task<List<ConfirmTimeSheetDto>> GetDetailByShiftWork(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _dbContext.Timesheets
                .Include(t => t.ShiftWork)
                .ThenInclude(s => s.ShiftCatalog)
                .Where(x => x.EmployeeId == employeeId && x.IsDeleted != true)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date.Value.Date <= endDate.Value.Date);
            }

            //((t.EndTime?.TotalHours ?? 0) - (t.StartTime?.TotalHours ?? 0))

            var timesheets = query;

            var groupedByDate = await timesheets
                .GroupBy(t => t.Date != null ? t.Date.Value.Date : (DateTime?)null) 
                .Select(g => new ConfirmTimeSheetDto
                {
                    Date = g.Key,
                    Shifts = g.Select(t => new ShiftDetailDto
                    {
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        ShiftWorkId = t.ShiftWorkId,
                        NumberOfWorkingHour = t.NumberOfWorkingHour,
                        TimekeepingType = t.TimekeepingType,
                        ShiftTableName = t.ShiftWork.ShiftTableName,
                        IsEnoughWork = t.NumberOfWorkingHour >= (t.ShiftWork.ShiftCatalog.WorkingHours ?? 0) ? true : false,
                        TimeKeepingLeaveStatus=t.TimeKeepingLeaveStatus
                    }).ToList(),
                })
                .ToListAsync();

            return groupedByDate;
        }

        public async Task<List<PermittedLeaveDto>> GetPermittedLeaves(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.PermittedLeaves)
                .Include(e => e.Contracts) 
                .FirstOrDefaultAsync(x => x.Id == employeeId);

            if (employee == null)
            {
                throw new EntityNotFoundException(nameof(Employee),$"Id={employeeId}"); 
            }

            var permittedLeaves = employee.PermittedLeaves
                .Where(p =>
                    p.StartDate.Value.Date <= endDate.Value.Date &&
                    p.EndDate.Value.Date >= startDate.Value.Date &&
                    p.Status == LeaveApplicationStatus.Approved &&
                    p.EmployeeId == employee.Id &&
                    employee.Contracts
                        .Where(c => c.ExpiredStatus == false) 
                        .Any(c =>
                            (c.EffectiveDate.Value.Date.Day == 1
                                ? c.EffectiveDate.Value.Date
                                : new DateTime(c.EffectiveDate.Value.Year, c.EffectiveDate.Value.Month, 1).AddMonths(1)) >= p.StartDate.Value.Date &&
                            (c.EffectiveDate.Value.Date.Day == 1
                                ? c.EffectiveDate.Value.Date
                                : new DateTime(c.EffectiveDate.Value.Year, c.EffectiveDate.Value.Month, 1).AddMonths(1)) <= p.EndDate.Value.Date)
                )
                .OrderBy(p => p.CreatedAt) 
                .Take(1)
                .Select(p => new PermittedLeaveDto
                {
                    Id = p.Id,
                    //Date = p.StartDate.Value,
                    NumberOfDays = p.NumberOfDays.Value
                })
                .ToList();

            return permittedLeaves;
        }

        private async Task AutoConfirmExpiredAsync()
        {
            var now = DateTime.Now;

            var expiredRecords = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                .Where(x => x.Date.HasValue
                            && x.Date.Value <= now
                            && x.Status.HasValue
                            && x.Status != SummaryTimesheetNameEmployeeConfirmStatus.Confirm
                            && x.Status != SummaryTimesheetNameEmployeeConfirmStatus.Reject
                            && x.Status != SummaryTimesheetNameEmployeeConfirmStatus.None)
                .ToListAsync();

            if (!expiredRecords.Any())
            {
                return;
            }

            expiredRecords.ForEach(x => x.Status = SummaryTimesheetNameEmployeeConfirmStatus.Confirm);
            await _dbContext.SaveChangesAsync();
        }


    }
}
