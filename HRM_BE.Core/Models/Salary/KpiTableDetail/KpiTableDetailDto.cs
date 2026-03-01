using HRM_BE.Core.Models.Salary.KpiTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Salary.KpiTableDetail
{
    public class KpiTableDetailDto
    {
        public int? Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? KpiTableId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public double? CompletionRate { get; set; }
        public double? Bonus { get; set; }
        public decimal? Revenue { get; set; }
        public string? StaffPositionCode { get; set; }
        public bool IsRevenueEditable { get; set; }

        //public virtual KpiTableDto? KpiTable { get; set; }
    }
}
