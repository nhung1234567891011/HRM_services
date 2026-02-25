using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Models.DetailTimeSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.SumaryTimeSheet
{
    public class SummaryTimesheetNameEmployeeConfirmTimeSheetDto
    {
        public int? ShiftWorkId { get; set; }// Dữ liệu bảng phân ca nào

        public int? EmployeeId { get; set; } //Id nhân viên

        public TimekeepingType? TimekeepingType { get; set; }// Loại chấm công, bằng GPS hay...

        public DateTime? Date { get; set; }// Ngày chấm công

        public TimeSpan? StartTime { get; set; }//Giờ bắt đầu chấm

        public TimeSpan? EndTime { get; set; }//Giờ ra 

        public double? NumberOfWorkingHour { get; set; }// Số giờ làm bằng bằng EndTime - StartTime

    }





    public class ShiftDetailDto
    {
        public int TimeSheetId { get; set; } // time sheet id 
        public TimeSpan? StartTime { get; set; } // Giờ bắt đầu ca
        public TimeSpan? EndTime { get; set; } // Giờ kết thúc ca
        public int? ShiftWorkId { get; set; }
        public double? NumberOfWorkingHour { get; set; }
        public TimekeepingType? TimekeepingType { get; set; }// Loại chấm công, bằng GPS hay...
        public string? ShiftTableName { get; set; }
        public TimeKeepingLeaveStatus TimeKeepingLeaveStatus { get; set; }
        public bool? IsEnoughWork {  get; set; } =false; // có làm đủ công theo ca không
        public bool IsOvertime { get; set; } // Ca làm thêm giờ (OT)
        public double? OvertimeHours { get; set; } // Số giờ OT trong ca (giờ thô, chưa nhân hệ số)
    }

    public class ConfirmTimeSheetDto
    {
        public DateTime? Date { get; set; } // Ngày chấm công
        public List<ShiftDetailDto> Shifts { get; set; } = new List<ShiftDetailDto>(); // Danh sách các ca trong ngày
    }

    public class ConfirmTimeSheetWithPermittedLeaveDto
    {
        public DateTime? Date { get; set; } // Ngày chấm công
        public List<ShiftDetailDto> Shifts { get; set; } = new List<ShiftDetailDto>(); // Danh sách các ca trong ngày
        public List<PermittedLeaveDto>? PermittedLeaves { get; set; }// ngày nghỉ có công

    }

}
