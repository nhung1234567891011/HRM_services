using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.IServices;
using HRM_BE.Data.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Data.ProfileEntity;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Data.Migrations;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.Data.Task;


namespace HRM_BE.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HrmContext _context;

        public UnitOfWork(HrmContext context, IMapper mapper, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, ITimesheetCalculationService calculationService)
        {
            _context = context;
            Banners = new BannerRepository(context, mapper,httpContextAccessor);
            Companies = new CompanyRepository(context, mapper, httpContextAccessor);
            Organizations = new OrganizationRepository(context, mapper, httpContextAccessor);
            OrganizationTypes = new OrgainizationTypeRepository(context, mapper, httpContextAccessor);
            GroupPositions = new GroupPositionRepository(context, mapper, httpContextAccessor);
            StaffTitles = new StaffTitleRepository(context, mapper, httpContextAccessor);
            StaffPositions = new StaffPositionRepository(context, mapper, httpContextAccessor);
            Employees = new EmployeeRepository(context, mapper, httpContextAccessor);
            ContractDurations = new ContractDurationRepository(context, mapper, httpContextAccessor);
            ContractTypes = new ContractTypeRepository(context, mapper, httpContextAccessor);
            WorkingForms = new WorkingFormRepository(context, mapper, httpContextAccessor);
            ContactInfos = new ContactInfoRepository(context, mapper, httpContextAccessor);
            Contracts = new ContractRepository(context, mapper, httpContextAccessor);
            Allowances = new AllowanceRepository(context, mapper, httpContextAccessor);
            ProfileInfos = new ProfileInfoRepository(context, mapper, httpContextAccessor);
            Profiles = new ProfileRepository(context, mapper, httpContextAccessor);
            Deductions = new DeductionRepository(context, mapper, httpContextAccessor);
            JobInfos = new JobInfoRepository(context, mapper, httpContextAccessor);
            NatureOfLabors = new NatureOfLaborRepository(context, mapper, httpContextAccessor);
            Address = new AddressRepository(context, httpContextAccessor);
            GeneralLeaveRegulation = new GeneralLeaveRegulationRepository(context, mapper, httpContextAccessor);
            LeaveApplications= new LeaveApplicationRepository(context, mapper, httpContextAccessor);
            CheckInCheckOutApplications = new CheckInCheckOutApplicationRepository(context, mapper, httpContextAccessor);
            TypeOfLeave = new TypeOfLeaveRepository(context, mapper, httpContextAccessor);
            Holiday = new HolidayRepository(context, mapper, httpContextAccessor);
            WorkFactor = new WorkFactorRepository(context, mapper, httpContextAccessor);
            TimekeepingRegulation = new TimekeepingRegulationRepository(context, mapper, httpContextAccessor);
            ShiftCatalogs = new ShiftCatalogRepository(context, mapper, httpContextAccessor);
            ShiftWorks = new ShiftWorkRepository(context, mapper, httpContextAccessor);
            DetailTimeSheets = new DetailTimeSheetRepository(context, mapper, httpContextAccessor);
            TimekeepingLocation = new TimekeepingLocationRepository(context, mapper, httpContextAccessor);
            ApplyOrganization = new ApplyOrganizationRepository(context, mapper, httpContextAccessor);
            TimekeepingSetting = new TimekeepingSettingRepository(context, mapper, httpContextAccessor);    
            SummaryTimeSheets = new SummaryTimeSheetRepository(context, mapper,httpContextAccessor);
            TypeOfLeaveEmployee= new TypeOfLeaveEmployeeRepository(context, mapper, httpContextAccessor);
            Timesheet = new TimesheetRepository(context, mapper, httpContextAccessor, calculationService);
            SummaryTimesheetNameEmployeeConfirms= new SummaryTimesheetNameEmployeeConfirmRepository(context, mapper, httpContextAccessor);
            PrefixConfigs = new PrefixConfigRepository(context, mapper, httpContextAccessor);
            KpiTables = new KpiTableRepository(context, mapper, httpContextAccessor);
            LeavePermissions = new LeavePermissionRepository(context, mapper, httpContextAccessor);
            SalaryComponents = new SalaryComponentRepository(context, mapper, httpContextAccessor);
            KpiTableDetails = new KpiTableDetailRepository(context, mapper, httpContextAccessor);
            KpiTableDetails = new KpiTableDetailRepository(context, mapper, httpContextAccessor);
            LeavePermissions = new LeavePermissionRepository(context, mapper, httpContextAccessor);
            Payrolls = new PayrollRepository(context, mapper, httpContextAccessor);
            PayrollDetails = new PayrollDetailRepository(context, mapper, httpContextAccessor);
            PayrollInquiries = new PayrollInquiryRepository(context, mapper, httpContextAccessor);
            RevenueCommissionPolicies = new RevenueCommissionPolicyRepository(context, mapper, httpContextAccessor);
            Departments=new DepartmentRepository(context, mapper, httpContextAccessor);
            Projects=new ProjectRepository(context, mapper,httpContextAccessor);
            Delegations = new DelegationRepository(context, mapper, httpContextAccessor);
            Comments=new CommentRepository(context, mapper, httpContextAccessor);
            GroupWorks = new GroupWorkRepository(context, mapper, httpContextAccessor);
            Works = new WorkRepository(context, mapper, httpContextAccessor);
            Tags = new TagRepository(context, mapper, httpContextAccessor);
            RemindWorkNotifications = new RemindWorkNotificationRepository(context, mapper, httpContextAccessor);
            RemindWorks = new RemindWorkRepository(context, mapper, httpContextAccessor);
            UserConnections = new UserConnectionRepository(context,mapper,httpContextAccessor);
            Reports = new ReportRepository(context);
        }

        public ILeavePermissionRepository LeavePermissions { get; private set; }
        //Salary 

        public IPayrollRepository Payrolls { get; private set; }
        public IPayrollDetailRepository PayrollDetails { get; private set; }
        public IPayrollInquiryRepository PayrollInquiries { get; private set; }
        public IRevenueCommissionPolicyRepository RevenueCommissionPolicies { get; private set; }
        public ISalaryComponentRepository SalaryComponents { get; private set; }
        public IKpiTableRepository KpiTables { get; private set; }
        public IKpiTableDetailRepository KpiTableDetails { get; private set; }
        public IPrefixConfigRepository PrefixConfigs { get; private set; }
        public IBannerRepository Banners { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public IOrganizationTypeRepository OrganizationTypes { get; private set; }
        public IOrganizationRepository Organizations { get; private set; }
        public IGroupPositionRepository GroupPositions { get; private set; }
        public IStaffTitleRepository StaffTitles { get; private set; }
        public IStaffPositionRepository StaffPositions{ get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IContractDurationRepository ContractDurations { get; private set; }
        public IContractTypeRepository ContractTypes { get; private set; }
        public IWorkingFormRepository WorkingForms { get; private set; }
        public IContactInfoRepository ContactInfos { get; private set; }
        public IContractRepository Contracts { get; private set; }
        public IAllowanceRepository Allowances { get; private set; }
        public IProfileInfoRepository ProfileInfos { get; private set; }
        public IProfileRepository Profiles { get; private set; }
        public IDeductionRepository Deductions { get; private set; }
        public IJobInfoRepository JobInfos { get; private set; }
        public INatureOfLaborRepository NatureOfLabors {  get; private set; }
        public IAddressRepository Address { get; private set; }
        public IGeneralLeaveRegulationRepository GeneralLeaveRegulation { get; private set; }
        public ITypeOfLeaveRepository TypeOfLeave { get; private set; }
        public ITypeOfLeaveEmployeeRepository TypeOfLeaveEmployee { get; private set; }
        public IHolidayRepository Holiday { get; private set; }
        public IWorkFactorRepository WorkFactor { get; private set; }
        public ITimekeepingRegulationRepository TimekeepingRegulation { get; private set; }
        public ITimekeepingLocationRepository TimekeepingLocation { get; private set; }
        public ITimekeepingSettingRepository TimekeepingSetting { get; private set; }
        public ILeaveApplicationRepository LeaveApplications { get; private set; }
        public ICheckInCheckOutApplicationRepository CheckInCheckOutApplications { get; private set; }
        public IShiftCatalogRepository ShiftCatalogs { get; private set; }
        public IShiftWorkRepository ShiftWorks { get; private set; }
        public IDetailTimeSheetRepository DetailTimeSheets { get; private set; }
        public IApplyOrganizationRepository ApplyOrganization { get; private set; }
        public ISummaryTimeSheetRepository SummaryTimeSheets { get; private set; }
        public ITimesheetRepository Timesheet { get; private set; }
        public ISummaryTimesheetNameEmployeeConfirmRepository SummaryTimesheetNameEmployeeConfirms { get; }
        public IDepartmentRepository Departments { get; }
        public IProjectRepository Projects { get; }
        public IDelegationRepository Delegations { get; }
        public ICommentRepository Comments { get; }
        public IGroupWorkRepository GroupWorks { get; private set; }
        public IWorkRepository Works { get; private set; }
        public ITagRepository Tags { get; private set; }
        public IUserConnectionRepository UserConnections { get; private set; }
        public IRemindWorkNotificationRepository RemindWorkNotifications { get; private set; }
        public IRemindWorkRepository RemindWorks { get; private set; }
        public IReportRepository Reports { get; private set; }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
