using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation
{
    public class UpdateTimesheetRequest
    {

        public DateTime? Date { get; set; }// Ngày chấm công

        public TimeSpan? StartTime { get; set; }//Giờ bắt đầu chấm.

        public TimeSpan? EndTime { get; set; }//Giờ ra 

        public double? LateDuration { get; set; } = 0; // Đi muộn (phút)

        public double? EarlyLeaveDuration { get; set; } = 0; // Về sớm (phút)

    }
}
