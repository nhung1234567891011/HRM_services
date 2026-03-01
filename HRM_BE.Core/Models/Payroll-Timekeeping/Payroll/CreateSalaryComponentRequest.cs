using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.Payroll
{
    public class CreateSalaryComponentRequest
    {
        public int? OrganizationId { get; set; } // Tổ chức công ty
        public string? ComponentName { get; set; } // Tên thành phần
        public string? ComponentCode { get; set; } // Mã thành phần
        public Nature Nature { get; set; } // Tính chất
        public Characteristic Characteristic { get; set; } // Thuộc tính
        public string? ValueFormula { get; set; } // Công thức giá trị

        // Rule-based configuration (Hướng A)
        public SalaryComponentCalcType CalcType { get; set; } = SalaryComponentCalcType.FixedAmount;
        public SalaryComponentBaseSource? BaseSource { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? UnitAmount { get; set; }
        public decimal? RatePercent { get; set; }
        public decimal? CapAmount { get; set; }
        public string? Description { get; set; } // Mô tả

        // Trạng thái (Đang theo dõi hoặc ngừng theo dõi)
        public Status Status { get; set; }
    }
}
