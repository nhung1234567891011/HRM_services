using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Core.Models.ShiftCatalog;

namespace HRM_BE.Core.Models.Official_Form.CheckInCheckOut
{
    public class CheckInCheckOutApplicationDto
    {
        public int Id { get; set; }
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
        public int? CreatedBy { get; set; }
        public string? CreatedName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedName { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public LeaveApplicationEmployeeDto Employee { get; set; }
        public LeaveApplicationEmployeeDto Approver { get; set; }
        public ShiftCatalogDto ShiftCatalog { get; set; }
    }

    public class GetCheckInCheckOutApplicationRequest : PagingRequest
    {
        public int? OrganizationId { get; set; }
        public int? EmployeeId { get; set; }
        public bool? ForApproval { get; set; }
        public string? KeyWord { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CheckInCheckOutStatus { get; set; }
    }

    public class CreateCheckInCheckOutApplicationRequest
    {
        public int EmployeeId { get; set; }
        public int ApproverId { get; set; }
        public DateTime Date { get; set; }
        public int CheckType { get; set; }
        public int ShiftCatalogId { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public string? TimeCheckIn { get; set; }
        public string? TimeCheckOut { get; set; }
    }

    public class UpdateCheckInCheckOutApplicationRequest
    {
        public int ApproverId { get; set; }
        public DateTime Date { get; set; }
        public int CheckType { get; set; }
        public int ShiftCatalogId { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public string? TimeCheckIn { get; set; }
        public string? TimeCheckOut { get; set; }
        public int? CheckInCheckOutStatus { get; set; }
    }
}

