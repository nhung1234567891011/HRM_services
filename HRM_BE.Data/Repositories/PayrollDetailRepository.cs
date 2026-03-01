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
            var query = _dbContext.PayrollDetails
                .Where(p => p.IsDeleted != true)
                .Include(p => p.Employee)
                .ThenInclude(e => e.Deductions)
                .AsQueryable();

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

        /// <summary>
        /// Tính và lưu PayrollDetail cho từng nhân viên. ActualWorkDays (chỉ giờ tiêu chuẩn, tối đa 8h/ngày) và OvertimeAmount được tính từ timesheet trong tháng.
        /// Khi dữ liệu chấm công thay đổi hoặc logic tính ngày công/OT thay đổi, cần gọi lại phương thức này cho kỳ lương tương ứng để cập nhật.
        /// </summary>
        public async Task CalculateAndSavePayrollDetails(int payrollId)
        {
            // Lấy Payroll
            var payroll = _dbContext.Payrolls.FirstOrDefault(p => p.Id == payrollId);
            if (payroll == null)
            {
                throw new Exception($"Payroll có Id = {payrollId} không tồn tại!");
            }

            var payrollMonth = payroll.CreatedAt ?? DateTime.Now;

            // 1. Lấy danh sách nhân viên thuộc tổ chức của Payroll
            var employees = _dbContext.Employees
                .Where(e => e.OrganizationId == payroll.OrganizationId)
                .Where(e => e.Contracts.Any(c => c.EmployeeId == e.Id))
                .Include(e => e.StaffPosition)
                .ToList();

            var standardWorkDays = _dbContext.ShiftWorks
                .Where(sw => sw.OrganizationId == payroll.OrganizationId)
                .Sum(sw => sw.TotalWork);

            var payrollDetails = new List<PayrollDetail>();

            foreach (var employee in employees)
            {
                var contract = _dbContext.Contracts.FirstOrDefault(c => c.EmployeeId == employee.Id);
                var timesheet = _dbContext.Timesheets.Where(t => t.EmployeeId == employee.Id);

                var employeeTimesheetsInPeriod = timesheet
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Month == payrollMonth.Month
                                && t.Date.Value.Year == payrollMonth.Year
                                && t.NumberOfWorkingHour.HasValue)
                    .ToList();

                // Lấy bảng KpiDetail theo tháng hiện tại
                var kpiDetail = _dbContext.KpiTableDetails.Include(k => k.KpiTable)
                    .FirstOrDefault(k =>
                        k.EmployeeId == employee.Id &&
                        k.KpiTable != null &&
                        k.KpiTable.ToDate.HasValue &&
                        k.KpiTable.ToDate.Value.Month == payrollMonth.Month &&
                        k.KpiTable.ToDate.Value.Year == payrollMonth.Year);

                // Lấy các khoản khấu trừ
                var deductions = _dbContext.Deductions.Where(d => d.EmployeeId == employee.Id).ToList();
                var totalDeductions = deductions.Sum(d => d.Value); // Tổng khấu trừ

                // 2. Tính toán các thành phần lương
                var baseSalary = contract?.SalaryAmount ?? 0;

                // Số ngày đi làm (phục vụ phụ cấp theo ngày như gửi xe)
                var attendanceDays = employeeTimesheetsInPeriod
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .Count();

                // Ngày công thực tế = quy đổi từ giờ làm tiêu chuẩn (tối đa 8h/ngày), KHÔNG bao gồm giờ tăng ca.
                // Giờ tăng ca được tính riêng trong OvertimeAmount (hệ số 200%). Mỗi ngày: chỉ phần <= 8h vào ngày công, phần > 8h vào OT.
                var normalHoursTotal = employeeTimesheetsInPeriod
                    .GroupBy(t => t.Date!.Value.Date)
                    .Sum(g =>
                    {
                        var totalHoursInDay = g.Sum(x => (decimal)(x.NumberOfWorkingHour ?? 0));
                        return Math.Min(totalHoursInDay, 8m);
                    });

                var actualWorkDaysForSalary = normalHoursTotal / 8m;

                var receivedSalary = standardWorkDays > 0 ? (baseSalary / (decimal)standardWorkDays) * actualWorkDaysForSalary : 0;
                var kpi = contract?.KpiSalary ?? 0;
                var kpiPercentage = kpiDetail?.CompletionRate ?? 0;
                var kpiSalary = (decimal)kpi * ((decimal)kpiPercentage / 100);
                var bonus = kpiDetail?.Bonus ?? 0;
                var salaryRate = contract?.SalaryRate ?? 1;

                // Lương 1 ngày
                var dailySalary = standardWorkDays > 0 ? baseSalary / (decimal)standardWorkDays : 0;
                var hourlySalary = dailySalary / 8m;

                // === Các khoản sau đều lấy từ Salary Component (bảng SalaryComponents) theo ComponentCode ===
                // - Thu nhập: ALLOWANCE_MEAL_TRAVEL, PARKING (và Hoa hồng qua chính sách doanh thu)
                // - Khấu trừ: BHXH, BHTN, BHYT, QUY_CONG_DOAN
                // Nếu không tìm thấy component (đúng OrganizationId + Code + Status=Tracking) thì dùng defaultValue.
                var organizationIdForComponent = payroll.OrganizationId ?? employee.OrganizationId;

                var allowanceMealTravel = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "ALLOWANCE_MEAL_TRAVEL",
                    defaultValue: 1_000_000m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                var parkingAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "PARKING",
                    defaultValue: 3_000m * (decimal)attendanceDays,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Giờ tăng ca: cùng totalHoursInDay như trên; phần ≤ 8h đã tính vào ngày công (normalHoursTotal), phần > 8h là OT.
                var rawOtMinutes = employeeTimesheetsInPeriod
                    .GroupBy(t => t.Date!.Value.Date)
                    .Sum(g =>
                    {
                        var totalHoursInDay = g.Sum(x => (decimal)(x.NumberOfWorkingHour ?? 0));
                        var otHoursInDay = Math.Max(totalHoursInDay - 8m, 0m);
                        return otHoursInDay * 60m;
                    });

                // Quy đổi sang giờ thập phân theo bảng làm tròn phút (làm tròn tới phút trước khi map)
                var rawOtMinutesRounded = (int)Math.Round(rawOtMinutes, MidpointRounding.AwayFromZero);
                if (rawOtMinutesRounded < 0) rawOtMinutesRounded = 0;

                var wholeHours = rawOtMinutesRounded / 60;
                var remainingMinutes = rawOtMinutesRounded % 60;
                var decimalPart = MapMinutesToOtDecimal(remainingMinutes);
                var otHoursDecimal = (decimal)wholeHours + decimalPart;

                // Hệ số OT 200%
                var overtimeAmount = otHoursDecimal * 2m * hourlySalary;

                // Hoa hồng doanh thu (cấu hình theo bậc)
                var revenue = kpiDetail?.Revenue ?? 0m;
                var payrollDate = payrollMonth;
                var commissionAmount = ResolveRevenueCommissionAmount(
                    organizationIdForComponent,
                    employee?.StaffPosition?.PositionCode,
                    revenue,
                    payrollDate);

                // BHXH: 100% từ Salary Component (ComponentCode = "BHXH"), thường % lương đóng BH
                var bhxhAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "BHXH",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // BHTN: 100% từ Salary Component (ComponentCode = "BHTN")
                var bhtnAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "BHTN",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // BHYT: 100% từ Salary Component (ComponentCode = "BHYT")
                var bhytAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "BHYT",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Gộp BHXH + BHTN + BHYT vào một cột BhxhAmount (tránh thêm cột DB)
                var totalInsuranceDeduction = bhxhAmount + bhtnAmount + bhytAmount;

                // Quỹ công đoàn: 100% từ Salary Component (ComponentCode = "QUY_CONG_DOAN")
                var unionFeeAmount = ResolveSalaryComponentWithRule(
                    organizationIdForComponent,
                    "QUY_CONG_DOAN",
                    defaultValue: 0m,
                    contractSalaryAmount: baseSalary,
                    contractSalaryInsurance: contract?.SalaryInsurance ?? baseSalary,
                    receivedSalary: receivedSalary,
                    attendanceDays: attendanceDays);

                // Công thức: Lương lõi + (Phụ cấp + Gửi xe + Hoa hồng + OT từ cấu hình) - (BHXH + BHTN + BHYT + Quỹ công đoàn từ Salary Component) - Khấu trừ khác
                var coreSalary = ((decimal)receivedSalary + (decimal)kpiSalary + (decimal)bonus) * ((decimal)salaryRate / 100);

                // Tổng lương (trước trừ BHXH, Quỹ công đoàn): Lương cứng + Phụ cấp đi lại ăn trưa + Tiền gửi xe + Hoa hồng + Lương tăng ca
                var totalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount + commissionAmount;

                // Tổng thực nhận = Tổng lương - (BHXH+BHTN+BHYT) - Quỹ công đoàn - Khấu trừ khác
                var totalReceivedSalary = totalSalary - (totalDeductions + totalInsuranceDeduction + unionFeeAmount);

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
                    ActualWorkDays = (double)actualWorkDaysForSalary,
                    ReceivedSalary = receivedSalary,
                    KPI = (decimal)kpi,
                    KpiPercentage = (decimal)kpiPercentage,
                    KpiSalary = (decimal)kpiSalary,
                    Bonus = (decimal)bonus,

                    SalaryRate = (decimal)salaryRate,
                    AllowanceMealTravel = allowanceMealTravel,
                    ParkingAmount = parkingAmount,
                    OvertimeAmount = overtimeAmount,
                    CommissionAmount = commissionAmount,
                    BhxhAmount = totalInsuranceDeduction,
                    UnionFeeAmount = unionFeeAmount,
                    TotalSalary = (decimal)totalSalary,
                    TotalReceivedSalary = (decimal)totalReceivedSalary,
                    ConfirmationStatus = PayrollConfirmationStatusEmployee.NotSent
                });

            }

            await CreateRangeAsync(payrollDetails);
        }

        private decimal ResolveSalaryComponentWithRule(
            int? organizationId,
            string componentCode,
            decimal defaultValue,
            decimal contractSalaryAmount,
            decimal contractSalaryInsurance,
            decimal receivedSalary,
            int attendanceDays)
        {
            if (!organizationId.HasValue) return defaultValue;

            var component = _dbContext.SalaryComponents
                .FirstOrDefault(c =>
                    c.OrganizationId == organizationId.Value &&
                    c.ComponentCode == componentCode &&
                    c.IsDeleted != true &&
                    c.Status == Status.Tracking);

            if (component == null) return defaultValue;

            switch (component.CalcType)
            {
                case SalaryComponentCalcType.FixedAmount:
                {
                    if (component.FixedAmount.HasValue) return component.FixedAmount.Value;
                    return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
                }
                case SalaryComponentCalcType.PerAttendanceDay:
                {
                    var unit = component.UnitAmount ?? TryParseLegacyValueFormula(component.ValueFormula, 0m);
                    if (unit <= 0m) return defaultValue;
                    return unit * attendanceDays;
                }
                case SalaryComponentCalcType.PercentOfBase:
                {
                    var ratePercent = component.RatePercent ?? TryParseLegacyValueFormula(component.ValueFormula, 0m);
                    if (ratePercent <= 0m) return defaultValue;

                    var baseAmount = component.BaseSource switch
                    {
                        SalaryComponentBaseSource.ContractSalaryInsurance => contractSalaryInsurance,
                        SalaryComponentBaseSource.ReceivedSalary => receivedSalary,
                        _ => contractSalaryAmount
                    };

                    if (component.CapAmount.HasValue && component.CapAmount.Value > 0m)
                    {
                        baseAmount = Math.Min(baseAmount, component.CapAmount.Value);
                    }

                    return baseAmount * (ratePercent / 100m);
                }
                default:
                    return defaultValue;
            }
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

            if (component == null)
            {
                return defaultValue;
            }

            // Backward-compatible: ưu tiên field mới, fallback về ValueFormula
            return ResolveComponentValue(component, defaultValue);
        }

        private decimal ResolveComponentValue(SalaryComponent component, decimal defaultValue)
        {
            // 1) Field mới (FixedAmount/UnitAmount/RatePercent) nếu có
            if (component.CalcType == SalaryComponentCalcType.FixedAmount)
            {
                if (component.FixedAmount.HasValue) return component.FixedAmount.Value;
                return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
            }

            // Các CalcType còn lại cần context -> nếu gọi nhầm thì fallback về default/legacy
            return TryParseLegacyValueFormula(component.ValueFormula, defaultValue);
        }

        private static decimal TryParseLegacyValueFormula(string? valueFormula, decimal defaultValue)
        {
            if (string.IsNullOrWhiteSpace(valueFormula)) return defaultValue;

            // Ưu tiên parse theo invariant, fallback sang vi-VN
            if (decimal.TryParse(valueFormula, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            var viCulture = CultureInfo.GetCultureInfo("vi-VN");
            if (decimal.TryParse(valueFormula, NumberStyles.Any, viCulture, out value))
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

                var allowanceMealTravel = payrollDetail.AllowanceMealTravel ?? 0m;
                var parkingAmount = payrollDetail.ParkingAmount ?? 0m;
                var overtimeAmount = payrollDetail.OvertimeAmount ?? 0m;
                var commissionAmount = payrollDetail.CommissionAmount ?? 0m;
                var totalInsuranceDeduction = payrollDetail.BhxhAmount ?? 0m;
                var unionFeeAmount = payrollDetail.UnionFeeAmount ?? 0m;

                // Công thức: Lương cứng + Phụ cấp + Gửi xe + Hoa hồng + OT - (BHXH+BHTN+BHYT) - Quỹ công đoàn
                var coreSalary = (receivedSalary + kpiSalary + bonus) * (salaryRate / 100);
                payrollDetail.TotalSalary = coreSalary + allowanceMealTravel + parkingAmount + overtimeAmount + commissionAmount;

                var totalDeductions = 0m;
                if (payrollDetail.EmployeeId.HasValue)
                {
                    totalDeductions = _dbContext.Deductions
                        .Where(d => d.EmployeeId == payrollDetail.EmployeeId.Value)
                        .Sum(d => d.Value) ?? 0;
                }
                payrollDetail.TotalReceivedSalary = payrollDetail.TotalSalary - (totalInsuranceDeduction + unionFeeAmount + totalDeductions);
            }

            await UpdateAsync(payrollDetail);
        }

        private decimal ResolveRevenueCommissionAmount(
            int? organizationId,
            string? staffPositionCode,
            decimal revenue,
            DateTime payrollDate)
        {
            if (!organizationId.HasValue) return 0m;
            if (revenue <= 0m) return 0m;
            if (string.IsNullOrWhiteSpace(staffPositionCode)) return 0m;

            var code = staffPositionCode.Trim().ToUpperInvariant();
            RevenueCommissionTargetType? targetType = null;

            if (code == "CTV")
            {
                targetType = RevenueCommissionTargetType.Ctv;
            }
            else if (code.StartsWith("SALE"))
            {
                targetType = RevenueCommissionTargetType.Sale;
            }

            if (!targetType.HasValue) return 0m;

            var policy = _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true
                            && p.Status == Status.Tracking
                            && p.OrganizationId == organizationId.Value
                            && p.TargetType == targetType.Value
                            && (!p.EffectiveFrom.HasValue || p.EffectiveFrom.Value.Date <= payrollDate.Date)
                            && (!p.EffectiveTo.HasValue || p.EffectiveTo.Value.Date >= payrollDate.Date))
                .Include(p => p.Tiers)
                .OrderByDescending(p => p.EffectiveFrom ?? DateTime.MinValue)
                .ThenByDescending(p => p.Id)
                .FirstOrDefault();

            if (policy == null) return 0m;
            if (policy.Tiers == null || policy.Tiers.Count == 0) return 0m;

            return CalculateProgressiveCommission(revenue, policy.Tiers);
        }

        private static decimal CalculateProgressiveCommission(decimal revenue, IEnumerable<RevenueCommissionTier> tiers)
        {
            if (revenue <= 0m) return 0m;

            var ordered = tiers
                .Where(t => t.IsDeleted != true)
                .OrderBy(t => t.FromAmount)
                .ThenBy(t => t.ToAmount.HasValue ? 0 : 1)
                .ThenBy(t => t.SortOrder)
                .ToList();

            decimal total = 0m;

            foreach (var tier in ordered)
            {
                var from = tier.FromAmount;
                var to = tier.ToAmount;

                if (revenue <= from) continue;

                var upper = to.HasValue && to.Value > from ? to.Value : revenue;
                var applicable = Math.Min(revenue, upper) - from;
                if (applicable <= 0m) continue;

                var rate = tier.RatePercent;
                if (rate <= 0m) continue;

                total += applicable * (rate / 100m);
            }

            return total;
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
                throw new Exception(ex.Message, ex);
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
