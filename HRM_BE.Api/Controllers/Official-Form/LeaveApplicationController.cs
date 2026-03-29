using AutoMapper;
using Hangfire;
using HRM_BE.Api.Services;
using HRM_BE.Api.Extension;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Constants.Identity;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Content.Banner;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Data;
using HRM_BE.Data.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace HRM_BE.Api.Controllers.Official_Form
{
    [Route("api/leave-application")]
    [ApiController]
    public class LeaveApplicationController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;
        private readonly HrmContext _dbContext;

        public LeaveApplicationController(IUnitOfWork unitOfWork, IMapper mapper,HrmContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dbContext = context;
        }

        private int GetCurrentEmployeeId()
        {
            var claimValue = ((System.Security.Claims.ClaimsIdentity)User.Identity).GetSpecificClaim(ClaimTypeConstant.EmployeeId);
            return int.TryParse(claimValue, out var employeeId) ? employeeId : 0;
        }

        private bool IsCurrentUserAdmin()
        {
            return User.Claims.Any(c => c.Type == ClaimTypeConstant.Permission && c.Value == PermissionConstant.Admin);
        }

        private static List<int> NormalizeEmployeeIds(IEnumerable<int>? employeeIds)
        {
            return (employeeIds ?? Enumerable.Empty<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();
        }

        private async Task<List<int>> GetInvalidEmployeeIdsAsync(IEnumerable<int> employeeIds, int? organizationId)
        {
            var normalizedIds = NormalizeEmployeeIds(employeeIds);
            if (normalizedIds.Count == 0)
            {
                return new List<int>();
            }

            var validEmployeeIds = await _dbContext.Employees
                .Where(e => normalizedIds.Contains(e.Id)
                    && e.IsDeleted != true
                    && (!organizationId.HasValue || e.OrganizationId == organizationId))
                .Select(e => e.Id)
                .ToListAsync();

            return normalizedIds.Except(validEmployeeIds).ToList();
        }

        /// <summary>
        /// HRM - Là Nhân viên,HR tôi muốn xem danh sách đơn xin nghỉ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<LeaveApplicationDto>>> GetPaging([FromQuery] GetLeaveApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<PagingResult<LeaveApplicationDto>>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var isAdmin = IsCurrentUserAdmin();
            var forApproval = request.ForApproval == true;

            // Ở màn "Đơn xin nghỉ", luôn khóa theo nhân viên hiện tại để tránh xem chéo dữ liệu bằng query param.
            if (!isAdmin && !forApproval)
            {
                request.EmployeeId = currentEmployeeId;
            }

            var result = await _unitOfWork.LeaveApplications.GetPaging(
                request.OrganizationId,
                request.EmployeeId,
                request.StartDate,
                request.EndDate,
                request.NumberOfDays,
                request.TypeOfLeaveId,
                request.ReasonForLeave,
                request.Note,
                request.Status,
                request.SortBy,
                request.OrderBy,
                request.PageIndex,
                request.PageSize,
                currentEmployeeId,
                isAdmin,
                forApproval);
            return new ApiResult<PagingResult<LeaveApplicationDto>>("Danh sách đơn xin nghỉ đã được lấy thành công!", result);
        }

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<ApiResult<LeaveApplicationDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var isAdmin = IsCurrentUserAdmin();
            var canView = await _dbContext.LeaveApplications.AnyAsync(l =>
                l.Id == request.Id &&
                l.IsDeleted != true &&
                (isAdmin ||
                 l.EmployeeId == currentEmployeeId ||
                 l.LeaveApplicationApprovers.Any(a => a.ApproverId == currentEmployeeId) ||
                 l.LeaveApplicationReplacements.Any(r => r.ReplacementId == currentEmployeeId)));

            if (!canView)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Bạn không có quyền xem đơn xin nghỉ này.");
            }

            var result = await _unitOfWork.LeaveApplications.GetById(request.Id);
            return new ApiResult<LeaveApplicationDto>("Đơn xin nghỉ đã được lấy thành công!", result);
        }

        /// <summary>
        /// HRM- Là nhân viên tôi muốn xem tổng số ngày nghỉ theo tổng thời gian
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-total-number-of-days-off")]
        public async Task<ApiResult<TotalNumberOfDaysOffDto>> GetTotalNumberOfDaysOff([FromQuery] GetTotalNumberOfDaysOffRequest request)
        {
            var result = await _unitOfWork.LeaveApplications.GetTotalNumberOfDaysOff(request.StartDate,request.EndDate,request.EmployeeId);
            return new ApiResult<TotalNumberOfDaysOffDto>("Tổng số ngày nghỉ đã được lấy thành công!", result);
        }

        /// <summary>
        /// HRM- Xem số ngày nghỉ mà theo lịch đi làm được nghỉ trong khoảng thời gian
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-count-scheduled-day-offs")]
        public async Task<ApiResult<double>> CountScheduledDayOffs([FromQuery] GetTotalNumberOfDaysOffRequest request)
        {
            var result = await _unitOfWork.LeaveApplications.CountScheduledDayOffs((DateTime)request.StartDate, (DateTime)request.EndDate, request.EmployeeId);
            return new ApiResult<double>("Tổng số ngày nghỉ theo lịch lấy thành công!", result);
        }

        /// <summary>
        /// HRM- Là nhân viên tôi muốn tạo đơn xin nghỉ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create")]
        public async Task<ApiResult<LeaveApplicationDto>> Create([FromBody] CreateLeaveApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var employee = await _dbContext.Employees
                .Where(e => e.Id == currentEmployeeId)
                .Select(e => new { e.Id, e.OrganizationId })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Không tìm thấy thông tin nhân viên đăng nhập.");
            }

            request.EmployeeId = employee.Id;
            request.OrganizationId = employee.OrganizationId;

            request.ApproverIds = NormalizeEmployeeIds(request.ApproverIds);
            request.ReplacementIds = NormalizeEmployeeIds(request.ReplacementIds);
            request.RelatedPersonIds = NormalizeEmployeeIds(request.RelatedPersonIds);

            if (request.ApproverIds.Count == 0)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Đơn xin nghỉ cần có ít nhất một người duyệt.");
            }

            if (request.ApproverIds.Contains(employee.Id) || request.ReplacementIds.Contains(employee.Id))
            {
                return ApiResult<LeaveApplicationDto>.Failure("Người tạo đơn không thể là người duyệt hoặc người thay thế.");
            }

            var overlapApproverReplacementIds = request.ApproverIds.Intersect(request.ReplacementIds).ToList();
            if (overlapApproverReplacementIds.Count > 0)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Người duyệt và người thay thế không được trùng nhau.");
            }

            var invalidEmployeeIds = await GetInvalidEmployeeIdsAsync(
                request.ApproverIds
                    .Concat(request.ReplacementIds)
                    .Concat(request.RelatedPersonIds),
                null);
            if (invalidEmployeeIds.Count > 0)
            {
                return ApiResult<LeaveApplicationDto>.Failure($"Có nhân viên không hợp lệ trong danh sách người duyệt/thay thế/liên quan: {string.Join(", ", invalidEmployeeIds)}");
            }

            if (request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest == null)
            {
                return ApiResult<LeaveApplicationDto>.Failure("Thiếu thông tin số ngày phép để tạo đơn xin nghỉ.");
            }

            request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.EmployeeId = employee.Id;

            var status = await _unitOfWork.TypeOfLeaveEmployee.CheckDaysRemaining(request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.DaysRemaining, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.EmployeeId, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.TypeOfLeaveId, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.Year);
            if(status == false)
            {
                return ApiResult<LeaveApplicationDto>.Failure($"Số ngày nghỉ vượt quá số ngày nghỉ cho phép của loại nghỉ phép {request.TypeOfLeaveName} trong năm {request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.Year.ToString()}");

            }
            var leaveApplication = _mapper.Map<LeaveApplication>(request);

            var result = await _unitOfWork.LeaveApplications.CreateAsync(leaveApplication);
            await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.DaysRemaining, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.EmployeeId, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.TypeOfLeaveId, request.UpdateDaysRemainingTypeOfLeaveEmployeeRequest.Year);

            // Check số lượng nghỉ và đổi trạng thái nghỉ phép
            //await _unitOfWork.LeavePermissions.TriggerUpdateNumberLeavePermission(result.EmployeeId.Value, result.Id, result.StartDate.Value);

            return ApiResult<LeaveApplicationDto>.Success("Tạo đơn xin nghỉ việc thành công", _mapper.Map<LeaveApplicationDto>(result));
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ApiResult<bool>> Update(int id, [FromBody] UpdateLeaveApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var leaveApplication = await _dbContext.LeaveApplications
                .Where(l => l.Id == id && l.IsDeleted != true)
                .Select(l => new
                {
                    l.EmployeeId,
                    l.Status,
                    l.OrganizationId,
                    l.TypeOfLeaveId,
                    l.NumberOfDays,
                    l.StartDate
                })
                .FirstOrDefaultAsync();

            if (leaveApplication == null)
            {
                return ApiResult<bool>.Failure("Không tìm thấy đơn xin nghỉ.");
            }

            if (leaveApplication.EmployeeId != currentEmployeeId)
            {
                return ApiResult<bool>.Failure("Bạn không có quyền cập nhật đơn xin nghỉ này.");
            }

            if (leaveApplication.Status != LeaveApplicationStatus.Pending)
            {
                return ApiResult<bool>.Failure("Chỉ có thể cập nhật đơn xin nghỉ ở trạng thái chờ xác nhận.");
            }

            request.ApproverIds = NormalizeEmployeeIds(request.ApproverIds);
            request.ReplacementIds = NormalizeEmployeeIds(request.ReplacementIds);
            request.RelatedPersonIds = NormalizeEmployeeIds(request.RelatedPersonIds);

            if (request.ApproverIds.Count == 0)
            {
                return ApiResult<bool>.Failure("Đơn xin nghỉ cần có ít nhất một người duyệt.");
            }

            if (request.ApproverIds.Contains(currentEmployeeId) || request.ReplacementIds.Contains(currentEmployeeId))
            {
                return ApiResult<bool>.Failure("Người tạo đơn không thể là người duyệt hoặc người thay thế.");
            }

            var overlapApproverReplacementIds = request.ApproverIds.Intersect(request.ReplacementIds).ToList();
            if (overlapApproverReplacementIds.Count > 0)
            {
                return ApiResult<bool>.Failure("Người duyệt và người thay thế không được trùng nhau.");
            }

            var invalidEmployeeIds = await GetInvalidEmployeeIdsAsync(
                request.ApproverIds
                    .Concat(request.ReplacementIds)
                    .Concat(request.RelatedPersonIds),
                null);
            if (invalidEmployeeIds.Count > 0)
            {
                return ApiResult<bool>.Failure($"Có nhân viên không hợp lệ trong danh sách người duyệt/thay thế/liên quan: {string.Join(", ", invalidEmployeeIds)}");
            }

            request.EmployeeId = leaveApplication.EmployeeId;
            request.OrganizationId = leaveApplication.OrganizationId;

            if (!request.TypeOfLeaveId.HasValue || !request.NumberOfDays.HasValue || !request.StartDate.HasValue)
            {
                return ApiResult<bool>.Failure("Thiếu thông tin loại nghỉ/số ngày nghỉ/ngày bắt đầu để cập nhật đơn xin nghỉ.");
            }

            if (!leaveApplication.TypeOfLeaveId.HasValue || !leaveApplication.NumberOfDays.HasValue || !leaveApplication.StartDate.HasValue)
            {
                return ApiResult<bool>.Failure("Đơn xin nghỉ hiện tại thiếu dữ liệu để đối soát số ngày nghỉ còn lại.");
            }

            var employeeId = leaveApplication.EmployeeId!.Value;
            var oldTypeOfLeaveId = leaveApplication.TypeOfLeaveId.Value;
            var oldNumberOfDays = leaveApplication.NumberOfDays.Value;
            var oldYear = leaveApplication.StartDate.Value.Year;

            var newTypeOfLeaveId = request.TypeOfLeaveId.Value;
            var newNumberOfDays = request.NumberOfDays.Value;
            var newYear = request.StartDate.Value.Year;

            await _unitOfWork.TypeOfLeaveEmployee.GetOrCreate(employeeId, oldTypeOfLeaveId, oldYear);
            await _unitOfWork.TypeOfLeaveEmployee.GetOrCreate(employeeId, newTypeOfLeaveId, newYear);

            // Hoàn lại số ngày đã giữ chỗ của đơn cũ trước khi kiểm tra/quy đổi sang dữ liệu mới.
            await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                -oldNumberOfDays,
                employeeId,
                oldTypeOfLeaveId,
                oldYear);

            var hasEnoughDays = await _unitOfWork.TypeOfLeaveEmployee.CheckDaysRemaining(
                newNumberOfDays,
                employeeId,
                newTypeOfLeaveId,
                newYear);

            if (!hasEnoughDays)
            {
                await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                    oldNumberOfDays,
                    employeeId,
                    oldTypeOfLeaveId,
                    oldYear);

                return ApiResult<bool>.Failure("Số ngày nghỉ vượt quá số ngày nghỉ còn lại theo loại nghỉ.");
            }

            await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                newNumberOfDays,
                employeeId,
                newTypeOfLeaveId,
                newYear);

            try
            {
                await _unitOfWork.LeaveApplications.Update(id, request);
            }
            catch
            {
                await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                    -newNumberOfDays,
                    employeeId,
                    newTypeOfLeaveId,
                    newYear);

                await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                    oldNumberOfDays,
                    employeeId,
                    oldTypeOfLeaveId,
                    oldYear);
                throw;
            }

            return ApiResult<bool>.Success("Cập nhật đơn xin nghỉ thành công", true);
        }
        /// <summary>
        /// HRM- Là HR tôi muốn duyệt đơn xin nghỉ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-status")]
        public async Task<ApiResult<bool>> UpdateStatus(int id, [FromBody] UpdateStatusLeaveApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var canApprove = await _dbContext.LeaveApplications
                .AnyAsync(l => l.Id == id
                    && l.IsDeleted != true
                    && (
                        l.LeaveApplicationApprovers.Any(a => a.ApproverId == currentEmployeeId)
                        || l.LeaveApplicationReplacements.Any(r => r.ReplacementId == currentEmployeeId)
                    ));
            if (!canApprove)
            {
                return ApiResult<bool>.Failure("Chỉ người duyệt hoặc người thay thế mới có thể duyệt đơn xin nghỉ.");
            }

            if (request.Status != LeaveApplicationStatus.Approved && request.Status != LeaveApplicationStatus.Rejected)
            {
                return ApiResult<bool>.Failure("Trạng thái duyệt không hợp lệ.");
            }

            var leaveApplicationSnapshot = await _dbContext.LeaveApplications
                .Where(l => l.Id == id && l.IsDeleted != true)
                .Select(l => new
                {
                    l.Status,
                    l.EmployeeId,
                    l.TypeOfLeaveId,
                    l.NumberOfDays,
                    l.StartDate
                })
                .FirstOrDefaultAsync();

            if (leaveApplicationSnapshot == null)
            {
                return ApiResult<bool>.Failure("Không tìm thấy đơn xin nghỉ.");
            }

            if (leaveApplicationSnapshot.Status != LeaveApplicationStatus.Pending)
            {
                return ApiResult<bool>.Failure("Chỉ có thể duyệt đơn xin nghỉ ở trạng thái chờ duyệt.");
            }

            if (request.Status == LeaveApplicationStatus.Rejected)
            {
                if (!leaveApplicationSnapshot.EmployeeId.HasValue
                    || !leaveApplicationSnapshot.TypeOfLeaveId.HasValue
                    || !leaveApplicationSnapshot.NumberOfDays.HasValue
                    || !leaveApplicationSnapshot.StartDate.HasValue)
                {
                    return ApiResult<bool>.Failure("Đơn xin nghỉ thiếu dữ liệu để hoàn tác số ngày phép.");
                }

                await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                    -leaveApplicationSnapshot.NumberOfDays.Value,
                    leaveApplicationSnapshot.EmployeeId.Value,
                    leaveApplicationSnapshot.TypeOfLeaveId.Value,
                    leaveApplicationSnapshot.StartDate.Value.Year);
            }
            await _unitOfWork.LeaveApplications.UpdateStatus(id, request.Status, request.ApproverNote);
            var leaveApplication = await _unitOfWork.LeaveApplications.GetById(id);
            // xử lý trừ số ngày nghỉ trong bảng LeavePermission
            if (request.Status == LeaveApplicationStatus.Approved && leaveApplication.OnPaidLeaveStatus == OnPaidLeaveStatus.Yes)
            {
                //await _unitOfWork.LeavePermissions.TriggerUpdateNumberLeavePermission(leaveApplication.EmployeeId.Value, leaveApplication.NumberOfDays.Value, leaveApplication.StartDate.Value, leaveApplication.EndDate.Value);


                var employeeId = leaveApplication.EmployeeId.Value;
                var startDate = leaveApplication.StartDate.Value;
                var endDate = leaveApplication.EndDate.Value;
                var numberOfDay = leaveApplication.NumberOfDays.Value;

                var leavePermission = await _dbContext.LeavePermissions.Where(l => l.EmployeeId == employeeId).FirstOrDefaultAsync();

                if (leavePermission == null)
                    throw new EntityNotFoundException("LeavePerrmission không tìm thấy");

                var employee = await _dbContext.Employees.FindAsync(employeeId);
                var organizationId = employee.OrganizationId;

                var allShiftWorks = await _dbContext.ShiftWorks
                    .Include(sw => sw.ShiftCatalog)
                    .Where(sw => sw.OrganizationId == organizationId && sw.IsDeleted != true &&
                                 sw.StartDate <= startDate &&
                                  sw.EndDate >= endDate)
                    .ToListAsync();
                if (leavePermission.NumerOfLeave >= numberOfDay)
                {
                    leavePermission.NumerOfLeave -= numberOfDay;
                    
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
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
                        var shiftWork = shiftWorksForDay.FirstOrDefault();

                        //   có ca làm việc thì tự động chấm công 
                        if (shiftWorksForDay.Any())
                        {
                            if (shiftWork != null)
                            {
                                BackgroundJob.Enqueue<JobHangFireService>(j => j.CreateTimeSheet(employeeId,date,shiftWork.Id,shiftWork.ShiftCatalog.WorkingHours.Value, TimeKeepingLeaveStatus.LeavePermission));
                            }

                        }
                    }
                }
                else
                {
                    // Số phép còn lại nhỏ hơn số ngày nghỉ
                    var remainingLeaveDays = leavePermission.NumerOfLeave;
                    leavePermission.NumerOfLeave = 0;

                    // Lặp qua từng ngày trong khoảng từ startDate đến endDate
                    for (DateTime date = startDate.Date; date <= endDate.Date && remainingLeaveDays > 0; date = date.AddDays(1))
                    {
                        // Kiểm tra ngày có ca làm việc không
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

                        // Nếu có ca làm việc
                        if (shiftWorksForDay.Any())
                        {
                            var shiftWork = shiftWorksForDay.FirstOrDefault();
                            if (shiftWork != null)
                            {
                                // Tạo schedule cho ngày này
                                BackgroundJob.Enqueue<JobHangFireService>(
                                    j => j.CreateTimeSheet(employeeId, date, shiftWork.Id, shiftWork.ShiftCatalog.WorkingHours.Value, TimeKeepingLeaveStatus.LeavePermission)
                                );

                                // Giảm số phép còn lại
                                remainingLeaveDays--;
                            }
                        }

                        // Dừng lặp nếu số phép đã hết
                        if (remainingLeaveDays <= 0)
                        {
                            break;
                        }
                    }
                }   
                    await _dbContext.SaveChangesAsync();
            }
            else if (request.Status == LeaveApplicationStatus.Approved && leaveApplication.OnPaidLeaveStatus == OnPaidLeaveStatus.No)
            {
                var employeeId = leaveApplication.EmployeeId.Value;
                var startDate = leaveApplication.StartDate.Value;
                var endDate = leaveApplication.EndDate.Value;
                var numberOfDay = leaveApplication.NumberOfDays.Value;

                //var leavePermission = await _dbContext.LeavePermissions.Where(l => l.EmployeeId == employeeId).FirstOrDefaultAsync();

                //if (leavePermission == null)
                //    throw new EntityNotFoundException("LeavePerrmission không tìm thấy");

                var employee = await _dbContext.Employees.FindAsync(employeeId);
                var organizationId = employee.OrganizationId;

                var allShiftWorks = await _dbContext.ShiftWorks
                    .Include(sw => sw.ShiftCatalog)
                    .Where(sw => sw.OrganizationId == organizationId && sw.IsDeleted != true &&
                                 sw.StartDate <= startDate &&
                                 sw.EndDate >= endDate)
                    .ToListAsync();
                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
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
                    var shiftWork = shiftWorksForDay.FirstOrDefault();

                    //   có ca làm việc thì tự động chấm công 
                    if (shiftWorksForDay.Any())
                    {
                        if (shiftWork != null)
                        {
                            if(leaveApplication.SalaryPercentage > 0 )
                            {
                                BackgroundJob.Enqueue<JobHangFireService>(j => j.CreateTimeSheet(employeeId, date, shiftWork.Id, shiftWork.ShiftCatalog.WorkingHours.Value, TimeKeepingLeaveStatus.LeaveNotPermissionWithSalary));
                            }
                            else
                            {
                                BackgroundJob.Enqueue<JobHangFireService>(j => j.CreateTimeSheet(employeeId, date, shiftWork.Id, shiftWork.ShiftCatalog.WorkingHours.Value, TimeKeepingLeaveStatus.LeaveNotPermission));
                            }

                        }

                    }
                }
            }
            return ApiResult<bool>.Success("Cập nhật trạng thái đơn xin nghỉ thành công", true);
        }


        /// <summary>
        /// HRM- Là HR tôi muốn duyệt nhiều đơn xin nghỉ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-status-multiple")]
        public async Task<ApiResult<bool>> UpdateStatusMultiple([FromBody] UpdateStatusLeaveApplicationMultipleRequest request)
        {
            if (request.UpdateStatusLeaveApplicationRequests == null || request.UpdateStatusLeaveApplicationRequests.Count == 0)
            {
                return ApiResult<bool>.Failure("Danh sách đơn cần duyệt không được để trống.");
            }

            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            if (request.UpdateStatusLeaveApplicationRequests.Any(x => x.Status != LeaveApplicationStatus.Approved && x.Status != LeaveApplicationStatus.Rejected))
            {
                return ApiResult<bool>.Failure("Có trạng thái duyệt không hợp lệ trong danh sách.");
            }

            var leaveApplicationIds = request.UpdateStatusLeaveApplicationRequests
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            var leaveApplicationSnapshots = await _dbContext.LeaveApplications
                .Where(l => leaveApplicationIds.Contains(l.Id) && l.IsDeleted != true)
                .Select(l => new
                {
                    l.Id,
                    l.Status,
                    l.EmployeeId,
                    l.TypeOfLeaveId,
                    l.NumberOfDays,
                    l.StartDate
                })
                .ToListAsync();

            if (leaveApplicationSnapshots.Count != leaveApplicationIds.Count)
            {
                return ApiResult<bool>.Failure("Có đơn xin nghỉ không tồn tại hoặc đã bị xóa.");
            }

            var nonPendingIds = leaveApplicationSnapshots
                .Where(l => l.Status != LeaveApplicationStatus.Pending)
                .Select(l => l.Id)
                .ToList();

            if (nonPendingIds.Count > 0)
            {
                return ApiResult<bool>.Failure("Chỉ có thể duyệt các đơn đang ở trạng thái chờ duyệt.");
            }

            var approvableIds = await _dbContext.LeaveApplications
                .Where(l => leaveApplicationIds.Contains(l.Id)
                            && l.IsDeleted != true
                            && (
                                l.LeaveApplicationApprovers.Any(a => a.ApproverId == currentEmployeeId)
                                || l.LeaveApplicationReplacements.Any(r => r.ReplacementId == currentEmployeeId)
                            ))
                .Select(l => l.Id)
                .Distinct()
                .ToListAsync();

            if (leaveApplicationIds.Except(approvableIds).Any())
            {
                return ApiResult<bool>.Failure("Chỉ người duyệt hoặc người thay thế mới có thể duyệt đơn xin nghỉ.");
            }

            var leaveApplicationSnapshotById = leaveApplicationSnapshots
                .ToDictionary(x => x.Id, x => x);

            foreach (var item in request.UpdateStatusLeaveApplicationRequests)
            {
                if (item.Status == LeaveApplicationStatus.Rejected)
                {
                    var leaveApplicationSnapshot = leaveApplicationSnapshotById[item.Id];

                    if (!leaveApplicationSnapshot.EmployeeId.HasValue
                        || !leaveApplicationSnapshot.TypeOfLeaveId.HasValue
                        || !leaveApplicationSnapshot.NumberOfDays.HasValue
                        || !leaveApplicationSnapshot.StartDate.HasValue)
                    {
                        return ApiResult<bool>.Failure("Đơn xin nghỉ thiếu dữ liệu để hoàn tác số ngày phép.");
                    }

                    await _unitOfWork.TypeOfLeaveEmployee.UpdateDaysRemaining(
                        -leaveApplicationSnapshot.NumberOfDays.Value,
                        leaveApplicationSnapshot.EmployeeId.Value,
                        leaveApplicationSnapshot.TypeOfLeaveId.Value,
                        leaveApplicationSnapshot.StartDate.Value.Year);
                }
                await _unitOfWork.LeaveApplications.UpdateStatus(item.Id, item.Status, item.ApproverNote);
            }

            return ApiResult<bool>.Success("Cập nhật trạng thái các đơn xin nghỉ thành công", true);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<ApiResult<bool>> Delete(int id)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var leaveApplication = await _dbContext.LeaveApplications
                .Where(l => l.Id == id && l.IsDeleted != true)
                .Select(l => new { l.EmployeeId, l.Status })
                .FirstOrDefaultAsync();

            if (leaveApplication == null)
            {
                return ApiResult<bool>.Failure("Không tìm thấy đơn xin nghỉ.");
            }

            if (leaveApplication.EmployeeId != currentEmployeeId)
            {
                return ApiResult<bool>.Failure("Bạn không có quyền xóa đơn xin nghỉ này.");
            }

            if (leaveApplication.Status != LeaveApplicationStatus.Pending)
            {
                return ApiResult<bool>.Failure("Chỉ có thể xóa đơn xin nghỉ ở trạng thái chờ xác nhận.");
            }

            await _unitOfWork.LeaveApplications.DeleteSoft(id);
            return ApiResult<bool>.Success("Xóa đơn xin nghỉ thành công", true);
        }


    }
}
