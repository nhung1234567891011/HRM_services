using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class PayrollDetailRepository : RepositoryBase<PayrollDetail, int>, IPayrollDetailRepository
    {
        private readonly IMapper _mapper;

        public PayrollDetailRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PayrollDetailDto> GetById(int id)
        {
            var entity = await GetPayrollDetailAndCheckExist(id);
            return _mapper.Map<PayrollDetailDto>(entity);
        }

        public async Task<PagingResult<PayrollDetailDto>> Paging(int? organizationId, string? name, int? payrollId, int? employeeId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.PayrollDetails.Where(p => p.IsDeleted != true).AsQueryable();

            if (payrollId.HasValue)
            {

                query = query.Where(p => p.PayrollId == payrollId);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(p => p.EmployeeId == employeeId);
            }

            if (organizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == organizationId);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                var pattern = $"%{keyword}%";

                query = query.Where(p =>
                    (p.FullName != null && EF.Functions.Like(p.FullName, pattern)) ||
                    (p.EmployeeCode != null && EF.Functions.Like(p.EmployeeCode, pattern)) ||
                    (p.Employee != null && (
                        (p.Employee.PhoneNumber != null && EF.Functions.Like(p.Employee.PhoneNumber, pattern)) ||
                        (p.Employee.WorkPhoneNumber != null && EF.Functions.Like(p.Employee.WorkPhoneNumber, pattern)) ||
                        (p.Employee.StaffPosition != null && p.Employee.StaffPosition.PositionName != null && EF.Functions.Like(p.Employee.StaffPosition.PositionName, pattern)) ||
                        (p.Employee.StaffTitle != null && p.Employee.StaffTitle.StaffTitleName != null && EF.Functions.Like(p.Employee.StaffTitle.StaffTitleName, pattern))
                    ))
                );
            }

            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<PayrollDetailDto>(query).ToListAsync();

            var result = new PagingResult<PayrollDetailDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }


        private async Task<PayrollDetail> GetPayrollDetailAndCheckExist(int payrollDetailId)
        {
            var payrollDetail = await _dbContext.PayrollDetails.FindAsync(payrollDetailId);
            if (payrollDetail is null)
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {payrollDetailId}");
            return payrollDetail;
        }

        public async Task CalculateAndSavePayrollDetails(int payrollId)
        {
            // Lấy Payroll
            var payroll = _dbContext.Payrolls.FirstOrDefault(p => p.Id == payrollId);
            if (payroll == null)
            {
                throw new Exception($"Payroll có Id = {payrollId} không tồn tại!");
            }

            // 1. Lấy danh sách nhân viên thuộc tổ chức của Payroll
            var employees = _dbContext.Employees
                .Where(e => e.OrganizationId == payroll.OrganizationId)
                .Where(e => e.Contracts.Any(c => c.EmployeeId == e.Id))
                .ToList();

            var standardWorkDays = _dbContext.ShiftWorks
                .Where(sw => sw.OrganizationId == payroll.OrganizationId)
                .Sum(sw => sw.TotalWork);

            var payrollDetails = new List<PayrollDetail>();

            foreach (var employee in employees)
            {
                var contract = _dbContext.Contracts.FirstOrDefault(c => c.EmployeeId == employee.Id);
                var timesheet = _dbContext.Timesheets.Where(t => t.EmployeeId == employee.Id);

                // Lấy bảng KpiDetail theo tháng hiện tại
                var kpiDetail = _dbContext.KpiTableDetails.Include(k => k.KpiTable)
                    .FirstOrDefault(k =>
                        k.EmployeeId == employee.Id &&
                        k.KpiTable != null &&
                        k.KpiTable.ToDate.HasValue &&
                        k.KpiTable.ToDate.Value.Month == payroll.CreatedAt.Value.Month &&
                        k.KpiTable.ToDate.Value.Year == payroll.CreatedAt.Value.Year);

                // Lấy các khoản khấu trừ
                var deductions = _dbContext.Deductions.Where(d => d.EmployeeId == employee.Id).ToList();
                var totalDeductions = deductions.Sum(d => d.Value); // Tổng khấu trừ

                // 2. Tính toán các thành phần lương
                var baseSalary = contract?.SalaryAmount ?? 0;
                var actualWorkDays = timesheet
                    .Where(t => t.Date.HasValue && t.Date.Value.Month == payroll.CreatedAt.Value.Month && t.Date.Value.Year == payroll.CreatedAt.Value.Year)
                    .Select(t => t.Date.Value.Date)
                    .Distinct()
                    .Count();

                var receivedSalary = standardWorkDays > 0 ? (baseSalary / (decimal)standardWorkDays) * (decimal)actualWorkDays : 0;
                var kpi = contract?.KpiSalary ?? 0;
                var kpiPercentage = kpiDetail?.CompletionRate ?? 0;
                var kpiSalary = (decimal)kpi * ((decimal)kpiPercentage / 100);
                var bonus = kpiDetail?.Bonus ?? 0;
                var salaryRate = contract?.SalaryRate ?? 1;

                // Lương 1 ngày
                var dailySalary = standardWorkDays > 0 ? baseSalary / (decimal)standardWorkDays : 0;

                // Đọc cấu hình thành phần lương từ SalaryComponents (theo tổ chức của bảng lương hoặc nhân viên)
                var organizationIdForComponent = payroll.OrganizationId ?? employee.OrganizationId;
                var allowanceMealTravel = GetSalaryComponentAmount(organizationIdForComponent, "ALLOWANCE_MEAL_TRAVEL", 1_000_000m);
                var parkingPerDay = GetSalaryComponentAmount(organizationIdForComponent, "PARKING", 3_000m);
                var parkingAmount = parkingPerDay * (decimal)actualWorkDays;

                // Tính giờ OT thô theo ngày: tổng giờ làm trong ngày vượt quá 8h (đã trừ nghỉ trưa trong NumberOfWorkingHour nếu có)
                var employeeTimesheetsInPeriod = timesheet
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Month == payroll.CreatedAt.Value.Month
                                && t.Date.Value.Year == payroll.CreatedAt.Value.Year
                                && t.NumberOfWorkingHour.HasValue)
                    .ToList();

                var rawOtHours = employeeTimesheetsInPeriod
                    .GroupBy(t => t.Date!.Value.Date)
                    .Sum(g =>
                    {
                        var totalHoursInDay = g.Sum(x => (decimal)(x.NumberOfWorkingHour ?? 0));
                        var normalHours = Math.Min(totalHoursInDay, 8m);
                        return totalHoursInDay > normalHours ? totalHoursInDay - normalHours : 0m;
                    });

                // Quy đổi sang giờ thập phân theo bảng làm tròn phút
                var rawOtMinutes = rawOtHours * 60m;
                var wholeHours = Math.Floor(rawOtMinutes / 60m);
                var remainingMinutes = (int)(rawOtMinutes - wholeHours * 60m);
                var decimalPart = MapMinutesToOtDecimal(remainingMinutes);
                var otHoursDecimal = (decimal)wholeHours + decimalPart;

                // Hệ số OT 200%
                var otHoursForPay = otHoursDecimal * 2m;
                var otDays = otHoursForPay / 8m;
                var overtimeAmount = otDays * dailySalary;

                // BHXH: lấy từ SalaryComponents, fallback 1.632.000
                var bhxhAmount = GetSalaryComponentAmount(organizationIdForComponent, "BHXH", 1_632_000m);

                // Lương lõi (lương cứng + KPI + thưởng) áp tỷ lệ hưởng
                var coreSalary = ((decimal)receivedSalary + (decimal)kpiSalary + (decimal)bonus) * ((decimal)salaryRate / 100);

                var totalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount;

                var totalReceivedSalary = totalSalary - (totalDeductions + bhxhAmount);

                // 3. Lưu vào bảng PayrollDetail
                payrollDetails.Add(new PayrollDetail
                {
                    OrganizationId = employee.OrganizationId,
                    PayrollId = payrollId,
                    EmployeeId = employee.Id,
                    ContractId = contract?.Id,
                    EmployeeCode = employee.EmployeeCode,
                    FullName = employee.LastName + " " + employee.FirstName,
                    ContractTypeStatus = contract?.ContractTypeStatus ?? ContractTypeStatus.Official,
                    BaseSalary = baseSalary,
                    StandardWorkDays = standardWorkDays,
                    ActualWorkDays = actualWorkDays,
                    ReceivedSalary = receivedSalary,
                    KPI = (decimal)kpi,
                    KpiPercentage = (decimal)kpiPercentage,
                    KpiSalary = (decimal)kpiSalary,
                    Bonus = (decimal)bonus,

                    SalaryRate = (decimal)salaryRate,
                    AllowanceMealTravel = allowanceMealTravel,
                    ParkingAmount = parkingAmount,
                    OvertimeAmount = overtimeAmount,
                    BhxhAmount = bhxhAmount,
                    TotalSalary = (decimal)totalSalary,
                    TotalReceivedSalary = (decimal)totalReceivedSalary,
                    ConfirmationStatus = PayrollConfirmationStatusEmployee.NotSent
                });

            }

            await CreateRangeAsync(payrollDetails);
        }

        private static decimal MapMinutesToOtDecimal(int minutes)
        {
            if (minutes <= 5) return 0.00m;
            if (minutes <= 10) return 0.10m;
            if (minutes <= 15) return 0.25m;
            if (minutes <= 20) return 0.30m;
            if (minutes <= 30) return 0.50m;
            if (minutes <= 45) return 0.75m;
            return 1.00m; // 46-59
        }

        private decimal GetSalaryComponentAmount(int? organizationId, string componentCode, decimal defaultValue)
        {
            if (!organizationId.HasValue)
            {
                return defaultValue;
            }

            var component = _dbContext.SalaryComponents
                .FirstOrDefault(c =>
                    c.OrganizationId == organizationId.Value &&
                    c.ComponentCode == componentCode &&
                    c.IsDeleted != true &&
                    c.Status == Status.Tracking);

            if (component == null || string.IsNullOrWhiteSpace(component.ValueFormula))
            {
                return defaultValue;
            }

            // Ưu tiên parse theo invariant, fallback sang vi-VN
            if (decimal.TryParse(component.ValueFormula, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            var viCulture = CultureInfo.GetCultureInfo("vi-VN");
            if (decimal.TryParse(component.ValueFormula, NumberStyles.Any, viCulture, out value))
            {
                return value;
            }

            return defaultValue;
        }

        public async Task RecalculateAndSavePayrollDetails(int payrollId)
        {
            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
            }

            // Nếu đã khóa thì không cho cập nhật (tính lại) phiếu lương
            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể cập nhật phiếu lương.");
            }

            // Soft delete các bản ghi cũ để tránh bị trùng dữ liệu
            var existingDetails = await _dbContext.PayrollDetails
                .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                .ToListAsync();

            if (existingDetails.Any())
            {
                foreach (var item in existingDetails)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.Now;
                }
                await UpdateRangeAsync(existingDetails);
            }

            // Tính lại và lưu mới
            await CalculateAndSavePayrollDetails(payrollId);
        }

        public async Task Update(int id, UpdatePayrollDetailRequest request)
        {
            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {id}");
            }

            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollDetail.PayrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollDetail.PayrollId}");
            }

            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể sửa phiếu lương.");
            }

            var shouldRecalcReceivedSalary = false;
            var shouldRecalcKpiSalary = false;
            var shouldRecalcTotalSalary = false;

            if (request.BaseSalary.HasValue)
            {
                payrollDetail.BaseSalary = request.BaseSalary.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.StandardWorkDays.HasValue)
            {
                payrollDetail.StandardWorkDays = request.StandardWorkDays.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.ActualWorkDays.HasValue)
            {
                payrollDetail.ActualWorkDays = request.ActualWorkDays.Value;
                shouldRecalcReceivedSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.KPI.HasValue)
            {
                payrollDetail.KPI = request.KPI.Value;
                shouldRecalcKpiSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.KpiPercentage.HasValue)
            {
                payrollDetail.KpiPercentage = request.KpiPercentage.Value;
                shouldRecalcKpiSalary = true;
                shouldRecalcTotalSalary = true;
            }

            if (request.Bonus.HasValue)
            {
                payrollDetail.Bonus = request.Bonus.Value;
                shouldRecalcTotalSalary = true;
            }

            if (request.SalaryRate.HasValue)
            {
                payrollDetail.SalaryRate = request.SalaryRate.Value;
                shouldRecalcTotalSalary = true;
            }

            if (shouldRecalcReceivedSalary)
            {
                var baseSalary = payrollDetail.BaseSalary ?? 0;
                var standardWorkDays = payrollDetail.StandardWorkDays ?? 0;
                var actualWorkDays = payrollDetail.ActualWorkDays ?? 0;

                payrollDetail.ReceivedSalary = standardWorkDays > 0
                    ? (baseSalary / standardWorkDays) * (decimal)actualWorkDays
                    : 0;
            }

            if (shouldRecalcKpiSalary)
            {
                var kpi = payrollDetail.KPI ?? 0;
                var kpiPercentage = payrollDetail.KpiPercentage ?? 0;
                payrollDetail.KpiSalary = kpi * (kpiPercentage / 100);
            }

            if (shouldRecalcTotalSalary)
            {
                var receivedSalary = payrollDetail.ReceivedSalary ?? 0;
                var kpiSalary = payrollDetail.KpiSalary ?? 0;
                var bonus = payrollDetail.Bonus ?? 0;
                var salaryRate = payrollDetail.SalaryRate ?? 100;

                var allowanceMealTravel = payrollDetail.AllowanceMealTravel ?? 1_000_000m;
                var parkingAmount = payrollDetail.ParkingAmount
                                     ?? 3_000m * (decimal)(payrollDetail.ActualWorkDays ?? 0);
                var overtimeAmount = payrollDetail.OvertimeAmount ?? 0m;
                var bhxhAmount = payrollDetail.BhxhAmount ?? 1_632_000m;

                var coreSalary = (receivedSalary + kpiSalary + bonus) * (salaryRate / 100);
                payrollDetail.TotalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount;

                // TotalReceivedSalary = TotalSalary - (BHXH + tổng khấu trừ khác theo nhân viên)
                var totalDeductions = 0m;
                if (payrollDetail.EmployeeId.HasValue)
                {
                    totalDeductions = _dbContext.Deductions
                        .Where(d => d.EmployeeId == payrollDetail.EmployeeId.Value)
                        .Sum(d => d.Value) ?? 0;
                }
                payrollDetail.TotalReceivedSalary = payrollDetail.TotalSalary - (bhxhAmount + totalDeductions);
            }

            await UpdateAsync(payrollDetail);
        }

        public async Task Delete(int id)
        {
            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Id = {id}");
            }

            var payroll = await _dbContext.Payrolls
                .FirstOrDefaultAsync(p => p.Id == payrollDetail.PayrollId && p.IsDeleted != true);

            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollDetail.PayrollId}");
            }

            if (payroll.PayrollStatus == PayrollStatus.Locked)
            {
                throw new Exception("Bảng lương đã khóa, không thể xóa phiếu lương.");
            }

            payrollDetail.IsDeleted = true;
            await UpdateAsync(payrollDetail);
        }

        public async Task DeleteRange(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new Exception("Danh sách Id không được để trống.");
            }

            var payrollDetails = await _dbContext.PayrollDetails
                .Where(p => ids.Contains(p.Id) && p.IsDeleted != true)
                .ToListAsync();

            if (payrollDetails.Count == 0)
            {
                return;
            }

            // Nếu bất kỳ phiếu lương nào thuộc bảng lương đã khóa thì chặn
            var payrollIds = payrollDetails.Where(x => x.PayrollId.HasValue).Select(x => x.PayrollId!.Value).Distinct().ToList();
            var lockedPayrollExists = await _dbContext.Payrolls.AnyAsync(p => payrollIds.Contains(p.Id) && p.IsDeleted != true && p.PayrollStatus == PayrollStatus.Locked);
            if (lockedPayrollExists)
            {
                throw new Exception("Có bảng lương đã khóa, không thể xóa phiếu lương.");
            }

            foreach (var item in payrollDetails)
            {
                item.IsDeleted = true;
            }
            await UpdateRangeAsync(payrollDetails);
        }

        public async Task<List<PayrollDetailDto>> FetchPayrollDetails(int payrollId)
        {
            try
            {
                var payroll = await _dbContext.Payrolls
                    .FirstOrDefaultAsync(p => p.Id == payrollId && p.IsDeleted != true);

                if (payroll == null)
                {
                    throw new EntityNotFoundException(nameof(Payroll), $"Id = {payrollId}");
                }

                // 1. Kiểm tra nếu bảng lương chi tiết đã tồn tại
                var existingDetails = await _dbContext.PayrollDetails
                    .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                    .ToListAsync();

                if (existingDetails.Any())
                {
                    return _mapper.Map<List<PayrollDetailDto>>(existingDetails);
                }

                // 2. Nếu chưa tồn tại, tính toán và lưu bảng lương chi tiết
                await CalculateAndSavePayrollDetails(payrollId);

                // 3. Lấy lại danh sách bảng lương chi tiết vừa tạo
                var newDetails = await _dbContext.PayrollDetails
                    .Where(pd => pd.PayrollId == payrollId && pd.IsDeleted != true)
                    .ToListAsync();

                return _mapper.Map<List<PayrollDetailDto>>(newDetails);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // Quản lý gửi bảng lương cho nhân viên xem
        public async Task SendPayrollDetailConfirmation(UpdateSendPayrollDetailConfirmationRequest request)
        {
            if (request.PayrollDetailIds == null || !request.PayrollDetailIds.Any())
            {
                throw new Exception($"Chọn ít nhất 1 bảng lương chi tiết để gửi");
            }

            var payrollDetails = await _dbContext.PayrollDetails
                .Where(p => request.PayrollDetailIds.Contains(p.Id))
                .ToListAsync();

            foreach (var payrollDetail in payrollDetails)
            {
                if (payrollDetail == null)
                {
                    throw new Exception($"Không tìm thấy bảng lương chi tiết");
                }

                payrollDetail.ConfirmationStatus = PayrollConfirmationStatusEmployee.Confirming;
                payrollDetail.ResponseDeadline = request.ResponseDeadline;

            }

            await UpdateRangeAsync(payrollDetails);
        }

        // Nhân viên xác nhận bảng lương
        public async Task ConfirmPayrollDetailByEmployee(int payrollDetailId)
        {
            // Kiểm tra nếu ID không hợp lệ
            if (payrollDetailId <= 0)
            {
                throw new ArgumentException("PayrollDetailId không hợp lệ.");
            }

            var payrollDetail = await _dbContext.PayrollDetails
                .FirstOrDefaultAsync(p => p.Id == payrollDetailId && p.IsDeleted != true);

            if (payrollDetail == null)
            {
                throw new EntityNotFoundException(nameof(PayrollDetail), $"Không tìm thấy bảng lương chi tiết với Id = {payrollDetailId}");
            }

            payrollDetail.ConfirmationStatus = PayrollConfirmationStatusEmployee.Confirmed;
            payrollDetail.ConfirmationDate = DateTime.Now;

            await UpdateAsync(payrollDetail);

        }

    }
}
