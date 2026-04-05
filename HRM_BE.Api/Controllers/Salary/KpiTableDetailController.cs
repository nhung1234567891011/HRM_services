using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Salary.KpiTableDetail;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Salary
{
    [Route("api/kpi-table-detail")]
    [ApiController]
    public class KpiTableDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public KpiTableDetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("get-by-id")]
        public async Task<ApiResult<KpiTableDetailDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.KpiTableDetails.GetById(request.Id);
            return ApiResult<KpiTableDetailDto>.Success("Lấy thông tin bảng kpi chi tiết thành công", result);
        }

        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<KpiTableDetailDto>>> Paging([FromQuery] GetKpiTableDetailRequest request)
        {
            var result = await _unitOfWork.KpiTableDetails.Paging(request,
                request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<KpiTableDetailDto>>.Success("Lấy danh sách thông tin bảng kpi chi tiết", result);
        }

        [HttpPost("create")]
        public async Task<ApiResult<KpiTableDetailDto>> Create([FromBody] CreateKpiTableDetailRequest request)
        {
            var result = await _unitOfWork.KpiTableDetails.Create(request);
            return ApiResult<KpiTableDetailDto>.Success("Thêm phân ca thành công", result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int KpiTableDetailId, [FromBody] UpdateKpiTableDetailRequest request)
        {

            await _unitOfWork.KpiTableDetails.Update(KpiTableDetailId, request);
            return Ok(ApiResult<bool>.Success("Cập nhật bảng kpi chi tiết thành công", true));
        }

        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = _unitOfWork.KpiTableDetails.Delete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá bảng kpi chi tiết thành công", true));
        }
    }

}
