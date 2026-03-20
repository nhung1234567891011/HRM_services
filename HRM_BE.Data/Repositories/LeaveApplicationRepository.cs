using AutoMapper;
using HRM_BE.Core.Constants.System;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Content.Banner;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;
using HRM_BE.Core.Models.ShiftCatalog;
using HRM_BE.Core.Models.SumaryTimeSheet;
using HRM_BE.Data.Migrations;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class LeaveApplicationRepository : RepositoryBase<LeaveApplication, int>, ILeaveApplicationRepository
    {

        private readonly IMapper _mapper;
        public LeaveApplicationRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagingResult<LeaveApplicationDto>> GetPaging(
        int? organizationId,
        int? employeeId,
        DateTime? startDate,
        DateTime? endDate,
        double? numberOfDays,
        int? typeOfLeaveId,
        string? reasonForLeave,
        string? note,
        LeaveApplicationStatus? status,
        string? sortBy,
        string? orderBy,
        int pageIndex = 1,
        int pageSize = 10,
        int currentEmployeeId = 0,
        bool isAdmin = false)
        {
            var query = _dbContext.LeaveApplications.Where(l => l.IsDeleted != true).AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(l => l.EmployeeId == currentEmployeeId
                                         || l.LeaveApplicationApprovers.Any(a => a.ApproverId == currentEmployeeId));
            }

            if (employeeId.HasValue)
            {
                query = query.Where(b => b.EmployeeId == employeeId);
            }

            if (organizationId.HasValue)
            {
                query = query.Where(b => b.OrganizationId == organizationId);
            }

            if (typeOfLeaveId.HasValue)
            {
                query = query.Where(b => b.TypeOfLeaveId == typeOfLeaveId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(b => b.StartDate >= startDate && b.EndDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(b => b.StartDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(b => b.EndDate <= endDate);
            }

            if (numberOfDays.HasValue)
            {
                query = query.Where(b => b.NumberOfDays == numberOfDays);
            }

            if (!string.IsNullOrEmpty(reasonForLeave))
            {
                query = query.Where(b => b.ReasonForLeave.Contains(reasonForLeave));
            }

            if (!string.IsNullOrEmpty(note))
            {
                query = query.Where(b => b.Note.Contains(note));
            }


            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status);
            }

            if (string.IsNullOrEmpty(orderBy) && string.IsNullOrEmpty(sortBy))
            {
                query = query.OrderByDescending(b => b.Id);
            }
            else if (string.IsNullOrEmpty(orderBy))
            {
                query = sortBy == SortByConstant.Asc
                    ? query.OrderBy(b => b.Id)
                    : query.OrderByDescending(b => b.Id);
            }
            else
            {
                query = orderBy switch
                {
                    OrderByConstant.Id when sortBy == SortByConstant.Asc => query.OrderBy(b => b.Id),
                    OrderByConstant.Id when sortBy == SortByConstant.Desc => query.OrderByDescending(b => b.Id),
                    _ => query.OrderByDescending(b => b.Id)
                };
            }

            int total = await query.CountAsync();
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var data = await _mapper.ProjectTo<LeaveApplicationDto>(query).ToListAsync();

            var result = new PagingResult<LeaveApplicationDto>(data, pageIndex, pageSize, sortBy, orderBy, total);
            return result;
        }

        public async Task<LeaveApplicationDto> GetById(int id)
        {
            var query = _dbContext.LeaveApplications.Where(l => l.Id == id).AsQueryable();
            var data = await _mapper.ProjectTo<LeaveApplicationDto>(query).FirstOrDefaultAsync();

            return data;
        }

        public async Task<bool> UpdateStatus(int id, LeaveApplicationStatus status, string? approverNote)
        {
            var leaveApplication = await _dbContext.LeaveApplications.FindAsync(id);
            if (leaveApplication is null)
                throw new EntityNotFoundException(nameof(LeaveApplication), $"Id = {id}");

            leaveApplication.Status = status;
            leaveApplication.ApproverNote = approverNote;
            await UpdateAsync(leaveApplication);
            return true;
        }

        public async Task<bool> UpdateStatusMultiple(List<int> ids, LeaveApplicationStatus status, string? approverNote)
        {
            var leaveApplications = await _dbContext.LeaveApplications
                .Where(la => ids.Contains(la.Id))
                .ToListAsync();

            if (!leaveApplications.Any())
                throw new EntityNotFoundException(nameof(LeaveApplication), $"Ids = {string.Join(", ", ids)} not found.");

            foreach (var leaveApplication in leaveApplications)
            {
                leaveApplication.Status = status;
                leaveApplication.ApproverNote = approverNote;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task Update(int id, UpdateLeaveApplicationRequest request)
        {
            var leaveApplication = await _dbContext.LeaveApplications
                .Include(l => l.LeaveApplicationApprovers)
                .Include(l => l.LeaveApplicationReplacements)
                .Include(l => l.LeaveApplicationRelatedPeople)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveApplication == null)
            {
                throw new EntityNotFoundException(nameof(LeaveApplication), $"Id = {id}");
            }

            _mapper.Map(request, leaveApplication);

            // danh sách người duyệt đơn
            leaveApplication.LeaveApplicationApprovers.Clear();
            leaveApplication.LeaveApplicationApprovers = request.ApproverIds
                .Select(id => new LeaveApplicationApprover { ApproverId = id, LeaveApplicationId = leaveApplication.Id })
                .ToList();

            // danh sách người thay thế
            leaveApplication.LeaveApplicationReplacements.Clear();
            leaveApplication.LeaveApplicationReplacements = request.ReplacementIds
                .Select(id => new LeaveApplicationReplacement { ReplacementId = id, LeaveApplicationId = leaveApplication.Id })
                .ToList();

            // danh sách người liên quan
            leaveApplication.LeaveApplicationRelatedPeople.Clear();
            leaveApplication.LeaveApplicationRelatedPeople = request.RelatedPersonIds
                .Select(id => new LeaveApplicationRelatedPerson { RelatedPersonId = id, LeaveApplicationId = leaveApplication.Id })
                .ToList();

            await UpdateAsync(leaveApplication);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TotalNumberOfDaysOffDto> GetTotalNumberOfDaysOff(DateTime? startDate, DateTime? endDate, int employeeId)
        {


            var leaveApplications = _dbContext.LeaveApplications
                .Where(l => l.EmployeeId == employeeId && l.Status == LeaveApplicationStatus.Approved &&
                            l.StartDate.HasValue && l.EndDate.HasValue &&
                            l.StartDate < endDate && l.EndDate > startDate)
                .Select(l => new
                {
                    TypeOfLeaveId = l.TypeOfLeaveId,
                    TypeOfLeaveName = l.TypeOfLeave.Name,
                    StartDate = l.StartDate.Value,
                    EndDate = l.EndDate.Value
                })
                .ToList();

            #region ignore
            //// tính ngày nghỉ đã điều chỉnh cho mỗi đơn nghỉ phép
            //var adjustedLeaves = leaveApplications.Select(l => new {
            //    l.TypeOfLeaveId,
            //    l.TypeOfLeaveName,
            //    AdjustedDays = Math.Round((EndDateAdjusted(l.EndDate, endDate.Value) - StartDateAdjusted(l.StartDate, startDate.Value)).TotalDays + 1, 2)
            //    //AdjustedDays = (EndDateAdjusted(l.EndDate, endDate.Value) - StartDateAdjusted(l.StartDate, startDate.Value)).TotalDays + 1
            //}).ToList();

            //var adjustedLeaves = leaveApplications.Select(l => new {
            //    l.TypeOfLeaveId,
            //    l.TypeOfLeaveName,
            //    StartDateAdjusted = StartDateAdjusted(l.StartDate, startDate.Value),
            //    EndDateAdjusted = EndDateAdjusted(l.EndDate, endDate.Value)
            //}).Select(l => new {
            //    l.TypeOfLeaveId,
            //    l.TypeOfLeaveName,
            //    AdjustedDays = Math.Round((l.EndDateAdjusted - l.StartDateAdjusted).TotalDays + 1 - CountSundays(l.StartDateAdjusted, l.EndDateAdjusted), 2)
            //}).ToList();

            //int CountSundays(DateTime adjustedStartDate, DateTime adjustedEndDate)
            //{
            //    int count = 0;
            //    for (DateTime date = adjustedStartDate; date <= adjustedEndDate; date = date.AddDays(1))
            //    {
            //        if (date.DayOfWeek == DayOfWeek.Sunday)
            //        {
            //            count++;
            //        }
            //    }
            //    return count;
            //}
            #endregion


            var adjustedLeaves = new List<dynamic>();

            foreach (var l in leaveApplications)
            {
                var startDateAdjusted = StartDateAdjusted(l.StartDate, startDate.Value);
                var endDateAdjusted = EndDateAdjusted(l.EndDate, endDate.Value);

                var scheduledDayOffs = await CountScheduledDayOffs(startDateAdjusted, endDateAdjusted, employeeId);

                adjustedLeaves.Add(new
                {
                    l.TypeOfLeaveId,
                    l.TypeOfLeaveName,
                    AdjustedDays = Math.Round((double)(endDateAdjusted - startDateAdjusted).TotalDays + 1 - scheduledDayOffs, 2)
                });
            }


            //gom nhóm
            var leaveSummary = adjustedLeaves
                .GroupBy(l => new { l.TypeOfLeaveId, l.TypeOfLeaveName })
                .Select(g => new
                {
                    TypeOfLeaveId = g.Key.TypeOfLeaveId,
                    TypeOfLeaveName = g.Key.TypeOfLeaveName,
                    TotalDays = g.Sum(l => (double)l.AdjustedDays)
                })
                .ToList();

            var totalNumberOfDaysOffGroupByTypeOfLeaves = new List<TotalNumberOfDaysOffGroupByTypeOfLeave>();

            foreach (var leaveSummar in leaveSummary)
            {
                var totalNumberOfDaysOffDto = new TotalNumberOfDaysOffGroupByTypeOfLeave()
                {
                    TypeOfLeaveId = leaveSummar.TypeOfLeaveId,
                    TypeOfLeaveName = leaveSummar.TypeOfLeaveName,
                    NumberOfDaysOff = Math.Round((double)leaveSummar.TotalDays, 2)
                };
                totalNumberOfDaysOffGroupByTypeOfLeaves.Add(totalNumberOfDaysOffDto);
            }

            double totalLeaveDays = Math.Round((double)leaveSummary.Sum(l => l.TotalDays), 2);

            DateTime StartDateAdjusted(DateTime startDate, DateTime filterStartDate)
            {
                return startDate < filterStartDate ? filterStartDate : startDate;
            }

            DateTime EndDateAdjusted(DateTime endDate, DateTime filterEndDate)
            {
                return endDate > filterEndDate ? filterEndDate : endDate;
            }


            return new TotalNumberOfDaysOffDto()
            {
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate,
                TotalNumberOfDaysOff = totalLeaveDays,
                TotalNumberOfDaysOffGroupByTypeOfLeaves = totalNumberOfDaysOffGroupByTypeOfLeaves
            };


        }


        public async Task<bool> DeleteSoft(int id)
        {
            var leaveApplication = await _dbContext.LeaveApplications.FindAsync(id);
            if (leaveApplication is null)
                throw new EntityNotFoundException(nameof(LeaveApplication), $"Id = {id}");

            leaveApplication.IsDeleted = true;
            await UpdateAsync(leaveApplication);
            return true;
        }


        //tính toán day nghỉ theo lịch
        public async Task<double> CountScheduledDayOffs(DateTime adjustedStartDate, DateTime adjustedEndDate, int employeeId)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException(nameof(Employee), $"Id={employeeId}");
            }

            var organizationId = employee.OrganizationId;

            var allShiftWorks = await _dbContext.ShiftWorks
                .Include(sw => sw.ShiftCatalog)
                .Where(sw => sw.OrganizationId == organizationId && sw.IsDeleted != true &&
                             sw.StartDate <= adjustedEndDate &&
                             (sw.EndDate == null || sw.EndDate >= adjustedStartDate))
                .ToListAsync();

            int count = 0;

            for (DateTime date = adjustedStartDate; date <= adjustedEndDate; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;
                var shiftWorksForDay = allShiftWorks.Where(sw =>
                    (dayOfWeek == DayOfWeek.Monday && sw.IsMonday == true) ||
                    (dayOfWeek == DayOfWeek.Tuesday && sw.IsTuesday == true) ||
                    (dayOfWeek == DayOfWeek.Wednesday && sw.IsWednesday == true) ||
                    (dayOfWeek == DayOfWeek.Thursday && sw.IsThursday == true) ||
                    (dayOfWeek == DayOfWeek.Friday && sw.IsFriday == true) ||
                    (dayOfWeek == DayOfWeek.Saturday && sw.IsSaturday == true) ||
                    (dayOfWeek == DayOfWeek.Sunday && sw.IsSunday == true))
                    .ToList();

                //  không có ca làm việc thì tăng số ngày nghỉ theo lịch 
                if (!shiftWorksForDay.Any())
                {
                    count++;
                }
            }

            return count;
        }

        public async Task<double> GetTotalLeaveEmployee(DateTime startDate, DateTime endDate, int employeeId)
        {
            var result = await _dbContext.LeaveApplications
                .Where(l => l.EmployeeId == employeeId &&
                l.Status == LeaveApplicationStatus.Approved &&
                l.StartDate < endDate && l.EndDate > startDate &&
                ( l.SalaryPercentage > 0 || l.OnPaidLeaveStatus == OnPaidLeaveStatus.Yes )).Select(s => s.NumberOfDays).SumAsync();
            return result ?? 0;
        }


    }
}

