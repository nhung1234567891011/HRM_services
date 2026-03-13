using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.SumaryTimeSheet;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace HRM_BE.Api.Controllers.SummaryTimeSheet
{
    /// <summary>
    /// API chấm công tổng hợp.
    /// </summary>
    [Route("api/summary-time-sheet")]
    [ApiController]
    public class SummaryTimeSheetController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Khởi tạo controller.
        /// </summary>
        public SummaryTimeSheetController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy bảng công tổng hợp theo Id.
        /// </summary>
        [HttpGet("get-by-id")]
        public async Task<ApiResult<SummaryTimeSheetDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.SummaryTimeSheets.GetById(request.Id);
            return ApiResult<SummaryTimeSheetDto>.Success("Lấy thông tin bảng công tổng hợp thành công", result);
        }
        /// <summary>
        /// HRM-Lấy chấm công tổng hợp theo nhân viên
        /// </summary> 
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-summary-time-sheet-with-employee")]
        public async Task<ApiResult<PagingResult<GetSummaryTimeSheetWithEmployeeDto>>> GetSummaryTimeSheetWithEmployeePaging([FromQuery] GetSummarySheetWorkWithEmployeePagingRequest request)
        {
            var result = await _unitOfWork.SummaryTimeSheets.GetSummaryTimeSheetPaging(request.Id,request.OrganizationId,request.KeyWord,request.StaffPositionId,request.SortBy,request.OrderBy,request.PageIndex,request.PageSize);

            result.Items ??= [];
            await EnrichSummaryTimeSheetWithEmployeeItems(result.Items, request.OrganizationId);
            return ApiResult<PagingResult<GetSummaryTimeSheetWithEmployeeDto>>.Success("Lấy chi tiết chấm công tổng hợp thành công",result);
        }

        /// <summary>
        /// HRM - Kết xuất Excel chấm công tổng hợp theo nhân viên
        /// </summary>
        [HttpGet("export-summary-time-sheet-with-employee")]
        public async Task<IActionResult> ExportSummaryTimeSheetWithEmployeeExcel(
            [FromQuery] GetSummarySheetWorkWithEmployeeExportRequest request)
        {
            var items = await _unitOfWork.SummaryTimeSheets.GetSummaryTimeSheetWithEmployeeList(
                request.Id,
                request.OrganizationId,
                request.KeyWord,
                request.StaffPositionId,
                request.SortBy,
                request.OrderBy);

            await EnrichSummaryTimeSheetWithEmployeeItems(items, request.OrganizationId);

            // EPPlus license context (cần để chạy được từ EPPlus v5+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("ChamCongTongHop");

            var headers = new[]
            {
                "STT",
                "Mã NV",
                "Họ và tên",
                "Phòng ban",
                "Chức vụ",
                "Từ ngày",
                "Đến ngày",
                "Ngày công chuẩn",
                "Ngày nghỉ (phép + lễ)",
                "Số ngày chấm công (distinct)",
                "Tổng giờ",
                "Quy đổi công",
                "Trạng thái"
            };

            for (int c = 0; c < headers.Length; c++)
            {
                ws.Cells[1, c + 1].Value = headers[c];
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var row = i + 2;

                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = item.EmployeeCode;
                ws.Cells[row, 3].Value = $"{item.LastName} {item.FirstName}".Trim();
                ws.Cells[row, 4].Value = item.Organization.OrganizationName;
                ws.Cells[row, 5].Value = item.StaffPosition.PositionName;
                ws.Cells[row, 6].Value = item.StartDate;
                ws.Cells[row, 7].Value = item.EndDate;
                ws.Cells[row, 8].Value = item.TotalWorkingDay;
                ws.Cells[row, 9].Value = item.TotalLeaveDay;
                ws.Cells[row, 10].Value = item.DatePerMonth;
                ws.Cells[row, 11].Value = item.TotalHour;
                ws.Cells[row, 12].Value = item.EqualDay;
                ws.Cells[row, 13].Value = GetVietnameseConfirmStatus(item.Status);
            }

            // Format
            ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
            ws.Cells[1, 1, items.Count + 1, headers.Length].AutoFitColumns();
            ws.Column(6).Style.Numberformat.Format = "dd/MM/yyyy";
            ws.Column(7).Style.Numberformat.Format = "dd/MM/yyyy";
            ws.View.FreezePanes(2, 1);

            // Add table (filter + style)
            var totalRows = Math.Max(1, items.Count + 1);
            var tableRange = ws.Cells[1, 1, totalRows, headers.Length];
            var table = ws.Tables.Add(tableRange, "ChamCongTongHopTable");
            table.TableStyle = TableStyles.Medium2;

            var fileName = $"ChamCongTongHop_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private static string GetVietnameseConfirmStatus(SummaryTimesheetNameEmployeeConfirmStatus status)
        {
            return status switch
            {
                SummaryTimesheetNameEmployeeConfirmStatus.None => "Chưa gửi xác nhận",
                SummaryTimesheetNameEmployeeConfirmStatus.Pending => "Đang xác nhận",
                SummaryTimesheetNameEmployeeConfirmStatus.Reject => "Bị từ chối",
                SummaryTimesheetNameEmployeeConfirmStatus.Confirm => "Đã xác nhận",
                SummaryTimesheetNameEmployeeConfirmStatus.SendedNotConfirm => "Chưa xác nhận",
                _ => status.ToString()
            };
        }

        private async Task EnrichSummaryTimeSheetWithEmployeeItems(List<GetSummaryTimeSheetWithEmployeeDto>? items, int organizationId)
        {
            if (items is null || items.Count == 0) return;

            var shiftCatalog = _unitOfWork.ShiftCatalogs.Find(g => g.OrganizationId == organizationId).FirstOrDefault();
            if (shiftCatalog is null || shiftCatalog.WorkingHours is null || shiftCatalog.WorkingHours.Value <= 0)
            {
                // Không có cấu hình ca làm => không thể tính chuẩn (giữ nguyên mặc định 0)
                return;
            }

            var shiftWorks = _unitOfWork.ShiftWorks.Find(s => s.ShiftCatalogId == shiftCatalog.Id).ToList();
            var totalWorkingDay = (double)shiftWorks.Sum(s => s.TotalWork ?? 0);

            foreach (var item in items)
            {
                var totalLeaveDay = await _unitOfWork.LeaveApplications.GetTotalLeaveEmployee(item.StartDate, item.EndDate, item.Id);

                var workingHours = _unitOfWork.Timesheet.Find(t => t.EmployeeId == item.Id);
                var totalWorkingHours = workingHours
                    .Where(t => t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None)
                    .Sum(t => t.NumberOfWorkingHour) ?? 0;

                var totalHoliday = await _unitOfWork.Holiday.GetNumberHoliday(item.StartDate, item.EndDate, organizationId);
                var totalHourHoliday = totalHoliday * shiftCatalog.WorkingHours.Value; // số giờ nghỉ lễ

                var paidLeaveHours = totalLeaveDay * shiftCatalog.WorkingHours.Value;

                item.TotalLeaveDay = totalLeaveDay + totalHoliday;
                item.TotalWorkingDay = totalWorkingDay;

                item.TotalHour = totalWorkingHours + paidLeaveHours + totalHourHoliday;

                item.EqualDay = item.TotalHour > 0
                    ? (item.TotalHour / shiftCatalog.WorkingHours.Value)
                    : 0;
            }
        }
        /// <summary>
        /// APi dùng để get chấm công tổng hợp cho màn bảng lương
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-summary-select")]
        public async Task<ApiResult<List<GetSelectSummaryTimeSheetDto>>> GetSummaryTimeSheetSelect()
        {
            var result = await _unitOfWork.SummaryTimeSheets.GetSelectSummaryTimeSheet();

            return ApiResult<List<GetSelectSummaryTimeSheetDto>>.Success("Lấy chi tiết chấm công tổng hợp thành công", result);
        }

        /// <summary>
        /// API dùng để get chấm công tổng hợp cho màn tạo bảng lương.
        /// (FE đang gọi: /api/summary-time-sheet/get-summary-select-for-payroll)
        /// </summary>
        /// <param name="organizationId">Tuỳ chọn. Nếu không truyền sẽ lấy theo OrganizationId trong token.</param>
        /// <param name="staffPositionIds">Tuỳ chọn. Danh sách id phân tách bởi dấu phẩy, ví dụ: "1,2,3".</param>
        [HttpGet("get-summary-select-for-payroll")]
        public async Task<ApiResult<List<GetSelectSummaryTimeSheetDto>>> GetSummaryTimeSheetSelectForPayroll(
            [FromQuery] int? organizationId,
            [FromQuery] string? staffPositionIds)
        {
            var result = await _unitOfWork.SummaryTimeSheets.GetSelectSummaryTimeSheetForPayroll(organizationId, staffPositionIds);
            return ApiResult<List<GetSelectSummaryTimeSheetDto>>.Success("Lấy danh sách bảng chấm công tổng hợp cho tạo bảng lương thành công", result);
        }
        /// <summary>
        /// HRM-Phân trang
        /// </summary> 
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<SummaryTimeSheetDto>>> Paging([FromQuery] GetSummaryTimeSheetPagingRequest request)
        {
            var result = await _unitOfWork.SummaryTimeSheets.Paging(request.SummaryTimesheetId,request.Name,request.Month,request.Year, request.OrganizationId,request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<SummaryTimeSheetDto>>.Success("Lấy danh sách thông tin bảng công tổng hợp", result);
        }
        /// <summary>
        /// Tạo bảng công tổng hợp.
        /// </summary>
        [HttpPost("create")]
        public async Task<ApiResult<SummaryTimeSheetDto>> Create([FromBody] CreateSummaryTimesheetRequest request)
        {
            var result = await _unitOfWork.SummaryTimeSheets.Create(request);
            return ApiResult<SummaryTimeSheetDto>.Success("Thêm bảng công tổng hợp thành công", result);
        }
        /// <summary>
        /// Cập nhật bảng công tổng hợp.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update(int shiftWorkId, [FromBody] UpdateSummaryTimeSheetRequest request)
        {
            await _unitOfWork.SummaryTimeSheets.Update(shiftWorkId, request);
            return Ok(ApiResult<bool>.Success("Cập nhật bảng công tổng hợp thành công", true));
        }

        /// <summary>
        /// Xoá bảng công tổng hợp.
        /// </summary>
        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            await _unitOfWork.SummaryTimeSheets.Delete(request.Id);
            return Ok(ApiResult<bool>.Success("Xoá bảng công tổng hợp thành công", true));
        }
    }
}
