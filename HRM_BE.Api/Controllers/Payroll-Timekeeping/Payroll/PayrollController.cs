using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Payroll_Timekeeping.Payroll
{
    [Route("api/payroll")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PayrollController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn thêm bảng lương
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResult<PayrollDto>> Create(CreatePayrollRequest request)
        {
            try
            {
                var result = await _unitOfWork.Payrolls.Create(request);
                return ApiResult<PayrollDto>.Success("Thêm bảng lương thành công", result);
            }
            catch (Exception ex)
            {
                return ApiResult<PayrollDto>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn xem danh sách bảng lương
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("paging")]
        public async Task<PagingResult<PayrollDto>> Paging([FromQuery] PagingPayrollRequest request)
        {
            var result = await _unitOfWork.Payrolls.Paging(request.OrganizationId, request.Name, request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return result;
        }

        [HttpGet("get-by-id")]
        public async Task<PayrollDto> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.Payrolls.GetById(request.Id);
            return result;
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn sửa bảng lương
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, UpdatePayrollRequest request)
        {
            await _unitOfWork.Payrolls.Update(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật bảng lương thành công", true));
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn xóa bảng lương
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.Payrolls.Delete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá bảng lương thành công", true));
        }


        /// <summary>
        /// Là nhân viên muốn xem bảng lương của mình
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("paging-for-employee")]
        public async Task<PagingResult<PayrollDto>> PagingForEmployee([FromQuery] PagingPayrollRequest request)
        {
            var result = await _unitOfWork.Payrolls.PagingForEmployee(request.OrganizationId, request.Name, request.EmployeeId, request.Year, request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return result;
        }


        /// <summary>
        /// Quản lý khóa và mở khóa bảng lương
        /// </summary>
        /// <param name="payrollId"></param>
        /// <returns></returns>
        [HttpPost("toggle-payroll-status")]  
        public async Task<IActionResult> TogglePayrollStatus(int payrollId)
        {
            try
            {
                await _unitOfWork.Payrolls.TogglePayrollStatus(payrollId);
                return Ok(ApiResult<bool>.Success("Đổi trạng thái khóa mở khóa bảng lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure($"{ex.Message}", false));
            }
        }


        /// <summary>
        /// Kiểm tra trạng thái khóa của bảng lương trả về true nếu đang bị khóa
        /// </summary>
        /// <param name="payrollId"></param>
        /// <returns></returns>
        [HttpGet("is-payroll-locked")]   
        public async Task<IActionResult> IsPayrollLocked(int payrollId)
        {
            try
            {
                var isLocked = await _unitOfWork.Payrolls.IsPayrollLocked(payrollId);
                return Ok(new {isLocked});
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure($"{ex.Message}", false));
            }
        }
    }
}
