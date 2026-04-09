using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HRM_BE.Api.Controllers.Payroll_Timekeeping.TimekeepingRegulation
{
    [Route("api/timekeeping-gps-log")]
    [ApiController]
    public class TimekeepingGpsLogController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TimekeepingGpsLogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Kiểm tra trạng thái checkin của nhân viên theo ca làm việc theo 3 trạng thái: có thể checkin, có thể checkout, đã checkout
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet("checkin-status")]
        public async Task<IActionResult> GetCheckInStatus(int employeeId)
        {
            // Lấy thông tin nhân viên
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không tồn tại", false));
            }

            // Lấy OrganizationId của nhân viên
            int organizationId = employee.OrganizationId ?? 0;
            if (organizationId == 0)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không thuộc tổ chức nào", false));
            }

            // Lấy ca làm việc (shiftWorkId) cho ngày hiện tại
            var nowDay = DateTimeHelper.BusinessNow;
            var shiftWorkId = await _unitOfWork.ShiftWorks.GetShiftWorkIdForModuleTimekeepingGpsInOutAsync(organizationId, nowDay);

            if (!shiftWorkId.HasValue)
            {
                return Ok(ApiResult<bool>.Failure("Không tìm thấy ca làm việc cho ngày hôm nay", false));
            }

            // Lấy thông tin timesheet của nhân viên cho ca làm việc hôm nay
            var timesheet = await _unitOfWork.Timesheet.GetByEmployeeAndShiftAsync(employeeId, shiftWorkId.Value, nowDay);

            // Lấy cấu hình áp dụng chấm công của tổ chức (ApplyOrganization)
            // Để front-end biết có yêu cầu vị trí GPS khi chấm công hay không
            var applyOrg = await _unitOfWork.ApplyOrganization.GetFirstByOrganizationId(organizationId);

            // Trả về thông tin checkin của nhân viên cho front-end
            var checkinStatus = new CheckInStatus
            {
                CanCheckIn = false,   // Trạng thái checkin có thể thực hiện hay không
                CanCheckOut = false,  // Trạng thái checkout có thể thực hiện hay không
                IsCheckedOut = false,  // Trạng thái đã checkout hay chưa
                // Nếu không có cấu hình, mặc định cho phép chấm ở bất cứ đâu (không yêu cầu GPS)
                TimekeepingLocationOption = applyOrg?.TimekeepingLocationOption ?? TimekeepingLocationOption.NotFix,
                TimekeepingLocationId = applyOrg?.TimekeepingLocationId
            };

            if (timesheet == null)
            {
                // Nếu chưa có timesheet cho nhân viên, có thể checkin
                checkinStatus.CanCheckIn = true;
            }
            else
            {
                // Nếu đã có timesheet cho nhân viên, kiểm tra trạng thái checkin và checkout
                if (!timesheet.StartTime.HasValue)
                {
                    // Nếu chưa checkin, có thể checkin
                    checkinStatus.CanCheckIn = true;
                }
                else if (!timesheet.EndTime.HasValue)
                {
                    // Đã checkin nhưng chưa checkout
                    checkinStatus.CanCheckOut = true;
                }
                else
                {
                    // Đã checkout
                    checkinStatus.IsCheckedOut = true;
                }
            }

            return Ok(ApiResult<CheckInStatus>.LogSuccess(checkinStatus, true));
        }

        /// <summary>
        /// Chấm công GPS cho nhân viên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("checkin-checkout")]
        public async Task<IActionResult> CheckinCheckout([FromBody] CheckinCheckoutRequest request)
        {
            var nowDay = DateTimeHelper.BusinessNow;
            var nowHourOfDay = nowDay.TimeOfDay;

            // Kiểm tra xem EmployeeId có tồn tại không
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không tồn tại", false));
            }

            // Kiểm tra xem EmployeeId thuộc Contract nào
            bool contractIsValid = await _unitOfWork.Contracts.CheckEmployeeHaveContractValid(request.EmployeeId);

            if (!contractIsValid)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không có hợp đồng hoặc đã hết hạn", false));
            }

            // Lấy OrganizationId từ nhân viên
            if (employee.OrganizationId == null)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không thuộc OrganizationId nào", false));
            }

            int organizationId = employee.OrganizationId.Value;

            // Lấy ShiftWorkId
            var shiftWorkId = await _unitOfWork.ShiftWorks.GetShiftWorkIdForModuleTimekeepingGpsInOutAsync(organizationId, nowDay);
            if (!shiftWorkId.HasValue)
            {
                return Ok(ApiResult<bool>.Failure("Không tìm thấy ca làm việc cho tổ chức này", false));
            }

            // Lấy thông tin ca làm việc
            var shiftWork = await _unitOfWork.ShiftWorks.GetById(shiftWorkId.Value);
            if (shiftWork == null)
            {
                return Ok(ApiResult<bool>.Failure("Không tìm thấy thông tin ca làm việc", false));
            }

            // Kiểm tra thời gian hiện tại có nằm trong khoảng thời gian của ShiftWork
            if (nowDay < shiftWork.StartDate || nowDay > shiftWork.EndDate)
            {
                return Ok(ApiResult<bool>.Failure("Nhân viên không nằm trong khoảng thời gian ca làm việc", false));
            }

            // Lấy thông tin ShiftCatalog từ Shiftwork
            var shiftCatalog = await _unitOfWork.Timesheet.GetShiftCatalogByShiftWorkId(shiftWorkId.Value);
            if (shiftCatalog == null)
            {
                return Ok(ApiResult<bool>.Failure("Không tìm thấy thông tin chi tiết của ca làm việc", false));
            }

            // Lấy cấu hình áp dụng chấm công của tổ chức (ApplyOrganization)
            // Để biết tổ chức này có yêu cầu vị trí GPS khi chấm công hay không
            var applyOrg = await _unitOfWork.ApplyOrganization.GetFirstByOrganizationId(organizationId);

            // Mặc định: nếu không có cấu hình thì vẫn kiểm tra theo địa điểm (giữ nguyên hành vi cũ)
            var requiresLocation = true;
            if (applyOrg != null)
            {
                // Quy ước:
                // - TimekeepingLocationOption.NotFix: không yêu cầu vị trí -> có thể chấm ở bất cứ đâu
                // - TimekeepingLocationOption.Fix: yêu cầu vị trí -> phải trong phạm vi GPS
                requiresLocation = applyOrg.TimekeepingLocationOption == TimekeepingLocationOption.Fix;
            }

            if (requiresLocation)

            {
                // Lấy thông tin địa điểm chấm công từ bảng TimekeepingLocation dựa trên OrganizationId
                var location = await _unitOfWork.TimekeepingLocation.GetByOrganizationIdAsync(organizationId);

                if (location == null)
                {
                    return Ok(ApiResult<bool>.Failure("Tổ chức của bạn hiện tại chưa thiết lập địa điểm chấm công", false));
                }

                // Kiểm tra vị trí GPS của nhân viên có nằm trong bán kính cho phép không
                if (!IsValidLocation(location, request.Latitude, request.Longitude))
                {
                    return Ok(ApiResult<bool>.Failure($"Hiện tại vị trí của bạn nằm ngoài phạm vi chấm công", false));
                }
            }

            // Kiểm tra dữ liệu chấm công theo ca
            var timesheet = await _unitOfWork.Timesheet.GetByEmployeeAndShiftAsync(request.EmployeeId, shiftWorkId.Value, nowDay);

            if (request.Type == TimekeepingGPSType.CheckIn)
            {
                // Kiểm tra thời gian checkin có nằm trong khoảng thời gian hợp lệ
                if (shiftCatalog.StartTimeIn.HasValue && shiftCatalog.EndTimeIn.HasValue)
                {
                    if (nowHourOfDay < shiftCatalog.StartTimeIn.Value || nowHourOfDay > shiftCatalog.EndTimeIn.Value)
                    {
                        return Ok(ApiResult<bool>.Failure($"Hiện tại không nằm trong khoảng thời gian checkin hợp lệ {shiftCatalog.StartTimeIn.Value} - {shiftCatalog.EndTimeIn.Value}", false));
                    }
                }

                if (timesheet != null && timesheet.StartTime.HasValue)
                {
                    return Ok(ApiResult<bool>.Failure("Bạn đã checkin trong ca làm việc này rồi", false));
                }
            }
            else if (request.Type == TimekeepingGPSType.CheckOut)
            {
                // Kiểm tra thời gian checkout có nằm trong khoảng thời gian hợp lệ
                if (shiftCatalog.StartTimeOut.HasValue && shiftCatalog.EndTimeOut.HasValue)
                {
                    if (nowHourOfDay < shiftCatalog.StartTimeOut.Value || nowHourOfDay > shiftCatalog.EndTimeOut.Value)
                    {
                        return Ok(ApiResult<bool>.Failure($"Hiện tại không nằm trong khoảng thời gian checkout hợp lệ {shiftCatalog.StartTimeOut.Value} - {shiftCatalog.EndTimeOut.Value}", false));
                    }
                }

                if (timesheet == null || !timesheet.StartTime.HasValue)
                {
                    return Ok(ApiResult<bool>.Failure("Bạn chưa checkin, không thể checkout", false));
                }

                if (timesheet.EndTime.HasValue)
                {
                    return Ok(ApiResult<bool>.Failure("Bạn đã checkout trong ca làm việc này rồi", false));
                }
            }
            else
            {
                return Ok(ApiResult<bool>.Failure("Loại chấm công không hợp lệ", false));
            }

            if (request.Type == TimekeepingGPSType.CheckIn)
            {
                // Lưu thông tin checkin vào bảng Timesheet
                var timesheetTable = timesheet ?? new Timesheet
                {
                    EmployeeId = request.EmployeeId,
                    ShiftWorkId = shiftWorkId.Value,
                    Date = nowDay,
                    StartTime = nowHourOfDay,
                    TimekeepingType = TimekeepingType.GPS,
                    LateDuration = 0,
                    EarlyLeaveDuration = 0
                };

                // Lấy giờ làm việc bắt đầu
                var shiftStartTime = shiftCatalog.StartTime;
                var checkInTime = nowHourOfDay;

                // Logic mới: chấm vào sớm hơn giờ ca thì ghi nhận từ giờ bắt đầu ca
                if (shiftStartTime != null)
                {
                    if (checkInTime < shiftStartTime)
                    {
                        // Chấm vào sớm: ghi nhận từ giờ bắt đầu ca
                        timesheetTable.StartTime = shiftStartTime.Value;
                        timesheetTable.LateDuration = 0;
                    }
                    else
                    {
                        // Chấm vào muộn hoặc đúng giờ: ghi nhận giờ chấm thực tế
                        timesheetTable.StartTime = checkInTime;
                        var lateDuration = checkInTime.Subtract(shiftStartTime.Value).TotalMinutes;
                        timesheetTable.LateDuration = lateDuration;
                    }
                }
                else
                {
                    timesheetTable.StartTime = checkInTime;
                    timesheetTable.LateDuration = 0;
                }

                await _unitOfWork.Timesheet.AddOrUpdateTimesheetAsync(timesheetTable);

                return Ok(ApiResult<bool>.Success("Check-in thành công!", true));
            }
            else if (request.Type == TimekeepingGPSType.CheckOut)
            {
                // Cập nhật thời gian checkout vào bảng Timesheet
                if (timesheet != null)
                {
                    var checkOutTime = nowHourOfDay;
                    var shiftEndTime = shiftCatalog.EndTime;

                    // Logic mới: chấm ra muộn hơn giờ ca thì ghi nhận giờ kết thúc ca
                    if (shiftEndTime != null)
                    {
                        if (checkOutTime < shiftEndTime)
                        {
                            // Chấm ra sớm: ghi nhận giờ chấm thực tế
                            timesheet.EndTime = checkOutTime;
                            var earlyLeaveDuration = shiftEndTime.Value.Subtract(checkOutTime).TotalMinutes;
                            timesheet.EarlyLeaveDuration = earlyLeaveDuration;
                        }
                        else
                        {
                            // Chấm ra muộn hoặc đúng giờ: ghi nhận giờ kết thúc ca
                            timesheet.EndTime = shiftEndTime.Value;
                            timesheet.EarlyLeaveDuration = 0;
                        }
                    }
                    else
                    {
                        timesheet.EndTime = checkOutTime;
                        timesheet.EarlyLeaveDuration = 0;
                    }

                    await _unitOfWork.Timesheet.AddOrUpdateTimesheetAsync(timesheet);
                }

                return Ok(ApiResult<bool>.Success("Check-out thành công!", true));
            }

            return Ok(ApiResult<bool>.Success("Có lỗi xảy ra khi chấm công", false));
        }

        // Kiểm tra vị trí GPS của nhân viên có nằm trong bán kính cho phép không
        private bool IsValidLocation(TimekeepingLocation location, double latitude, double longitude)
        {
            double distance = CalculateDistance(double.Parse(location.Latitude), double.Parse(location.Longitude), latitude, longitude);
            return distance <= location.AllowableRadius;
        }

        // Dùng công thức haversine để tính khoảng cách giữa 2 điểm trên bản đồ
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Bán kính Trái đất (km)
            var dLat = DegreeToRadian(lat2 - lat1);
            var dLon = DegreeToRadian(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreeToRadian(lat1)) * Math.Cos(DegreeToRadian(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c * 1000; // Khoảng cách (m)
        }

        private double DegreeToRadian(double deg)
        {
            return deg * (Math.PI / 180);
        }

    }

    public class CheckInStatus
    {
        public bool CanCheckIn { get; set; }
        public bool CanCheckOut { get; set; }
        public bool IsCheckedOut { get; set; }

        // 0: không yêu cầu vị trí (chấm ở bất cứ đâu)
        // 1: yêu cầu vị trí (kiểm tra GPS trong phạm vi địa điểm)
        public TimekeepingLocationOption TimekeepingLocationOption { get; set; }
        public int? TimekeepingLocationId { get; set; }
    }

}
