using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Models.DetailTimeSheet;
using HRM_BE.Core.Models.Organization;
using HRM_BE.Core.Models.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.SumaryTimeSheet
{
    public class GetSummaryTimeSheetWithEmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public GetOrganizationForEmployeeDto Organization { get; set; }
        public GetDetailTimeSheetStaffPositionDto StaffPosition { get; set; }
        public double TotalWorkingDay { get; set; }
        public double TotalLeaveDay { get; set; }
        public double DatePerMonth { get; set; }
        public double TotalHour { get; set; }
        public double TotalOtHour { get; set; }
        public double EqualDay { get; set; }
        public double TotalExistLeaveDay { get; set; }
        public SummaryTimesheetNameEmployeeConfirmStatus Status { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
