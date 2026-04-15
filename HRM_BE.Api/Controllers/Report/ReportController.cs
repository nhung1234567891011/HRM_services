using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Report;
using Microsoft.AspNetCore.Mvc;

namespace HRM_BE.Api.Controllers.Report
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Báo cáo phân bổ nhân sự
        /// </summary>
        [HttpGet("hr-distribution")]
        public async Task<ApiResult<HrDistributionReportDto>> GetHrDistributionReport([FromQuery] int? organizationId)
        {
            var result = await _unitOfWork.Reports.GetHrDistributionReport(organizationId);
            return ApiResult<HrDistributionReportDto>.Success("Lấy báo cáo phân bổ nhân sự thành công", result);
        }

        /// <summary>
        /// Báo cáo tổng hợp thu nhập theo tháng
        /// </summary>
        [HttpGet("monthly-income")]
        public async Task<ApiResult<MonthlyIncomeReportDto>> GetMonthlyIncomeReport([FromQuery] int? organizationId, [FromQuery] int year)
        {
            var result = await _unitOfWork.Reports.GetMonthlyIncomeReport(organizationId, year);
            return ApiResult<MonthlyIncomeReportDto>.Success("Lấy báo cáo thu nhập thành công", result);
        }

        /// <summary>
        /// Báo cáo hiệu suất
        /// </summary>
        [HttpGet("performance")]
        public async Task<ApiResult<PerformanceReportDto>> GetPerformanceReport(
            [FromQuery] int? organizationId,
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] int? fromYear,
            [FromQuery] int? fromMonth,
            [FromQuery] int? toYear,
            [FromQuery] int? toMonth)
        {
            var result = await _unitOfWork.Reports.GetPerformanceReport(
                organizationId,
                year,
                month,
                fromYear,
                fromMonth,
                toYear,
                toMonth);
            return ApiResult<PerformanceReportDto>.Success("Lấy báo cáo hiệu suất thành công", result);
        }

        /// <summary>
        /// Báo cáo chuyên cần
        /// </summary>
        [HttpGet("attendance")]
        public async Task<ApiResult<AttendanceReportDto>> GetAttendanceReport([FromQuery] int? organizationId, [FromQuery] int year, [FromQuery] int? month)
        {
            var result = await _unitOfWork.Reports.GetAttendanceReport(organizationId, year, month);
            return ApiResult<AttendanceReportDto>.Success("Lấy báo cáo chuyên cần thành công", result);
        }
    }
}
