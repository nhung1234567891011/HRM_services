using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Models.ProfileInfoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class PayrollDetailDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; } // Tổ chức công ty
        public string? OrganizationName { get; set; }
        public int? PayrollId { get; set; }  // Bảng lương
        public int? EmployeeId { get; set; } // Nhân viên
        public int? ContractId { get; set; } // Thông tin hợp đồng

        public string? EmployeeCode { get; set; } // Mã nhân viên
        public string? FullName { get; set; } // Họ tên
        public string? Department { get; set; } // Bộ phận
        public ContractTypeStatus ContractTypeStatus { get; set; } // Loại hợp đồng
        public decimal? BaseSalary { get; set; } // Lương cứng
        public int? StandardWorkDays { get; set; } // Ngày công chuẩn
        public double? ActualWorkDays { get; set; } // Ngày công thực tế
        public decimal? ReceivedSalary { get; set; } // Lương thực nhận
        public decimal? KPI { get; set; } // KPI
        public decimal? KpiPercentage { get; set; } // % KPI đạt
        public decimal? KpiSalary { get; set; } // Lương KPI
        public decimal? Bonus { get; set; } // Thưởng
        public decimal? SalaryRate { get; set; } // Tỉ lệ hưởng lương
        
        public decimal? AllowanceMealTravel { get; set; } // Phụ cấp đi lại, ăn trưa
        public decimal? ParkingAmount { get; set; } // Tiền gửi xe công ty
        public decimal? OvertimeAmount { get; set; } // Lương tăng ca
        public decimal? CommissionAmount { get; set; } // Hoa hồng doanh thu
        public decimal? BhxhAmount { get; set; } // BHXH (+ BHTN + BHYT gộp) phải trừ

        public decimal? TotalSalary { get; set; } // Tổng lương
        public decimal? TotalReceivedSalary { get; set; } // Tổng lương thực nhận
        public PayrollConfirmationStatusEmployee ConfirmationStatus { get; set; } // Trạng thái xác nhận lương của nhân viên
        public DateTime? ResponseDeadline { get; set; } // Thời hạn phản hồi của nhân viên
        public DateTime? ConfirmationDate { get; set; } // Thời gian nhân viên xác nhận bảng lương

        public List<DeductionDto>? Deductions { get; set; } // Danh sách các khoản khấu trừ

        /// <summary>BHXH (alias cho FE - bằng BhxhAmount).</summary>
        public decimal? SocialInsurance { get; set; }
        /// <summary>Phụ cấp đi lại ăn trưa + Tiền gửi xe (cho FE).</summary>
        public decimal? TotalAllowance { get; set; }
        /// <summary>Tổng khấu trừ theo nhân viên (cho FE).</summary>
        public decimal? TotalDeduction { get; set; }
        /// <summary>Quỹ công đoàn (mặc định 0 nếu chưa dùng).</summary>
        public decimal? UnionFee { get; set; }
    }
}
