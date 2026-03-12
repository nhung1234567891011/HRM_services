using HRM_BE.Core.Data;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Staff;

namespace HRM_BE.Core.Data.Official_Form
{
    public class CheckInCheckOutApplication : EntityBase<int>
    {
        public int EmployeeId { get; set; }
        public int ApproverId { get; set; }
        public DateTime Date { get; set; }
        public int CheckType { get; set; }
        public int CheckInCheckOutStatus { get; set; }
        public int ShiftCatalogId { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public TimeSpan? TimeCheckIn { get; set; }
        public TimeSpan? TimeCheckOut { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Employee Approver { get; set; }
        public virtual ShiftCatalog ShiftCatalog { get; set; }
    }
}

