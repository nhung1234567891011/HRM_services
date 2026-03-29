using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Official_Form.LeaveApplication
{
    public class GetLeaveApplicationRequest:PagingRequest
    {
        public int? OrganizationId { get; set; } // tổ chức công ty đơn vị
        public int? EmployeeId { get; set; }// Nhân viên tạo đơn
        public DateTime? StartDate { get; set; } // Ngày bắt đầu nghỉ
        public DateTime? EndDate { get; set; } // Ngày kết thúc nghỉ
        public double? NumberOfDays { get; set; } // Số ngày nghỉ
        public int? TypeOfLeaveId { get; set; } // Loại nghỉ phép
        public decimal? SalaryPercentage { get; set; } // Phần trăm lương được hưởng trong thời gian nghỉ == TypeOfLeave.SalaryRate
        public string? ReasonForLeave { get; set; } // Lý do xin nghỉ
        public string? Note { get; set; } // Ghi chú thêm cho đơn
        public OnPaidLeaveStatus? OnPaidLeaveStatus { get; set; }  // Có chọn nghỉ trừ số ngày nghỉ không

        public LeaveApplicationStatus? Status { get; set; } 
        public bool? ForApproval { get; set; }

    }
}
