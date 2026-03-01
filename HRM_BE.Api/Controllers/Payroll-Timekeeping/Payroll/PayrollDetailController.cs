using Azure.Core;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Mail;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using HRM_BE.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HRM_BE.Api.Controllers.Payroll_Timekeeping.Payroll
{
    [Route("api/payroll-detail")]
    [ApiController]
    public class PayrollDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public PayrollDetailController(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        /// <summary>
        /// HRM-là Admin, tôi muốn xem danh sách bảng lương chi tiết
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("paging")]
        public async Task<PagingResult<PayrollDetailDto>> Paging([FromQuery] PagingPayrollDetailRequest request)
        {
            var result = await _unitOfWork.PayrollDetails.Paging(request.OrganizationId, request.Name, request.PayrollId, request.EmployeeId, request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return result;
        }

        [HttpGet("get-by-id")]
        public async Task<PayrollDetailDto> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var result = await _unitOfWork.PayrollDetails.GetById(request.Id);
            return result;
        }

        [HttpPost("calculate-and-save-payroll-details")]
        public async Task<IActionResult> CalculateAndSavePayrollDetails([FromQuery] int payrollId)
        {
            try
            {
                await _unitOfWork.PayrollDetails.CalculateAndSavePayrollDetails(payrollId);
                return Ok(ApiResult<bool>.Success("Tính toán bảng lương chi tiết thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        /// <summary>
        /// HRM - Cập nhật (tính lại) phiếu lương cho 1 bảng lương.
        /// FE đang dùng route: payroll-detail/recalculate-and-save-payroll-details
        /// </summary>
        [HttpPost("recalculate-and-save-payroll-details")]
        public async Task<IActionResult> RecalculateAndSavePayrollDetails([FromQuery] int payrollId)
        {
            try
            {
                await _unitOfWork.PayrollDetails.RecalculateAndSavePayrollDetails(payrollId);
                return Ok(ApiResult<bool>.Success("Cập nhật phiếu lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        /// <summary>
        /// HRM - Sửa phiếu lương (PayrollDetail)
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] UpdatePayrollDetailRequest request)
        {
            try
            {
                await _unitOfWork.PayrollDetails.Update(id, request);
                return Ok(ApiResult<bool>.Success("Cập nhật phiếu lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        /// <summary>
        /// HRM - Xóa phiếu lương (soft delete)
        /// </summary>
        [HttpPut("delete")]
        public async Task<IActionResult> Delete([FromQuery] EntityIdentityRequest<int> request)
        {
            try
            {
                await _unitOfWork.PayrollDetails.Delete(request.Id);
                return Ok(ApiResult<bool>.Success("Xóa phiếu lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        /// <summary>
        /// HRM - Xóa nhiều phiếu lương (soft delete)
        /// </summary>
        [HttpPut("delete-range")]
        public async Task<IActionResult> DeleteRange([FromBody] ListIntRequest request)
        {
            try
            {
                await _unitOfWork.PayrollDetails.DeleteRange(request.Ids ?? new List<int>());
                return Ok(ApiResult<bool>.Success("Xóa phiếu lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        [HttpPost("fetch-payroll-details")]
        public async Task<List<PayrollDetailDto>> FetchPayrollDetails(int payrollId)
        {
            var result = await _unitOfWork.PayrollDetails.FetchPayrollDetails(payrollId);
            return result;
        }

        /// <summary>
        /// Quản lý gửi bảng lương cho nhân viên xem
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("send-payroll-detail-confirmation")]
        public async Task<IActionResult> SendPayrollDetailConfirmation([FromBody] UpdateSendPayrollDetailConfirmationRequest request)
        {
            try
            {
                await _unitOfWork.PayrollDetails.SendPayrollDetailConfirmation(request);

                var firstDetail = await _unitOfWork.PayrollDetails.GetById(request.PayrollDetailIds[0]);
                var payrollId = firstDetail.PayrollId;

                var page = await _unitOfWork.PayrollDetails.Paging(
                    organizationId: null,
                    name: null,
                    payrollId: payrollId,
                    employeeId: null,
                    sortBy: null,
                    orderBy: null,
                    pageIndex: 1,
                    pageSize: int.MaxValue);

                var detailsToSend = (page.Items ?? new List<PayrollDetailDto>())
                    .Where(d => request.PayrollDetailIds.Contains(d.Id))
                    .ToList();

                foreach (var payrollDetail in detailsToSend)
                {
                    string? email = null;
                    if (payrollDetail.EmployeeId.HasValue)
                    {
                        var employeeDto = await _unitOfWork.Employees.GetById(payrollDetail.EmployeeId.Value);
                        email = employeeDto.CompanyEmail ?? employeeDto.AccountEmail ?? employeeDto.PersonalEmail;
                    }

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        continue;
                    }

                    await _mailService.SendMail(new SendMailRequest
                    {
                        ToEmail = email,
                        Subject = $"Phiếu lương - {payrollDetail.FullName}",
                        Body = BuildPayrollDetailEmailBody(payrollDetail)
                    });
                }

                return Ok(ApiResult<bool>.Success("Gửi xác nhận bảng lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }

        private static string BuildPayrollDetailEmailBody(PayrollDetailDto payrollDetail)
        {
            var sb = new StringBuilder();
            sb.Append("<div style='font-family: Arial, sans-serif; font-size: 14px;'>");
            sb.Append($"<p>Chào {System.Net.WebUtility.HtmlEncode(payrollDetail.FullName ?? "Anh/Chị")},</p>");
            sb.Append("<p>Phiếu lương của Anh/Chị đã được gửi để xác nhận. Thông tin tóm tắt:</p>");

            sb.Append("<table cellpadding='6' cellspacing='0' style='border-collapse: collapse; border: 1px solid #ddd;'>");
            AppendRow(sb, "Mã nhân viên", payrollDetail.EmployeeCode);
            AppendRow(sb, "Lương cơ bản", payrollDetail.BaseSalary);
            AppendRow(sb, "Ngày công chuẩn", payrollDetail.StandardWorkDays);
            AppendRow(sb, "Ngày công thực tế", payrollDetail.ActualWorkDays);
            AppendRow(sb, "Lương theo công", payrollDetail.ReceivedSalary);
            AppendRow(sb, "Lương KPI", payrollDetail.KpiSalary);
            AppendRow(sb, "Thưởng", payrollDetail.Bonus);
            AppendRow(sb, "Tổng lương", payrollDetail.TotalSalary);
            AppendRow(sb, "Thực nhận", payrollDetail.TotalReceivedSalary);
            AppendRow(sb, "Hạn phản hồi", payrollDetail.ResponseDeadline?.ToString("yyyy-MM-dd"));
            sb.Append("</table>");

            sb.Append("<p>Vui lòng đăng nhập hệ thống HRM để xem chi tiết và xác nhận phiếu lương.</p>");
            sb.Append("<p>Trân trọng.</p>");
            sb.Append("</div>");
            return sb.ToString();
        }

        private static void AppendRow(StringBuilder sb, string label, object? value)
        {
            sb.Append("<tr>");
            sb.Append($"<td style='border: 1px solid #ddd; font-weight: bold;'>{System.Net.WebUtility.HtmlEncode(label)}</td>");
            sb.Append($"<td style='border: 1px solid #ddd;'>{System.Net.WebUtility.HtmlEncode(value?.ToString() ?? string.Empty)}</td>");
            sb.Append("</tr>");
        }

        /// <summary>
        /// Nhân viên xem và xác nhận bảng lương
        /// </summary>
        /// <param name="payrollDetailId"></param>
        /// <returns></returns>
        [HttpPost("confirm-payroll-detail-by-employee")]
        public async Task<IActionResult> ConfirmPayrollDetailByEmployee([FromQuery] int payrollDetailId)
        {
            try
            {
                await _unitOfWork.PayrollDetails.ConfirmPayrollDetailByEmployee(payrollDetailId);
                return Ok(ApiResult<bool>.Success("Nhân viên xác nhận bảng lương thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(ApiResult<bool>.Failure(ex.Message, false));
            }
        }
    }
}
