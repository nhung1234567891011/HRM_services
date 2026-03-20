using HRM_BE.Core.Data.Content;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.IRepositories
{
    public interface ILeaveApplicationRepository: IRepositoryBase<LeaveApplication, int>
    {
        Task<PagingResult<LeaveApplicationDto>> GetPaging(int? organizationId, int? employeeId, DateTime? startDate, DateTime? endDate, double? numberOfDays,int? typeOfLeaveId,string? reasonForLeave,string? note, LeaveApplicationStatus? status, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10, int currentEmployeeId = 0, bool isAdmin = false);

        Task<LeaveApplicationDto> GetById(int id);

        Task<bool> UpdateStatus(int id, LeaveApplicationStatus status, string? approverNote);

        Task<bool> UpdateStatusMultiple(List<int> ids, LeaveApplicationStatus status, string? approverNote);

        Task Update(int id, UpdateLeaveApplicationRequest request);

        Task<TotalNumberOfDaysOffDto> GetTotalNumberOfDaysOff(DateTime? startDate, DateTime? endDate, int employeeId);

        Task<bool> DeleteSoft(int id);

        Task<double> CountScheduledDayOffs(DateTime adjustedStartDate, DateTime adjustedEndDate, int employeeId);
        Task<double> GetTotalLeaveEmployee(DateTime startDate, DateTime endDate, int employeeId);



    }
}
