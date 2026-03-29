using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Models.DetailTimeSheet;
using HRM_BE.Core.Models.ShiftWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.SumaryTimeSheet
{
    public class SummaryTimeSheetDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; } // Tên đơn vị, ví dụ: "CÔNG TY CÔNG NGHỆ VÀ TRUYỀN THÔNG..."
        public string? TimekeepingSheetName { get; set; } // Tên bảng chấm công, ví dụ: "Bảng chấm công tháng 10".
        public bool IsTransferredToPayroll { get; set; } // Đã được chuyển sang tính lương hay chưa.

        public TimekeepingMethod? TimekeepingMethod { get; set; } // Hình thức chấm công, ví dụ: "Theo giờ".

        public virtual List<GetSummaryTimesheetNameStaffPositionDto> SummaryTimesheetNameStaffPositions { get; set; }
        //public virtual List<SummaryTimesheetNameStaffPosition> SummaryTimesheetNameStaffPositions { get; set; }
        public SummaryTimesheetNameEmployeeConfirmStatus Status { get; set; } // Trạng thái xác nhận của nhân viên

        public virtual List<GetSummaryTimeSheetDetailTimeSheetDto> SummaryTimesheetNameDetailTimesheetNames { get; set; }
        public virtual GetOrganizationShiftWorkDto Organization { get; set; }
    }

}
