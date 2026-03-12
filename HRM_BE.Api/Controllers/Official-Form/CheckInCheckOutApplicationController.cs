using ClosedXML.Excel;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.CheckInCheckOut;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Official_Form
{
    [Route("api/checkin-checkout-application")]
    [ApiController]
    public class CheckInCheckOutApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckInCheckOutApplicationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<CheckInCheckOutApplicationDto>>> GetPaging(
            [FromQuery] GetCheckInCheckOutApplicationRequest request)
        {
            var result = await _unitOfWork.CheckInCheckOutApplications.GetPaging(
                request.OrganizationId,
                request.EmployeeId,
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
            await _unitOfWork.CheckInCheckOutApplications.UpdateAsync(id, request);
            return ApiResult<bool>.Success("Cập nhật đơn checkin/checkout thành công", true);
        }

        [HttpPut("update-status")]
        public async Task<ApiResult<bool>> UpdateStatus(int id, [FromBody] int status)
        {
            await _unitOfWork.CheckInCheckOutApplications.UpdateStatusAsync(id, status);
            return ApiResult<bool>.Success("Cập nhật trạng thái đơn checkin/checkout thành công", true);
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] GetCheckInCheckOutApplicationRequest request)
        {
            var data = await _unitOfWork.CheckInCheckOutApplications.GetExportData(
                request.OrganizationId,
                request.EmployeeId,
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
                worksheet.Cell(rowIndex, 2).Value = row.Employee?.EmployeeCode ?? "";
                worksheet.Cell(rowIndex, 3).Value = $"{row.Employee?.LastName} {row.Employee?.FirstName}".Trim();
                worksheet.Cell(rowIndex, 4).Value = row.Date.ToString("dd/MM/yyyy");
                worksheet.Cell(rowIndex, 5).Value = row.TimeCheckIn.HasValue ? row.TimeCheckIn.Value.ToString(@"hh\:mm") : "";
                worksheet.Cell(rowIndex, 6).Value = row.TimeCheckOut.HasValue ? row.TimeCheckOut.Value.ToString(@"hh\:mm") : "";
                worksheet.Cell(rowIndex, 7).Value = row.Reason ?? "";
                worksheet.Cell(rowIndex, 8).Value = row.Description ?? "";
                worksheet.Cell(rowIndex, 9).Value = row.ShiftCatalog?.Name ?? "";
                worksheet.Cell(rowIndex, 10).Value = $"{row.Approver?.LastName} {row.Approver?.FirstName}".Trim();
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
    }
}

