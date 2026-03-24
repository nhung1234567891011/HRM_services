using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Organization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace HRM_BE.Api.Controllers.Company
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public OrganizationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.Organizations.GetById(request.Id);
            return Ok(ApiResult<OrganizationDto>.Success("Lấy thông tin tổ chức thành công", result));
        }
        //[HttpGet("paging")]
        //public async Task<ApiResult<PagingResult<GetOrganizationDto>>> Paging([FromQuery] GetPagingOrganizationRequest request)
        //{
        //    var result = await _unitOfWork.Organizations.GetAll(request.keyWord, request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
        //    return ApiResult<PagingResult<GetOrganizationDto>>.Success("Lấy danh sách tổ chức thành công", result);
        //}

        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<GetOrganizationDto>>> Paging([FromQuery] GetPagingOrganizationRequest request)
        {
            var result = await _unitOfWork.Organizations.Paging(
                request.keyWord,
                request.SortBy,
                request.OrderBy,
                request.PageIndex,
                request.PageSize,
                request.OrganizationId);

            return ApiResult<PagingResult<GetOrganizationDto>>.Success("Lấy danh sách tổ chức thành công", result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyWord,
            [FromQuery] int? organizationId,
            [FromQuery] string? sortBy,
            [FromQuery] string? orderBy)
        {
            // Lấy toàn bộ danh sách (không phân trang) có thể lọc theo keyWord và OrganizationId
            var items = await _unitOfWork.Organizations.Export(
                keyWord,
                organizationId,
                sortBy,
                orderBy);

            return Ok(ApiResult<List<GetOrganizationDto>>.Success("Lấy danh sách tổ chức thành công", items));
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] string? keyWord,
            [FromQuery] int? organizationId,
            [FromQuery] string? sortBy,
            [FromQuery] string? orderBy)
        {
            // Sử dụng lại logic Export ở repository (dựa trên Paging)
            var items = await _unitOfWork.Organizations.Export(
                keyWord,
                organizationId,
                sortBy,
                orderBy);

            // Xuất CSV đơn giản (Excel vẫn mở được)
            var sb = new StringBuilder();
            sb.AppendLine("OrganizationCode,OrganizationName,Abbreviation,TotalEmployees");

            foreach (var item in items)
            {
                var code = item.OrganizationCode?.Replace(",", " ") ?? string.Empty;
                var name = item.OrganizationName?.Replace(",", " ") ?? string.Empty;
                var abbr = item.Abbreviation?.Replace(",", " ") ?? string.Empty;
                var totalEmployees = item.TotalEmployees;

                sb.AppendLine($"{code},{name},{abbr},{totalEmployees}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "organizations.csv");
        }

        [HttpGet("get-select")]
        public async Task<IActionResult> GetSelect([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.Organizations.GetSelect(request.Id);
            return Ok(ApiResult<OrganizationSelectDto>.Success("Lấy thông tin tổ chức thành công", result));
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request)
        {
            try
            {
                var createdOrganization = await _unitOfWork.Organizations.Create(request);
                return Ok(ApiResult<OrganizationDto>.Success("Thêm tổ chức thành công", createdOrganization));
            } catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizationRequest request)
        {
            await _unitOfWork.Organizations.Update(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật tổ chức thành công",true));
        }
        [HttpPut("delete")]
        public async Task<IActionResult> Delete(int organizationId)
        {
            await _unitOfWork.Organizations.Delete(organizationId);
            return Ok(ApiResult<bool>.Success("Xoá tổ chức thành công",true));
        }
    }
}
