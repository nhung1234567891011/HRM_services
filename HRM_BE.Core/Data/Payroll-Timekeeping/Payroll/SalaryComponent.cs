using HRM_BE.Core.Data.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Data.Payroll_Timekeeping.Payroll
{
    // Bảng thành phần lương
    public class SalaryComponent : EntityBase<int>
    {
        public int? OrganizationId { get; set; } // Tổ chức công ty
        public string? ComponentName { get; set; } // Tên thành phần
        public string? ComponentCode { get; set; } // Mã thành phần
        public Nature Nature { get; set; } // Tính chất
        public Characteristic Characteristic { get; set; } // Thuộc tính
        public string? ValueFormula { get; set; } // Legacy: số tiền cố định (string). Dần thay bằng cấu hình CalcType bên dưới.

        // Rule-based configuration (Hướng A)
        public SalaryComponentCalcType CalcType { get; set; } = SalaryComponentCalcType.FixedAmount;
        public SalaryComponentBaseSource? BaseSource { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? UnitAmount { get; set; } // Ví dụ: theo ngày công đi làm (gửi xe), theo ngày công chuẩn...
        public decimal? RatePercent { get; set; } // Ví dụ: 8 nghĩa là 8%
        public decimal? CapAmount { get; set; } // Trần (nếu có)
        public string? Description { get; set; } // Mô tả

        // Trạng thái (Đang theo dõi hoặc ngừng theo dõi)
        public Status Status { get; set; } = Status.Tracking;

        public virtual Organization Organization { get; set; }
    }

    public enum SalaryComponentCalcType
    {
        FixedAmount = 0,          // Số tiền cố định
        PerAttendanceDay = 1,     // Theo số ngày đi làm (UnitAmount * AttendanceDays)
        PercentOfBase = 2         // Tính theo % của BaseSource (có thể áp trần CapAmount)
    }

    public enum SalaryComponentBaseSource
    {
        ContractSalaryAmount = 0,     // Contract.SalaryAmount
        ContractSalaryInsurance = 1,  // Contract.SalaryInsurance (lương đóng BH)
        ReceivedSalary = 2            // PayrollDetail.ReceivedSalary (lương theo ngày công)
    }

    public enum Nature
    {
        Earning, // Thu nhập
        Deduction, // Khấu trừ
        Other // Khác
    }

    public enum Characteristic
    {
        Fixed, // Cố định
        Variable // Biến đổi
    }

    public enum Status
    {
        Tracking, // Đang theo dõi
        Untracking // Ngừng theo dõi
    }
}
