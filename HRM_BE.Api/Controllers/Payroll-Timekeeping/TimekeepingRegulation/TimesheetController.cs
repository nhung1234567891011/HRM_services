using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Models.ShiftCatalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Payroll_Timekeeping.TimekeepingRegulation
{
    [Route("api/time-sheet")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TimesheetController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Lấy về danh sách chấm công của nhân viên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("paging")]
        public async Task<PagingResult<TimesheetDto>> Paging([FromQuery] PagingTimesheetRequest request)
        {
            var result = await _unitOfWork.Timesheet.Paging(request.EmployeeId, request.StartDate, request.EndDate, request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return result;
        }


        [HttpGet("get-by-id")]
        public async Task<TimesheetDto> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.Timesheet.GetById(request.Id);
            return result;
        }

        /// <summary>
        /// Lấy về danh mục ca làm việc theo shiftWorkId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-by-shiftworkid")]
        public async Task<ApiResult<ShiftCatalogDto>> GetByShiftWorkId([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.ShiftCatalogs.GetByShiftWorkId(request.Id);
            return ApiResult<ShiftCatalogDto>.Success("Lấy thông tin danh mục ca thành công", result);
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn chỉnh sửa chấm công chi tiết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, UpdateTimesheetRequest request)
        {
            await _unitOfWork.Timesheet.Update(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật chấm công chi tiết thành công", true));
        }

        /// <summary>
        /// Thêm mới bản ghi chấm công (cho ngày không đi làm hoặc chưa có dữ liệu)
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateTimesheetRequest request)
        {
            var id = await _unitOfWork.Timesheet.CreateTimesheet(request);
            return Ok(ApiResult<int>.Success("Tạo bản ghi chấm công thành công", id));
        }

        /// <summary>
        /// Lấy về số lần đi muộn về sớm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-time-sheet-duration-late-or-early")]
        public async Task<ApiResult<TimesheetDurationLateOrEarlyDto>> GetTimesheetDurationLateOrEarly([FromQuery] GetTotalNumberOfDaysOffRequest request)
        {
            var result = await _unitOfWork.Timesheet.GetTimesheetDurationLateOrEarly(request.StartDate, request.EndDate, request.EmployeeId);
            return ApiResult<TimesheetDurationLateOrEarlyDto>.Success("Lấy số ngày đi sớm về muộn thành công", result);
        }
    }
}
