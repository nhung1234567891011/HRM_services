using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Data.Staff;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM_BE.Core.Data.Payroll_Timekeeping.Payroll
{
    // Bảng lương
    public class Payroll : EntityBase<int>
    {
        public int? OrganizationId { get; set; } // Tổ chức công ty
        public string? PayrollName { get; set; } // Tên bảng lương
        public PayrollStatus PayrollStatus { get; set; } = PayrollStatus.Unlocked; // Trạng thái
        public PayrollConfirmationStatus PayrollConfirmationStatus { get; set; } = PayrollConfirmationStatus.NotSent; // Trạng thái xác nhận
        public virtual Organization Organization { get; set; }

        public virtual ICollection<PayrollSummaryTimesheet> PayrollSummaryTimesheets { get; set; }     
        public virtual ICollection<PayrollStaffPosition> PayrollStaffPositions { get; set; }
        public virtual ICollection<PayrollDetail> PayrollDetails { get; set; }
    }

    public class PayrollSummaryTimesheet
    {
        public int? PayrollId { get; set; }
        public int? SummaryTimesheetNameId { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual SummaryTimesheetName SummaryTimesheetName { get; set; }
    }

    public class PayrollStaffPosition
    {
        public int? PayrollId { get; set; }
        public int? StaffPositionId { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual StaffPosition StaffPosition { get; set; }
    }

    public enum PayrollStatus
    {
        Unlocked, // Chưa khóa
        Locked, // Đã khóa
    }

    public enum PayrollConfirmationStatus
    {
        NotSent,          // Chưa gửi xác nhận
        NotConfirmed,     // Chưa xác nhận
        Confirming,       // Đang xác nhận
        Confirmed         // Đã xác nhận
    }

    // Bảng lương chi tiết
    public class PayrollDetail : EntityBase<int>
    {
        public int? OrganizationId { get; set; } // Tổ chức công ty
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
        public decimal? HolidayWorkAmount { get; set; } // Lương phụ trội làm việc ngày nghỉ lễ
        public decimal? CommissionAmount { get; set; } // Hoa hồng doanh thu
        public decimal? BhxhAmount { get; set; } // BHXH (+ BHTN + BHYT gộp chung) phải trừ
        public decimal? UnionFeeAmount { get; set; } // Quỹ công đoàn

        public decimal? TotalSalary { get; set; } // Tổng lương
        public decimal? TotalReceivedSalary { get; set; } // Tổng lương thực nhận
        public PayrollConfirmationStatusEmployee ConfirmationStatus { get; set; } = PayrollConfirmationStatusEmployee.NotSent; // Trạng thái xác nhận lương của nhân viên
        public DateTime? ResponseDeadline { get; set; } // Thời hạn phản hồi của nhân viên
        public DateTime? ConfirmationDate { get; set; } // Thời gian nhân viên xác nhận bảng lương
        public virtual HRM_BE.Core.Data.Profile.Contract Contract { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual Organization Organization { get; set; }
    }

    // Trạng thái xác nhận lương của nhân viên
    public enum PayrollConfirmationStatusEmployee
    {
        NotSent,          // Chưa gửi xác nhận công
        Confirming,       // Đang xác nhận
        Rejected,         // Từ chối xác nhận
        Confirmed         // Đã xác nhận
    }

    // Bảng thắc mắc về bảng lương
    public class PayrollInquiry : EntityBase<int>
    {
        public int? PayrollDetailId { get; set; } // ID của bảng lương chi tiết
        public string? Content { get; set; } // Nội dung thắc mắc
        public InquiryStatus Status { get; set; } = InquiryStatus.Pending; // Trạng thái thắc mắc

        public virtual PayrollDetail PayrollDetail { get; set; }
    }

    public enum InquiryStatus
    {
        Pending,   // Đang chờ xử lý
        Resolved,  // Đã xử lý
        Rejected   // Đã từ chối
    }
}
