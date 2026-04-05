using HRM_BE.Core.Exceptions;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.DetailTimeSheet;
using HRM_BE.Core.Models.SumaryTimeSheet;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace HRM_BE.Api.Controllers.DetailTimesheet
{
    [Route("api/detail-timesheet")]
    [ApiController]
    public class DetailTimesheetController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public DetailTimesheetController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("get-by-id")]
        public async Task<ApiResult<DetailTimeSheetDto>> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.DetailTimeSheets.GetById(request.Id);
            return ApiResult<DetailTimeSheetDto>.Success("Lấy thông tin bảng chi tiết chấm công thành công", result);
        }
        [HttpGet("statistic-detail-time-sheet")]
        public async Task<ApiResult<StatiscTimeSheetDto>> StatisticDetailTimeSheet([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.DetailTimeSheets.StatiscTimeSheetDto(request.Id);
            return ApiResult<StatiscTimeSheetDto>.Success("Thống kê chi tiết chấm công thành công", result);
        }
        [HttpGet("paging")]
        public async Task<ApiResult<PagingResult<DetailTimeSheetDto>>> Paging([FromQuery] GetDetailTimeSheetPagingRequest request)
        {
            var result = await _unitOfWork.DetailTimeSheets.Paging(request.Name, request.Month,request.Year, request.OrganizationId,request.StaffPositionId,request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<DetailTimeSheetDto>>.Success("Lấy danh sách thông tin chi tiết chấm công", result);
        } 
        [HttpGet("get-select")]
        public async Task<ApiResult<PagingResult<DetailTimeSheetDto>>> GetSelect([FromQuery] GetDetailTimeSheetPagingRequest request)
        {
            var result = await _unitOfWork.DetailTimeSheets.GetSelect(request.Name, request.Month,request.Year, request.OrganizationId,request.StaffPositionId,request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return ApiResult<PagingResult<DetailTimeSheetDto>>.Success("Lấy danh sách chọn thông tin chi tiết chấm công", result);
        } 
        [HttpGet("get-detail-time-sheet-with-time-sheet")]
        public async Task<ApiResult<PagingResult<GetDetailTimesheetWithEmployeeDto>>> DetailTimeSheetWithEmployee([FromQuery] GetDetailTimeSheetWithEmplopyeePagingRequest request)
        {
            var result = await _unitOfWork.DetailTimeSheets.DetailTimeSheetWithEmployeePaging(request.DetailTimeSheetId,request.KeyWord, request.OrganizationId,request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);


            return ApiResult<PagingResult<GetDetailTimesheetWithEmployeeDto>>.Success("Lấy danh sách thông tin chi tiết chấm công", result);
        }

        /// <summary>
        /// HRM - Xuất chấm công dạng bảng tháng giống mẫu.
        /// </summary>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] GetDetailTimeSheetWithEmplopyeePagingRequest request)
        {
            var detailTimesheet = await _unitOfWork.DetailTimeSheets.GetById(request.DetailTimeSheetId);
            if (detailTimesheet == null)
            {
                return NotFound(ApiResult<bool>.Failure("Không tìm thấy bảng chấm công chi tiết."));
            }

            if (!detailTimesheet.StartDate.HasValue || !detailTimesheet.EndDate.HasValue)
            {
                return BadRequest(ApiResult<bool>.Failure("Bảng chấm công chi tiết chưa có khoảng thời gian hợp lệ."));
            }

            var items = await _unitOfWork.DetailTimeSheets.DetailTimeSheetWithEmployee(
                request.DetailTimeSheetId,
                request.KeyWord,
                request.OrganizationId,
                request.SortBy,
                request.OrderBy);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("ChamCongChiTiet");

            var startDate = detailTimesheet.StartDate.Value.Date;
            var endDate = detailTimesheet.EndDate.Value.Date;
            var dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            var summaryHeaders = new[]
            {
                "Công ngày",
                "Nghỉ lễ",
                "Công 1/2 ngày",
                "Công 3/4 ngày",
                "Tổng số công thực tế đi làm",
                "Số ngày nghỉ hưởng lương",
                "Số ngày nghỉ trừ lương",
                "Tổng số công/tháng tính lương",
                "Ghi chú"
            };

            var firstDateColumn = 3;
            var firstSummaryColumn = firstDateColumn + dates.Count;
            var lastColumn = firstSummaryColumn + summaryHeaders.Length - 1;
            var organizationName = detailTimesheet.Organization?.OrganizationName
                ?? items.FirstOrDefault()?.OrganizationName
                ?? string.Empty;

            ws.Cells[1, 1, 1, lastColumn].Merge = true;
            ws.Cells[1, 1].Value = organizationName;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 12;
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            ws.Cells[2, 1, 2, lastColumn].Merge = true;
            ws.Cells[2, 1].Value = "BẢNG CHẤM CÔNG";
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.Font.Size = 18;
            ws.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells[3, 1, 3, lastColumn].Merge = true;
            ws.Cells[3, 1].Value = startDate.Month == endDate.Month && startDate.Year == endDate.Year
                ? $"Tháng {startDate:MM} năm {startDate:yyyy}"
                : $"Từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}";
            ws.Cells[3, 1].Style.Font.Bold = true;
            ws.Cells[3, 1].Style.Font.Italic = true;
            ws.Cells[3, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
            ws.Cells[3, 1].Style.Font.Size = 12;
            ws.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells[4, 1, 6, 1].Merge = true;
            ws.Cells[4, 1].Value = "STT";
            ws.Cells[4, 2, 6, 2].Merge = true;
            ws.Cells[4, 2].Value = "Họ và tên";

            ws.Cells[4, firstDateColumn, 4, firstDateColumn + dates.Count - 1].Merge = true;
            ws.Cells[4, firstDateColumn].Value = "Ngày tháng năm";

            for (var i = 0; i < summaryHeaders.Length; i++)
            {
                var column = firstSummaryColumn + i;
                ws.Cells[4, column, 6, column].Merge = true;
                ws.Cells[4, column].Value = summaryHeaders[i];
            }

            for (var i = 0; i < dates.Count; i++)
            {
                var date = dates[i];
                var column = firstDateColumn + i;
                ws.Cells[5, column].Value = GetVietnameseDayShortName(date.DayOfWeek);
                ws.Cells[6, column].Value = date.Day.ToString("00");
            }

            var row = 7;
            var totals = new MonthlyExportTotals();

            foreach (var employee in items)
            {
                var timesheetDays = employee.Timesheets.ToDictionary(x => x.Date!.Value.Date);
                var employeeStats = new MonthlyEmployeeStats();
                var overtimeDates = new List<DateTime>();
                var fullWorkDates = new List<DateTime>();
                var partialWorkDates = new List<string>();
                var holidayDates = new List<DateTime>();

                ws.Cells[row, 1].Value = row - 6;
                ws.Cells[row, 2].Value = $"{employee.LastName} {employee.FirstName}".Trim();

                for (var i = 0; i < dates.Count; i++)
                {
                    var date = dates[i];
                    var column = firstDateColumn + i;
                    timesheetDays.TryGetValue(date, out var dayData);
                    var shifts = dayData?.Shifts ?? new List<ShiftDetailDto>();
                    var cellCode = BuildMonthlyCellCode(employee, date, shifts);

                    ws.Cells[row, column].Value = cellCode;
                    ws.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    employeeStats.ApplyCell(cellCode);

                    var dateLabel = date.ToString("dd/MM");
                    if (cellCode == "x")
                    {
                        fullWorkDates.Add(date);
                    }
                    else if (cellCode == "1/2" || cellCode == "3/4")
                    {
                        partialWorkDates.Add($"{dateLabel} ({cellCode})");
                    }
                    else if (cellCode == "L")
                    {
                        holidayDates.Add(date);
                    }

                    if (shifts.Any(x => x.IsOvertime || (x.OvertimeHours ?? 0d) > 0d))
                    {
                        overtimeDates.Add(date);
                    }
                }

                var paidLeaveDays = GetPaidLeaveDays(employee, dates);
                var unpaidLeaveDays = employeeStats.UnpaidLeaveDays;
                var workedDays = employeeStats.WorkDays;
                var halfDays = employeeStats.HalfDays;
                var threeQuarterDays = employeeStats.ThreeQuarterDays;
                var holidayDays = employeeStats.HolidayDays;
                var paidAbsenceDays = holidayDays + paidLeaveDays;
                var actualWorkedDays = Math.Round(workedDays + (halfDays * 0.5) + (threeQuarterDays * 0.75), 2);
                var payrollDays = Math.Round(actualWorkedDays + paidAbsenceDays, 2);

                ws.Cells[row, firstSummaryColumn + 0].Value = workedDays;
                ws.Cells[row, firstSummaryColumn + 1].Value = holidayDays;
                ws.Cells[row, firstSummaryColumn + 2].Value = halfDays;
                ws.Cells[row, firstSummaryColumn + 3].Value = threeQuarterDays;
                ws.Cells[row, firstSummaryColumn + 4].Value = actualWorkedDays;
                ws.Cells[row, firstSummaryColumn + 5].Value = paidAbsenceDays;
                ws.Cells[row, firstSummaryColumn + 6].Value = unpaidLeaveDays;
                ws.Cells[row, firstSummaryColumn + 7].Value = payrollDays;
                ws.Cells[row, firstSummaryColumn + 8].Value = BuildMonthlyNote(overtimeDates, fullWorkDates, partialWorkDates, holidayDates);
                ws.Cells[row, firstSummaryColumn + 8].Style.WrapText = true;
                ws.Cells[row, firstSummaryColumn + 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                ws.Cells[row, firstSummaryColumn + 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                totals.WorkDays += workedDays;
                totals.HolidayDays += holidayDays;
                totals.HalfDays += halfDays;
                totals.ThreeQuarterDays += threeQuarterDays;
                totals.ActualWorkedDays += actualWorkedDays;
                totals.PaidAbsenceDays += paidAbsenceDays;
                totals.UnpaidLeaveDays += unpaidLeaveDays;
                totals.PayrollDays += payrollDays;

                row++;
            }

            ws.Cells[row, 1].Value = "Cộng";
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, firstSummaryColumn + 0].Value = totals.WorkDays;
            ws.Cells[row, firstSummaryColumn + 1].Value = totals.HolidayDays;
            ws.Cells[row, firstSummaryColumn + 2].Value = totals.HalfDays;
            ws.Cells[row, firstSummaryColumn + 3].Value = totals.ThreeQuarterDays;
            ws.Cells[row, firstSummaryColumn + 4].Value = Math.Round(totals.ActualWorkedDays, 2);
            ws.Cells[row, firstSummaryColumn + 5].Value = Math.Round(totals.PaidAbsenceDays, 2);
            ws.Cells[row, firstSummaryColumn + 6].Value = Math.Round(totals.UnpaidLeaveDays, 2);
            ws.Cells[row, firstSummaryColumn + 7].Value = Math.Round(totals.PayrollDays, 2);
            ws.Cells[row, firstSummaryColumn + 8].Value = string.Empty;

            ApplyMonthlyExportStyles(ws, row, lastColumn, firstDateColumn, dates.Count, firstSummaryColumn, summaryHeaders.Length);

            var fileName = $"ChamCongChiTiet_{detailTimesheet.TimekeepingSheetName?.Replace('/', '_') ?? "BangChamCong"}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("get-detail-time-sheet")]
        public async Task<ApiResult<List<GetDetailTimesheetWithEmployeeDto>>> DetailTimeSheetWithEmployeeNoPaging(
            [FromQuery] int detailTimeSheetId,
            [FromQuery] string? keyWord,
            [FromQuery] int? organizationId,
            [FromQuery] string? sortBy,
            [FromQuery] string? orderBy)
        {
            var result = await _unitOfWork.DetailTimeSheets.DetailTimeSheetWithEmployee(
                detailTimeSheetId,
                keyWord,
                organizationId,
                sortBy,
                orderBy);

            return ApiResult<List<GetDetailTimesheetWithEmployeeDto>>.Success("Lấy thông tin chi tiết chấm công ", result);
        }

        private static string BuildMonthlyCellCode(
            GetDetailTimesheetWithEmployeeDto employee,
            DateTime date,
            IReadOnlyCollection<ShiftDetailDto> shifts)
        {
            var holiday = employee.Holidays.FirstOrDefault(h => date >= h.FromDate.Date && date <= h.ToDate.Date);
            if (holiday != null)
            {
                return "L";
            }

            var paidLeave = employee.PermittedLeaves.Any(p => p.Date.Any(d => d.Date == date.Date));
            var hasTimesheet = shifts.Count > 0;
            var totalWorkingHours = shifts.Sum(x => x.NumberOfWorkingHour ?? 0d);

            if (!hasTimesheet)
            {
                return paidLeave ? "P" : "N";
            }

            if (shifts.Any(x => x.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.LeaveNotPermission))
            {
                return "N";
            }

            if (shifts.Any(x => x.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.LeavePermission))
            {
                return "P";
            }

            if (totalWorkingHours >= 7.5)
            {
                return "x";
            }

            if (totalWorkingHours >= 5.5)
            {
                return "3/4";
            }

            if (totalWorkingHours > 0)
            {
                return "1/2";
            }

            return paidLeave ? "P" : "N";
        }

        private static string GetVietnameseDayShortName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "CN",
                DayOfWeek.Monday => "T2",
                DayOfWeek.Tuesday => "T3",
                DayOfWeek.Wednesday => "T4",
                DayOfWeek.Thursday => "T5",
                DayOfWeek.Friday => "T6",
                DayOfWeek.Saturday => "T7",
                _ => string.Empty
            };
        }

        private static double GetPaidLeaveDays(GetDetailTimesheetWithEmployeeDto employee, IReadOnlyCollection<DateTime> dates)
        {
            return employee.PermittedLeaves
                .SelectMany(x => x.Date)
                .Select(d => d.Date)
                .Where(d => dates.Contains(d))
                .Distinct()
                .Count();
        }

        private static string BuildMonthlyNote(
            IReadOnlyCollection<DateTime> overtimeDates,
            IReadOnlyCollection<DateTime> fullWorkDates,
            IReadOnlyCollection<string> partialWorkDates,
            IReadOnlyCollection<DateTime> holidayDates)
        {
            var parts = new List<string>();

            if (overtimeDates.Count > 0)
            {
                parts.Add($"Tăng ca: {FormatDateRanges(overtimeDates)}");
            }

            if (fullWorkDates.Count > 0)
            {
                parts.Add($"Đủ công: {FormatDateRanges(fullWorkDates)}");
            }

            if (partialWorkDates.Count > 0)
            {
                var uniquePartial = partialWorkDates
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                parts.Add($"Thiếu công: {string.Join(", ", uniquePartial)}");
            }

            if (holidayDates.Count > 0)
            {
                parts.Add($"Lễ: {FormatDateRanges(holidayDates)}");
            }

            return string.Join(Environment.NewLine, parts);
        }

        private static string FormatDateRanges(IEnumerable<DateTime> dates)
        {
            var sorted = dates
                .Select(x => x.Date)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (sorted.Count == 0)
            {
                return string.Empty;
            }

            var ranges = new List<string>();
            var rangeStart = sorted[0];
            var rangeEnd = sorted[0];

            for (var i = 1; i < sorted.Count; i++)
            {
                var current = sorted[i];
                if (current == rangeEnd.AddDays(1))
                {
                    rangeEnd = current;
                    continue;
                }

                ranges.Add(FormatRange(rangeStart, rangeEnd));
                rangeStart = current;
                rangeEnd = current;
            }

            ranges.Add(FormatRange(rangeStart, rangeEnd));
            return string.Join(", ", ranges);
        }

        private static string FormatRange(DateTime from, DateTime to)
        {
            return from == to
                ? from.ToString("dd/MM")
                : $"{from:dd/MM}-{to:dd/MM}";
        }

        private static void ApplyMonthlyExportStyles(
            ExcelWorksheet ws,
            int lastDataRow,
            int lastColumn,
            int firstDateColumn,
            int dateColumnCount,
            int firstSummaryColumn,
            int summaryColumnCount)
        {
            var headerRange = ws.Cells[4, 1, 6, lastColumn];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headerRange.Style.WrapText = true;

            ws.Cells[4, 1, lastDataRow, lastColumn].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[4, 1, lastDataRow, lastColumn].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[4, 1, lastDataRow, lastColumn].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[4, 1, lastDataRow, lastColumn].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            ws.Cells[4, firstDateColumn, 6, firstDateColumn + dateColumnCount - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[4, firstDateColumn, 6, firstDateColumn + dateColumnCount - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 242, 204));

            ws.Cells[4, firstSummaryColumn + 4, lastDataRow, firstSummaryColumn + 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[4, firstSummaryColumn + 4, lastDataRow, firstSummaryColumn + 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(221, 235, 247));

            ws.Cells[4, firstSummaryColumn + 7, lastDataRow, firstSummaryColumn + 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[4, firstSummaryColumn + 7, lastDataRow, firstSummaryColumn + 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(198, 224, 180));

            ws.Cells[lastDataRow, 1, lastDataRow, lastColumn].Style.Font.Bold = true;
            ws.Cells[lastDataRow, 1, lastDataRow, lastColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[lastDataRow, 1, lastDataRow, lastColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));

            ws.Column(1).Width = 6;
            ws.Column(2).Width = 22;

            for (var column = firstDateColumn; column < firstDateColumn + dateColumnCount; column++)
            {
                ws.Column(column).Width = 4;
            }

            for (var column = firstSummaryColumn; column < firstSummaryColumn + summaryColumnCount; column++)
            {
                ws.Column(column).Width = column == firstSummaryColumn + summaryColumnCount - 1 ? 48 : 12;
            }

            ws.View.FreezePanes(7, 3);
        }

        private sealed class MonthlyEmployeeStats
        {
            public int WorkDays { get; private set; }
            public int HolidayDays { get; private set; }
            public int HalfDays { get; private set; }
            public int ThreeQuarterDays { get; private set; }
            public int UnpaidLeaveDays { get; private set; }

            public void ApplyCell(string code)
            {
                switch (code)
                {
                    case "x":
                        WorkDays++;
                        break;
                    case "L":
                        HolidayDays++;
                        break;
                    case "1/2":
                        HalfDays++;
                        break;
                    case "3/4":
                        ThreeQuarterDays++;
                        break;
                    case "N":
                        UnpaidLeaveDays++;
                        break;
                }
            }
        }

        private sealed class MonthlyExportTotals
        {
            public int WorkDays { get; set; }
            public int HolidayDays { get; set; }
            public int HalfDays { get; set; }
            public int ThreeQuarterDays { get; set; }
            public double ActualWorkedDays { get; set; }
            public double PaidAbsenceDays { get; set; }
            public double UnpaidLeaveDays { get; set; }
            public double PayrollDays { get; set; }
        }
        
        [HttpPost("create")]
        public async Task<ApiResult<DetailTimeSheetDto>> Create([FromBody] CreateDetailTimeSheetRequest request)
        {
            var result = await _unitOfWork.DetailTimeSheets.Create(request);
            return ApiResult<DetailTimeSheetDto>.Success("Thêm bảng chi tiết chấm công thành công", result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> update(int shiftWorkId, [FromBody] UpdateDetailTimeSheetRequest request)
        {
            await _unitOfWork.DetailTimeSheets.Update(shiftWorkId, request);
            return Ok(ApiResult<bool>.Success("Cập nhật chi tiết chấm công thành công", true));
        }
        [HttpPut("Lock")]
        public async Task<IActionResult> Lock(int shiftWorkId, [FromBody] UpdateLockDetailTimeSheetRequest request )
        {
            await _unitOfWork.DetailTimeSheets.LockDetailTimeSheet(shiftWorkId,request.IsLock);
            return Ok(ApiResult<bool>.Success("Khóa chi tiết chấm công thành công", true));
        }
        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            try
            {
                await _unitOfWork.DetailTimeSheets.Delete(request.Id);
                return Ok(ApiResult<bool>.Success("Xoá chi tiết chấm công thành công", true));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResult<bool>.Failure(ex.Message, false));
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ApiResult<bool>.Failure(ex.Message, false));
            }
        }
    }
}
