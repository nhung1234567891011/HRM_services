using HRM_BE.Core.Data.Official_Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Official_Form.LeaveApplication
{
    public class UpdateLeaveApplicationRequest
    {
        public int? EmployeeId { get; set; }
        public int? OrganizationId { get; set; } // tổ chức công ty đơn vị
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? NumberOfDays { get; set; }
        public int? TypeOfLeaveId { get; set; }
        public decimal? SalaryPercentage { get; set; }
        public string? ReasonForLeave { get; set; }
        public string? Note { get; set; }
        public string? ApproverNote { get; set; } // Ghi chú của người duyệt cho đơn
        public OnPaidLeaveStatus? OnPaidLeaveStatus { get; set; } = HRM_BE.Core.Data.Official_Form.OnPaidLeaveStatus.No;

        public LeaveApplicationStatus? Status { get; set; } = LeaveApplicationStatus.Pending;

        public List<int> ApproverIds { get; set; } = new(); // Danh sách ID người duyệt đơn
        public List<int> ReplacementIds { get; set; } = new(); // Danh sách ID người thay thế
        public List<int> RelatedPersonIds { get; set; } = new(); // Danh sách ID người liên quan

    }
}
