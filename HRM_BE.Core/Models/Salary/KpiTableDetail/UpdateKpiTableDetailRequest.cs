using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.Models.Salary.KpiTableDetail
{
    public class UpdateKpiTableDetailRequest
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public double? CompletionRate { get; set; }
        public double? Bonus { get; set; }
    }
}
