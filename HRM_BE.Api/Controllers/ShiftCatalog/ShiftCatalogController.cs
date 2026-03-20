using HRM_BE.Core.Constants;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Company;
using HRM_BE.Core.Models.ShiftCatalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.ShiftCatalog
{
    [Route("api/shift-catalog")]
    [ApiController]
    public class ShiftCatalogController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShiftCatalogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("get-by-id")]

        public async Task<ApiResult<ShiftCatalogDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.ShiftCatalogs.GetById(request.Id);
            return ApiResult<ShiftCatalogDto>.Success("Lấy thông tin phần ca thành công", result);
        }
        [HttpGet("paging")]

        public async Task<ApiResult<PagingResult<ShiftCatalogDto>>> Paging([FromQuery] GetShiftCatalogPagingRequest request)
        {
            var result = await _unitOfWork.ShiftCatalogs.Paging(request.Name,request.OrganizationId,
                request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<ShiftCatalogDto>>.Success("Lấy danh sách thông tin phần ca", result);
        }
        [HttpPost("create")]
        public async Task<ApiResult<ShiftCatalogDto>> Create([FromBody] CreateShiftCatalogRequest request)
        {
            var result = await _unitOfWork.ShiftCatalogs.Create(request);
            return ApiResult<ShiftCatalogDto>.Success("Thêm phân ca thành công", result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> update(int shiftCatalogId, [FromBody] UpdateShiftCatalogRequest request)
        {

            await _unitOfWork.ShiftCatalogs.Update(shiftCatalogId, request);
            return Ok(ApiResult<bool>.Success("Cập nhật phân ca thành công", true));
        }

        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.ShiftCatalogs.Delete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá phân ca thành công", true));
        }

        [HttpPut("delete-range")]
        public async Task<IActionResult> DeleteRange([FromBody] ListEntityIdentityRequest<int> request)
        {
            await _unitOfWork.ShiftCatalogs.DeleteRange(request);
            return Ok(ApiResult<bool>.Success("Xoá nhiều phân ca thành công", true));
        }
    }
}
