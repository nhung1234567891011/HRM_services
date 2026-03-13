namespace HRM_BE.Core.Constants.Identity
{
    public class PermissionConstant
    {
        // Master Permissions
        public const string Admin = "A";
        public const string Master = "M";
        public const string Employee = "E";

        /// <summary>
        /// Human Resources Information
        /// </summary>

        // Company Information Permissions
        public const string ManageCompanyInformation = "MCI";
        public const string ManageCompanyInformationView = "MCI.V";
        public const string ManageCompanyInformationCreate = "MCI.C";
        public const string ManageCompanyInformationEdit = "MCI.E";
        public const string ManageCompanyInformationDelete = "MCI.D";

        // Organizational Structure + Diagram Permissions
        public const string ManageOrganizationalStructure = "MOS";
        public const string ManageOrganizationalStructureView = "MOS.V";
        public const string ManageOrganizationalStructureCreate = "MOS.C";
        public const string ManageOrganizationalStructureEdit = "MOS.E";
        public const string ManageOrganizationalStructureDelete = "MOS.D";

        // Position Permissions
        public const string ManagePosition = "MP";
        public const string ManagePositionView = "MP.V";
        public const string ManagePositionCreate = "MP.C";
        public const string ManagePositionEdit = "MP.E";
        public const string ManagePositionDelete = "MP.D";

        // Object Permissions
        public const string ManageObject = "MO";
        public const string ManageObjectView = "MO.V";
        public const string ManageObjectCreate = "MO.C";
        public const string ManageObjectEdit = "MO.E";
        public const string ManageObjectDelete = "MO.D";
        public const string ManageObjectSendMail = "MO.SM";
        public const string ManageObjectChangePosition = "MO.CP";

        // Profile Permissions
        public const string ManageProfile = "MPR";
        public const string ManageProfileView = "MPR.V";
        public const string ManageProfileCreate = "MPR.C";
        public const string ManageProfileEdit = "MPR.E";
        public const string ManageProfileDelete = "MPR.D";

        // Contract Permissions
        public const string ManageContract = "MC";
        public const string ManageContractView = "MC.V";
        public const string ManageContractCreate = "MC.C";
        public const string ManageContractEdit = "MC.E";
        public const string ManageContractDelete = "MC.D";
        public const string ManageContractExport = "MC.EX";
        public const string ManageContractTerminate = "MC.T"; // Quyền chấm dứt hợp đồng


        /// <summary>
        /// Timekeeping
        /// </summary>

        // Timekeeping Location Permissions
        // Quyền quản lý thông tin địa điểm chấm công
        public const string TimekeepingLocation = "MTL";
        // Quyền xem thông tin địa điểm chấm công
        public const string TimekeepingLocationView = "MTL.V";
        // Quyền tạo mới thông tin địa điểm chấm công
        public const string TimekeepingLocationCreate = "MTL.C";
        // Quyền chỉnh sửa thông tin địa điểm chấm công
        public const string TimekeepingLocationEdit = "MTL.E";
        // Quyền xóa thông tin địa điểm chấm công
        public const string TimekeepingLocationDelete = "MTL.D";

        // Unit Application Permissions
        // Quyền quản lý thông tin đơn vị áp dụng
        public const string UnitApplication = "MUA";
        // Quyền xem thông tin đơn vị áp dụng
        public const string UnitApplicationView = "MUA.V";
        // Quyền tạo mới thông tin đơn vị áp dụng
        public const string UnitApplicationCreate = "MUA.C";
        // Quyền chỉnh sửa thông tin đơn vị áp dụng
        public const string UnitApplicationEdit = "MUA.E";
        // Quyền xóa thông tin đơn vị áp dụng
        public const string UnitApplicationDelete = "MUA.D";


        // Timekeeping Rules Permissions
        public const string ManageTimekeepingRules = "MTR";
        public const string ManageTimekeepingRulesView = "MTR.V";
        public const string ManageTimekeepingRulesCreate = "MTR.C";
        public const string ManageTimekeepingRulesEdit = "MTR.E";
        public const string ManageTimekeepingRulesDelete = "MTR.D";

        // Leave Regulations Permissions
        public const string ManageLeaveRegulations = "MLR";
        public const string ManageLeaveRegulationsView = "MLR.V";
        public const string ManageLeaveRegulationsCreate = "MLR.C";
        public const string ManageLeaveRegulationsEdit = "MLR.E";
        public const string ManageLeaveRegulationsDelete = "MLR.D";

        // Shift Setup Permissions
        public const string ManageShiftSetup = "MSS";
        public const string ManageShiftSetupView = "MSS.V";
        public const string ManageShiftSetupCreate = "MSS.C";
        public const string ManageShiftSetupEdit = "MSS.E";
        public const string ManageShiftSetupDelete = "MSS.D";

        // Shift Allocation Permissions
        public const string ManageShiftAllocation = "MSA";
        public const string ManageShiftAllocationView = "MSA.V";
        public const string ManageShiftAllocationCreate = "MSA.C";
        public const string ManageShiftAllocationEdit = "MSA.E";
        public const string ManageShiftAllocationDelete = "MSA.D";

        // Detailed Timekeeping Permissions
        public const string ManageDetailedTimekeeping = "MDT";
        public const string ManageDetailedTimekeepingView = "MDT.V";
        public const string ManageDetailedTimekeepingCreate = "MDT.C";
        public const string ManageDetailedTimekeepingEdit = "MDT.E";
        public const string ManageDetailedTimekeepingDelete = "MDT.D";
        public const string ManageDetailedTimekeepingLock = "MDT.L";

        // General Timekeeping Permissions
        public const string ManageGeneralTimekeeping = "MGT";
        public const string ManageGeneralTimekeepingView = "MGT.V";
        public const string ManageGeneralTimekeepingCreate = "MGT.C";
        public const string ManageGeneralTimekeepingEdit = "MGT.E";
        public const string ManageGeneralTimekeepingDelete = "MGT.D";
        public const string ManageGeneralTimekeepingTransferSalary = "MGT.TS";
        public const string ManageGeneralTimekeepingSendConfirmation = "MGT.SC";
        public const string ManageGeneralTimekeepingFeedback = "MGT.FB";


        // Leave Regulations Permissions
        // Quyền quản lý thông tin quy định nghỉ - loại nghỉ
        public const string LeaveTypes = "MLT";
        // Quyền xem thông tin quy định nghỉ - loại nghỉ
        public const string LeaveTypesView = "MLT.V";
        // Quyền tạo mới thông tin quy định nghỉ - loại nghỉ
        public const string LeaveTypesCreate = "MLT.C";
        // Quyền chỉnh sửa thông tin quy định nghỉ - loại nghỉ
        public const string LeaveTypesEdit = "MLT.E";
        // Quyền xóa thông tin quy định nghỉ - loại nghỉ
        public const string LeaveTypesDelete = "MLT.D";


        // Public Holiday Permissions
        // Quyền quản lý thông tin quy định nghỉ lễ
        public const string PublicHolidays = "MPH";
        // Quyền xem thông tin quy định nghỉ lễ
        public const string PublicHolidaysView = "MPH.V";
        // Quyền tạo mới thông tin quy định nghỉ lễ
        public const string PublicHolidaysCreate = "MPH.C";
        // Quyền chỉnh sửa thông tin quy định nghỉ lễ
        public const string PublicHolidaysEdit = "MPH.E";
        // Quyền xóa thông tin quy định nghỉ lễ
        public const string PublicHolidaysDelete = "MPH.D";

        /// <summary>
        /// Payroll 
        /// </summary>

        // Salary Components Permissions
        public const string ManageSalaryComponents = "MSC";
        public const string ManageSalaryComponentsView = "MSC.V";
        public const string ManageSalaryComponentsCreate = "MSC.C";
        public const string ManageSalaryComponentsEdit = "MSC.E";
        public const string ManageSalaryComponentsDelete = "MSC.D";

        // KPI Permissions
        public const string ManageKPI = "MKPI";
        public const string ManageKPIView = "MKPI.V";
        public const string ManageKPICreate = "MKPI.C";
        public const string ManageKPIEdit = "MKPI.E";
        public const string ManageKPIDelete = "MKPI.D";

        // Payroll Table Permissions
        public const string ManagePayrollTable = "MPT";
        public const string ManagePayrollTableView = "MPT.V";
        public const string ManagePayrollTableCreate = "MPT.C";
        public const string ManagePayrollTableEdit = "MPT.E";
        public const string ManagePayrollTableDelete = "MPT.D";
        public const string ManagePayrollTableSend = "MPT.S";
        public const string ManagePayrollTableFeedback = "MPT.FB";

        // Statistical Report Permissions
        public const string ManageStatisticalReport = "MSR";
        public const string ManageStatisticalReportView = "MSR.V";

    }
}



//// Timekeeping Permissions
//public const string ManageTimekeeping = "MT";
//public const string ManageTimekeepingView = "MT.V";
//public const string ManageTimekeepingCreate = "MT.C";
//public const string ManageTimekeepingEdit = "MT.E";
//public const string ManageTimekeepingDelete = "MT.D";

//// Human Resources Information Permissions
//public const string ManageHumanResourcesInformation = "MHRI";
//public const string ManageHumanResourcesInformationView = "MHRI.V";
//public const string ManageHumanResourcesInformationCreate = "MHRI.C";
//public const string ManageHumanResourcesInformationEdit = "MHRI.E";
//public const string ManageHumanResourcesInformationDelete = "MHRI.D";

//// Payroll Permissions
//public const string ManagePayroll = "MPa";
//public const string ManagePayrollView = "MPa.V";
//public const string ManagePayrollCreate = "MPa.C";
//public const string ManagePayrollEdit = "MPa.E";
//public const string ManagePayrollDelete = "MPa.D";