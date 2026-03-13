using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Models.ShiftCatalog;
using HRM_BE.Core.Models.ShiftWork;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Core.IRepositories
{
    public interface ITimesheetRepository : IRepositoryBase<Timesheet, int>
    {
        Task AddOrUpdateTimesheetAsync(Timesheet timesheet);
        Task<TimesheetDto> GetById(int id);
        Task Update(int id, UpdateTimesheetRequest request);
        Task<PagingResult<TimesheetDto>> Paging(int? employeeId, DateTime? startDate, DateTime? endDate, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10);
        Task<Timesheet?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
        Task<ShiftCatalogDto> GetShiftCatalogByShiftWorkId(int shiftWorkId);

        Task<TimesheetDurationLateOrEarlyDto> GetTimesheetDurationLateOrEarly(DateTime? startDate, DateTime? endDate, int employeeId);

        // Lấy thông tin chấm công theo nhân viên và ca làm việc
        Task<Timesheet?> GetByEmployeeAndShiftAsync(int employeeId, int shiftWorkId, DateTime date);

        // Tạo mới bản ghi chấm công (thêm ngày không đi làm hoặc ngày chưa có dữ liệu)
        Task<int> CreateTimesheet(CreateTimesheetRequest request);
    }
}
