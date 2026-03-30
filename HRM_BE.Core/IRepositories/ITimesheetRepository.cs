using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Models.ShiftCatalog;

namespace HRM_BE.Core.IRepositories
{
    public interface ITimesheetRepository : IRepositoryBase<Timesheet, int>
    {
        Task AddOrUpdateTimesheetAsync(Timesheet timesheet);
        Task<TimesheetDto> GetById(int id);
        Task Update(int id, UpdateTimesheetRequest request);
        Task<PagingResult<TimesheetDto>> Paging(int? employeeId, DateTime? startDate, DateTime? endDate, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10);
        Task<Timesheet?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
        Task<ShiftCatalogDto?> GetShiftCatalogByShiftWorkId(int shiftWorkId);

        Task<TimesheetDurationLateOrEarlyDto> GetTimesheetDurationLateOrEarly(DateTime? startDate, DateTime? endDate, int employeeId);

        Task<Timesheet?> GetByEmployeeAndShiftAsync(int employeeId, int shiftWorkId, DateTime date);

        Task<int> CreateTimesheet(CreateTimesheetRequest request);

        Task<int> RecalculateAllTimesheets(DateTime? startDate = null, DateTime? endDate = null);
    }
}
