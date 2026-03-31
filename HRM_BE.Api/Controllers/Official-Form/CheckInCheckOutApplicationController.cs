using ClosedXML.Excel;
using HRM_BE.Api.Extension;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Constants.Identity;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.CheckInCheckOut;
using HRM_BE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS1591

namespace HRM_BE.Api.Controllers.Official_Form
{
    [Route("api/checkin-checkout-application")]
    [ApiController]
    [Authorize]
    public class CheckInCheckOutApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HrmContext _dbContext;

        public CheckInCheckOutApplicationController(IUnitOfWork unitOfWork, HrmContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        private int GetCurrentEmployeeId()
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            if (identity == null)
            {
                return 0;
            }

            var claimValue = identity.GetSpecificClaim(ClaimTypeConstant.EmployeeId);
            return int.TryParse(claimValue, out var employeeId) ? employeeId : 0;
        }

        private bool IsCurrentUserAdmin()
        {
            return User.Claims.Any(c => c.Type == ClaimTypeConstant.Permission && c.Value == PermissionConstant.Admin);
        }

        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<CheckInCheckOutApplicationDto>>> GetPaging(
            [FromQuery] GetCheckInCheckOutApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<PagingResult<CheckInCheckOutApplicationDto>>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var isAdmin = IsCurrentUserAdmin();
            var forApproval = request.ForApproval == true;

            if (!isAdmin)
            {
                if (forApproval)
                {
                    request.EmployeeId = null;
                }
                else
                {
                    request.EmployeeId = currentEmployeeId;
                }
            }

            var result = await _unitOfWork.CheckInCheckOutApplications.GetPaging(
                request.OrganizationId,
                request.EmployeeId,
                currentEmployeeId,
                isAdmin,
                forApproval,
                request.KeyWord,
                request.StartDate,
                request.EndDate,
                request.CheckInCheckOutStatus,
                request.SortBy,
                request.OrderBy,
                request.PageIndex,
                request.PageSize);

            return new ApiResult<PagingResult<CheckInCheckOutApplicationDto>>(
                "Danh sách đơn checkin/checkout đã được lấy thành công!",
                result
            );
        }

        [HttpGet("get-by-id")]
        public async Task<ApiResult<CheckInCheckOutApplicationDto>> GetById(
            [FromQuery] EntityIdentityRequest<int> request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<CheckInCheckOutApplicationDto>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var isAdmin = IsCurrentUserAdmin();
            var canView = await _dbContext.CheckInCheckOutApplications.AnyAsync(x =>
                x.Id == request.Id &&
                x.IsDeleted != true &&
                (isAdmin || x.EmployeeId == currentEmployeeId || x.ApproverId == currentEmployeeId));

            if (!canView)
            {
                return ApiResult<CheckInCheckOutApplicationDto>.Failure("Bạn không có quyền xem đơn checkin/checkout này.");
            }

            var result = await _unitOfWork.CheckInCheckOutApplications.GetById(request.Id);
            return new ApiResult<CheckInCheckOutApplicationDto>(
                "Đơn checkin/checkout đã được lấy thành công!",
                result
            );
        }

        [HttpPost("create")]
        public async Task<ApiResult<CheckInCheckOutApplicationDto>> Create(
            [FromBody] CreateCheckInCheckOutApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<CheckInCheckOutApplicationDto>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var approverExists = await _dbContext.Employees.AnyAsync(e => e.Id == request.ApproverId && e.IsDeleted != true);
            if (!approverExists)
            {
                return ApiResult<CheckInCheckOutApplicationDto>.Failure("Người duyệt không tồn tại.");
            }

            request.EmployeeId = currentEmployeeId;

            var id = await _unitOfWork.CheckInCheckOutApplications.CreateAsync(request);
            var result = await _unitOfWork.CheckInCheckOutApplications.GetById(id);
            return ApiResult<CheckInCheckOutApplicationDto>.Success(
                "Tạo đơn checkin/checkout thành công",
                result
            );
        }

        [HttpPut("update")]
        public async Task<ApiResult<bool>> Update(
            int id,
            [FromBody] UpdateCheckInCheckOutApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            var application = await _dbContext.CheckInCheckOutApplications
                .Where(x => x.Id == id && x.IsDeleted != true)
                .Select(x => new { x.EmployeeId, x.CheckInCheckOutStatus })
                .FirstOrDefaultAsync();

            if (application == null)
            {
                return ApiResult<bool>.Failure("Không tìm thấy đơn checkin/checkout.");
            }

            if (application.EmployeeId != currentEmployeeId)
            {
                return ApiResult<bool>.Failure("Bạn không có quyền cập nhật đơn checkin/checkout này.");
            }

            if (application.CheckInCheckOutStatus != 0)
            {
                return ApiResult<bool>.Failure("Chỉ có thể cập nhật đơn ở trạng thái chờ duyệt.");
            }

            await _unitOfWork.CheckInCheckOutApplications.UpdateAsync(id, request);
            return ApiResult<bool>.Success("Cập nhật đơn checkin/checkout thành công", true);
        }

        [HttpPut("update-status")]
        public async Task<ApiResult<bool>> UpdateStatus(int id, [FromBody] int status)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập.");
            }

            if (status != 1 && status != 2)
            {
                return ApiResult<bool>.Failure("Trạng thái duyệt không hợp lệ.");
            }

            var application = await _dbContext.CheckInCheckOutApplications
                .Where(x => x.Id == id && x.IsDeleted != true)
                .FirstOrDefaultAsync();

            if (application == null)
            {
                return ApiResult<bool>.Failure("Không tìm thấy đơn checkin/checkout.");
            }

            // Only the designated approver can approve/reject (including admin if they are the designated approver)
            if (application.ApproverId != currentEmployeeId)
            {
                return ApiResult<bool>.Failure("Chỉ người duyệt được chỉ định mới có quyền duyệt/từ chối đơn này.");
            }

            if (application.CheckInCheckOutStatus != 0)
            {
                return ApiResult<bool>.Failure("Đơn này đã được xử lý trước đó.");
            }

            await _unitOfWork.CheckInCheckOutApplications.UpdateStatusAsync(id, status);

            if (status == 1)
            {
                await ApplyApprovedCheckInCheckOutToTimesheetForUpdateCheckInCheckOutAsync(application);
            }

            return ApiResult<bool>.Success("Cập nhật trạng thái đơn checkin/checkout thành công", true);
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] GetCheckInCheckOutApplicationRequest request)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                return Ok(ApiResult<bool>.Failure("Không xác định được nhân viên đăng nhập."));
            }

            var isAdmin = IsCurrentUserAdmin();
            var forApproval = request.ForApproval == true;

            var data = await _unitOfWork.CheckInCheckOutApplications.GetExportData(
                request.OrganizationId,
                request.EmployeeId,
                currentEmployeeId,
                isAdmin,
                forApproval,
                request.KeyWord,
                request.StartDate,
                request.EndDate,
                request.CheckInCheckOutStatus);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Đơn CheckIn/CheckOut");

            var headers = new[]
            {
                "STT", "Mã nhân viên", "Họ và tên", "Ngày", "Giờ chấm vào",
                "Giờ chấm ra", "Lý do", "Mô tả", "Ca làm việc", "Người duyệt", "Trạng thái"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int i = 0; i < data.Count; i++)
            {
                var row = data[i];
                var rowIndex = i + 2;
                var statusText = row.CheckInCheckOutStatus switch
                {
                    0 => "Chờ duyệt",
                    1 => "Đã duyệt",
                    2 => "Từ chối",
                    _ => "Không xác định"
                };

                worksheet.Cell(rowIndex, 1).Value = i + 1;
                worksheet.Cell(rowIndex, 2).Value = row.Employee.EmployeeCode;
                worksheet.Cell(rowIndex, 3).Value = $"{row.Employee.LastName} {row.Employee.FirstName}".Trim();
                worksheet.Cell(rowIndex, 4).Value = row.Date.ToString("dd/MM/yyyy");
                worksheet.Cell(rowIndex, 5).Value = row.TimeCheckIn.HasValue ? row.TimeCheckIn.Value.ToString(@"hh\:mm") : "";
                worksheet.Cell(rowIndex, 6).Value = row.TimeCheckOut.HasValue ? row.TimeCheckOut.Value.ToString(@"hh\:mm") : "";
                worksheet.Cell(rowIndex, 7).Value = row.Reason;
                worksheet.Cell(rowIndex, 8).Value = row.Description ?? "";
                worksheet.Cell(rowIndex, 9).Value = row.ShiftCatalog.Name;
                worksheet.Cell(rowIndex, 10).Value = $"{row.Approver.LastName} {row.Approver.FirstName}".Trim();
                worksheet.Cell(rowIndex, 11).Value = statusText;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"DonCheckInCheckOut_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private async Task ApplyApprovedCheckInCheckOutToTimesheetForUpdateCheckInCheckOutAsync(HRM_BE.Core.Data.Official_Form.CheckInCheckOutApplication application)
        {
            var now = DateTime.Now;
            var currentUserId = User.GetUserId();
            var currentUserName = User.GetUserName();

            var employee = await _dbContext.Employees
                .Where(e => e.Id == application.EmployeeId && e.IsDeleted != true)
                .Select(e => new { e.Id, e.OrganizationId })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return;
            }

            var shiftWork = await _dbContext.ShiftWorks
                .Include(sw => sw.ShiftCatalog)
                .Where(sw =>
                    sw.IsDeleted != true &&
                    sw.OrganizationId == employee.OrganizationId &&
                    sw.ShiftCatalogId == application.ShiftCatalogId &&
                    sw.StartDate.HasValue && sw.EndDate.HasValue &&
                    sw.StartDate.Value.Date <= application.Date.Date &&
                    sw.EndDate.Value.Date >= application.Date.Date)
                .OrderByDescending(sw => sw.StartDate)
                .FirstOrDefaultAsync();

            var shiftCatalog = shiftWork?.ShiftCatalog;

            var existing = await _dbContext.Timesheets
                .FirstOrDefaultAsync(t =>
                    t.EmployeeId == application.EmployeeId &&
                    t.Date.HasValue &&
                    t.Date.Value.Date == application.Date.Date);

            var hasCheckIn = application.CheckType == 0 || application.CheckType == 2;
            var hasCheckOut = application.CheckType == 1 || application.CheckType == 2;

            TimeSpan? adjustedStartTime = null;
            TimeSpan? adjustedEndTime = null;
            double lateDuration = 0;
            double earlyLeaveDuration = 0;

            if (hasCheckIn && application.TimeCheckIn.HasValue)
            {
                var checkInTime = application.TimeCheckIn.Value;

                // Rule for check-in only: if check in early or on-time, counting starts from shift start.
                if (!hasCheckOut && shiftCatalog?.StartTime.HasValue == true && checkInTime <= shiftCatalog.StartTime.Value)
                {
                    adjustedStartTime = shiftCatalog.StartTime.Value;
                }
                else
                {
                    adjustedStartTime = checkInTime;
                }

                if (shiftCatalog?.StartTime.HasValue == true && adjustedStartTime.Value > shiftCatalog.StartTime.Value)
                {
                    lateDuration = (adjustedStartTime.Value - shiftCatalog.StartTime.Value).TotalMinutes;
                }
            }

            if (hasCheckOut && application.TimeCheckOut.HasValue)
            {
                // Checkout is always kept as submitted, even when later than shift end.
                var checkOutTime = application.TimeCheckOut.Value;
                adjustedEndTime = checkOutTime;

                if (shiftCatalog?.EndTime.HasValue == true)
                {
                    var baseDate = application.Date.Date;
                    var checkOutAt = baseDate.Add(checkOutTime);
                    var shiftEndAt = baseDate.Add(shiftCatalog.EndTime.Value);

                    if (shiftCatalog.StartTime.HasValue)
                    {
                        var shiftStartAt = baseDate.Add(shiftCatalog.StartTime.Value);

                        // Overnight shift: move end to next day and align checkout to the same timeline.
                        if (shiftEndAt <= shiftStartAt)
                        {
                            shiftEndAt = shiftEndAt.AddDays(1);

                            if (checkOutAt <= shiftStartAt)
                            {
                                checkOutAt = checkOutAt.AddDays(1);
                            }
                        }
                    }

                    if (checkOutAt < shiftEndAt)
                    {
                        earlyLeaveDuration = (shiftEndAt - checkOutAt).TotalMinutes;
                    }
                }
            }

            Timesheet timesheet;

            if (existing == null)
            {
                timesheet = new Timesheet
                {
                    EmployeeId = application.EmployeeId,
                    Date = application.Date.Date,
                    ShiftWorkId = shiftWork?.Id,
                    StartTime = adjustedStartTime,
                    EndTime = adjustedEndTime,
                    LateDuration = lateDuration,
                    EarlyLeaveDuration = earlyLeaveDuration,
                    TimeKeepingLeaveStatus = TimeKeepingLeaveStatus.None,
                    TimekeepingType = TimekeepingType.GPS,
                    CreatedBy = currentUserId > 0 ? currentUserId : null,
                    CreatedName = currentUserName,
                    CreatedAt = now,
                    UpdatedBy = currentUserId > 0 ? currentUserId : null,
                    UpdatedName = currentUserName,
                    UpdatedAt = now,
                    IsDeleted = false
                };

                await _dbContext.Timesheets.AddAsync(timesheet);
            }
            else
            {
                timesheet = existing;

                if (hasCheckIn && adjustedStartTime.HasValue)
                {
                    timesheet.StartTime = adjustedStartTime;
                    timesheet.LateDuration = lateDuration;
                }

                if (hasCheckOut && adjustedEndTime.HasValue)
                {
                    timesheet.EndTime = adjustedEndTime;
                    timesheet.EarlyLeaveDuration = earlyLeaveDuration;
                }

                if (!timesheet.ShiftWorkId.HasValue && shiftWork != null)
                {
                    timesheet.ShiftWorkId = shiftWork.Id;
                }

                timesheet.TimeKeepingLeaveStatus = TimeKeepingLeaveStatus.None;
                timesheet.TimekeepingType ??= TimekeepingType.GPS;

                timesheet.UpdatedBy = currentUserId > 0 ? currentUserId : null;
                timesheet.UpdatedName = currentUserName;
                timesheet.UpdatedAt = now;

                // Backfill old records that were created without audit metadata.
                if (!timesheet.CreatedAt.HasValue)
                {
                    timesheet.CreatedAt = now;
                }

                if (!timesheet.CreatedBy.HasValue && currentUserId > 0)
                {
                    timesheet.CreatedBy = currentUserId;
                }

                if (string.IsNullOrWhiteSpace(timesheet.CreatedName))
                {
                    timesheet.CreatedName = currentUserName;
                }
            }

            var calculatedWorkingHours = CalculateWorkingHoursForCheckInCheckOut(timesheet, shiftCatalog);
            timesheet.NumberOfWorkingHour = calculatedWorkingHours;

            var standardHours = shiftCatalog?.WorkingHours ?? 8.0;
            timesheet.OvertimeHour = calculatedWorkingHours > standardHours
                ? Math.Round(calculatedWorkingHours - standardHours, 2)
                : 0;

            if (calculatedWorkingHours + 0.01 >= standardHours)
            {
                // If total hours already meet shift standard, do not keep early-leave penalty.
                timesheet.EarlyLeaveDuration = 0;
            }

            await _dbContext.SaveChangesAsync();
        }

        private static double CalculateWorkingHoursForCheckInCheckOut(
            Timesheet timesheet,
            HRM_BE.Core.Data.Payroll_Timekeeping.Shift.ShiftCatalog? shiftCatalog)
        {
            if (!timesheet.StartTime.HasValue || !timesheet.EndTime.HasValue)
            {
                return 0;
            }

            var date = timesheet.Date ?? DateTime.Today;
            var start = date.Date.Add(timesheet.StartTime.Value);
            var end = date.Date.Add(timesheet.EndTime.Value);

            if (end <= start)
            {
                end = end.AddDays(1);
            }

            var workingHours = (end - start).TotalHours;

            if (shiftCatalog?.TakeABreak == true
                && shiftCatalog.StartTakeABreak.HasValue
                && shiftCatalog.EndTakeABreak.HasValue)
            {
                var breakStart = date.Date.Add(shiftCatalog.StartTakeABreak.Value);
                var breakEnd = date.Date.Add(shiftCatalog.EndTakeABreak.Value);

                if (breakEnd <= breakStart)
                {
                    breakEnd = breakEnd.AddDays(1);
                }

                // For overnight shifts, break time like 01:00 belongs to next day timeline.
                if (breakStart < start)
                {
                    breakStart = breakStart.AddDays(1);
                    breakEnd = breakEnd.AddDays(1);
                }

                if (start < breakEnd && end > breakStart)
                {
                    var overlapStart = start > breakStart ? start : breakStart;
                    var overlapEnd = end < breakEnd ? end : breakEnd;
                    workingHours -= (overlapEnd - overlapStart).TotalHours;
                }
            }

            return Math.Max(0, Math.Round(workingHours, 2));
        }
    }
}

#pragma warning restore CS1591

