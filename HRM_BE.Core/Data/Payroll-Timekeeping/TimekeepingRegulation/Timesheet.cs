using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation
{
    //Bảng chấm công
    public class Timesheet:EntityBase<int>
    {   
        public int? ShiftWorkId { get; set; }// Dữ liệu bảng phân ca nào

        public int? EmployeeId { get; set; } //Id nhân viên

        //public int? TimekeepingTypeId { get; set; }// Loại chấm công, bằng GPS hay...

        public TimekeepingType? TimekeepingType { get; set; }// Loại chấm công, bằng GPS hay...

        public DateTime? Date { get; set; }// Ngày chấm công

        public TimeSpan? StartTime { get; set; }//Giờ bắt đầu chấm.

        public TimeSpan? EndTime { get; set; }//Giờ ra 

        public double? NumberOfWorkingHour { get; set; }// Số giờ làm bằng bằng EndTime - StartTime

        public double? OvertimeHour { get; set; } = 0; // Số giờ tăng ca (phần vượt quá tiêu chuẩn ca)

        public double? LateDuration { get; set; } = 0; // Đi muộn (phút)

        public double? EarlyLeaveDuration { get; set; } = 0; // Về sớm (phút)

        public TimeKeepingLeaveStatus TimeKeepingLeaveStatus { get; set; } = TimeKeepingLeaveStatus.None;
        public virtual ShiftWork? ShiftWork { get; set; } 

        public virtual Employee? Employee { get; set; }

        //public virtual TimekeepingType? TimekeepingType { get; set; }

    }

    public enum TimekeepingGPSType
    {
        CheckIn,
        CheckOut
    }
    public enum TimeKeepingLeaveStatus
    {
        None,// Đi làm bình thường
        LeavePermission, // nghỉ có phép
        LeaveNotPermission, // nghỉ không phép
        LeaveNotPermissionWithSalary // nghỉ không có phép nhưng có lương 
    }
}
