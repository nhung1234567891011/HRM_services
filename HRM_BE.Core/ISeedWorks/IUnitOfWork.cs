
using HRM_BE.Core.IRepositories;
using HRM_BE.Data.Repositories;

namespace HRM_BE.Core.ISeedWorks
{
    public interface IUnitOfWork
    {
        //Salary
        public ILeavePermissionRepository LeavePermissions { get; }
        public IKpiTableRepository KpiTables { get; }
        public IKpiTableDetailRepository KpiTableDetails { get; }
        public ISalaryComponentRepository SalaryComponents { get; }
        public IPayrollRepository Payrolls{ get; }
        public IPayrollDetailRepository PayrollDetails { get; }
        public IPayrollInquiryRepository PayrollInquiries { get; }
        public IRevenueCommissionPolicyRepository RevenueCommissionPolicies { get; }

        public IPrefixConfigRepository PrefixConfigs { get; }
        public IBannerRepository Banners { get; }
        public ICompanyRepository Companies { get; }
        public IOrganizationRepository Organizations { get; }
        public IOrganizationTypeRepository OrganizationTypes { get; }
        public IGroupPositionRepository GroupPositions { get; }
        public IStaffTitleRepository StaffTitles { get; }
        public IStaffPositionRepository StaffPositions { get; }
        public IEmployeeRepository Employees { get; }
        public IContractDurationRepository ContractDurations { get; }
        public IContractTypeRepository ContractTypes { get; }
        public INatureOfLaborRepository NatureOfLabors { get; }
        public IWorkingFormRepository WorkingForms { get; }
        public IContractRepository Contracts { get; }
        public IProfileRepository Profiles { get; }
        public IContactInfoRepository ContactInfos { get; }
        public IAllowanceRepository Allowances { get; }
        public IProfileInfoRepository  ProfileInfos { get; }
        public IDeductionRepository Deductions { get; }
        public IJobInfoRepository JobInfos { get; }
        public IAddressRepository Address { get; }
        public IGeneralLeaveRegulationRepository GeneralLeaveRegulation { get; }
        public ITypeOfLeaveRepository TypeOfLeave { get; }
        public ITypeOfLeaveEmployeeRepository TypeOfLeaveEmployee { get; }
        public IHolidayRepository Holiday { get; }
        public IWorkFactorRepository WorkFactor { get; }
        public ITimekeepingRegulationRepository TimekeepingRegulation { get; }
        public ITimekeepingLocationRepository TimekeepingLocation { get; }
        public ITimesheetRepository Timesheet { get; }
        public ITimekeepingSettingRepository TimekeepingSetting { get; }
        public ILeaveApplicationRepository LeaveApplications { get; }
        public IShiftCatalogRepository ShiftCatalogs { get; }
        public IShiftWorkRepository ShiftWorks { get; }
        public IDetailTimeSheetRepository DetailTimeSheets { get; }
        public IApplyOrganizationRepository ApplyOrganization { get; }
        public ISummaryTimeSheetRepository SummaryTimeSheets { get; }

        public ISummaryTimesheetNameEmployeeConfirmRepository SummaryTimesheetNameEmployeeConfirms { get; }
        public IGroupWorkRepository GroupWorks { get; }

        public IDepartmentRepository Departments { get; }
        public IProjectRepository Projects { get; }
        public IDelegationRepository Delegations { get; }
        public ICommentRepository Comments{ get; }
        public IWorkRepository Works { get; }
        public ITagRepository Tags { get; }
        public IUserConnectionRepository UserConnections { get; }
        public IRemindWorkNotificationRepository RemindWorkNotifications { get; }
        public IRemindWorkRepository RemindWorks { get; }
        Task<int> CompleteAsync();
    }
}
