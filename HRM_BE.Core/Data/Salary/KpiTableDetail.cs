using HRM_BE.Core.Data.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Data.Salary
{
    public class KpiTableDetail:EntityBase<int>
    {
        public int? KpiTableId { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }   
        public string? EmployeeName { get; set; }
        public double? CompletionRate { get; set; }
        public double? Bonus { get; set; }
        public decimal? Revenue { get; set; }

        public virtual KpiTable? KpiTable { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
