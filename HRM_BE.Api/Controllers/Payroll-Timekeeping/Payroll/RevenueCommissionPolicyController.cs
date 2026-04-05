using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Payroll_Timekeeping.Payroll
{
    [Route("api/revenue-commission-policy")]
    [ApiController]
    public class RevenueCommissionPolicyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevenueCommissionPolicyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("paging")]
        public async Task<PagingResult<RevenueCommissionPolicyDto>> Paging([FromQuery] PagingRevenueCommissionPolicyRequest request)
        {
            return await _unitOfWork.RevenueCommissionPolicies.Paging(request);
        }

        [HttpGet("get-by-id")]
        public async Task<RevenueCommissionPolicyDto> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            return await _unitOfWork.RevenueCommissionPolicies.GetById(request.Id);
        }

        [HttpPost("create")]
        public async Task<ApiResult<RevenueCommissionPolicyDto>> Create([FromBody] CreateRevenueCommissionPolicyRequest request)
        {
            var result = await _unitOfWork.RevenueCommissionPolicies.Create(request);
            return ApiResult<RevenueCommissionPolicyDto>.Success("Thêm cấu hình hoa hồng thành công", result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] UpdateRevenueCommissionPolicyRequest request)
        {
            await _unitOfWork.RevenueCommissionPolicies.Update(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật cấu hình hoa hồng thành công", true));
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] int id, [FromQuery] Status status)
        {
            await _unitOfWork.RevenueCommissionPolicies.UpdateStatus(id, status);
            return Ok(ApiResult<bool>.Success("Cập nhật trạng thái cấu hình hoa hồng thành công", true));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.RevenueCommissionPolicies.Delete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá cấu hình hoa hồng thành công", true));
        }

        [HttpDelete("hard-delete")]
        public async Task<IActionResult> HardDelete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.RevenueCommissionPolicies.HardDelete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá vĩnh viễn cấu hình hoa hồng thành công", true));
        }
    }
}

