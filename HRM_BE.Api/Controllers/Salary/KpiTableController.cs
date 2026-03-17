using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Salary.KpiTable;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.KpiTable
{
    [Route("api/kpi-table")]
    [ApiController]
    public class KpiTableController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public KpiTableController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-by-id")]

        public async Task<ApiResult<KpiTableDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.KpiTables.GetById(request.Id);
            return ApiResult<KpiTableDto>.Success("Lấy thông tin phần ca thành công", result);
        }


        [HttpGet("paging")]

        public async Task<ApiResult<PagingResult<KpiTableDto>>> Paging([FromQuery] GetKpiTableRequest request)
        {
            var result = await _unitOfWork.KpiTables.Paging(request.NameKpiTable, request.OrganizationId,
                request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<KpiTableDto>>.Success("Lấy danh sách thông tin phần ca", result);
        }

        [HttpPost("create")]
        public async Task<ApiResult<KpiTableDto>> Create([FromBody] CreateKpiTableRequest request)
        {
            var result = await _unitOfWork.KpiTables.Create(request);
            return ApiResult<KpiTableDto>.Success("Thêm phân ca thành công", result);
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update(int KpiTableId, [FromBody] UpdateKpiTableRequest request)
        {

            await _unitOfWork.KpiTables.Update(KpiTableId, request);
            return Ok(ApiResult<bool>.Success("Cập nhật phân ca thành công", true));
        }


        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.KpiTables.Delete(request.Id);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResult<bool>.Success("Xoá bảng KPI thành công", true));
        }

        [HttpDelete("hard-delete")]
        public async Task<IActionResult> HardDelete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.KpiTables.HardDelete(request.Id);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResult<bool>.Success("Xoá vĩnh viễn bảng KPI thành công", true));
        }
    }
}
