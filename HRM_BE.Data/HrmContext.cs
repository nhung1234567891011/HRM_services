using HRM_BE.Core.Constants.System;
using HRM_BE.Core.Data;
using HRM_BE.Core.Data.Address;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Data.Content;
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.Data.ProfileEntity;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Data.Task;
using HRM_BE.Data.DataSeeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Data;
using System.Reflection.Emit;
using System.Xml.Linq;
namespace HRM_BE.Data
{
    public class HrmContext : IdentityDbContext<User, Role, int>
    {
        public HrmContext()
        {
        }
        public HrmContext(DbContextOptions options) : base(options)
        {
        }
        // prefix config
        public virtual DbSet<PrefixConfig> PrefixConfigs { get; set; } = null!;
        public virtual DbSet<LeavePermission> LeavePermissions { get; set; } = null!;
        //banner
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        //company
        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<StaffTitle> StaffTitles { get; set; }
        public virtual DbSet<StaffPosition> StaffPositions { get; set; }
        public virtual DbSet<GroupPosition> GroupPositions { get; set; }
        public virtual DbSet<OrganizationLeader> OrganizationLeaders { get; set; }
        //Entity
        public virtual DbSet<Permission>? Permissions { get; set; }
        public virtual DbSet<RolePermission>? RolePermissions { get; set; }
        public virtual DbSet<OrganizationPosition> OrganizationPositions { get; set; }
        //Profile 
        public virtual DbSet<Allowance> Allowances { get; set; }
        public virtual DbSet<ContractType> ContractTypes { get; set; }
        public virtual DbSet<NatureOfLabor> NatureOfLabor { get; set; }
        public virtual DbSet<ContractDuration> ContractDurations { get; set; }
        public virtual DbSet<WorkingForm> WorkingForms { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ContactInfo> ContactInfos { get; set; }
        public virtual DbSet<ProfileInfo> ProfileInfos { get; set; }
        public virtual DbSet<Deduction> Deductions { get; set; }
        public virtual DbSet<JobInfo> InfoJobs { get; set; }

        //address
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;

        //leave regulation

        public virtual DbSet<GeneralLeaveRegulation> GeneralLeaveRegulations { get; set; } = null!;

        public virtual DbSet<TypeOfLeave> TypeOfLeaves { get; set; } = null!;

        public virtual DbSet<TypeOfLeaveEmployee> TypeOfLeaveEmployees { get; set; } = null!;

        public virtual DbSet<Holiday> Holidays { get; set; } = null!;

        public virtual DbSet<WorkFactor> WorkFactors { get; set; } = null!;


        // Payroll
        public virtual DbSet<SalaryComponent> SalaryComponents { get; set; } = null!;
        public virtual DbSet<Payroll> Payrolls { get; set; } = null!;
        public virtual DbSet<PayrollSummaryTimesheet> PayrollSummaryTimesheets { get; set; }
        public virtual DbSet<PayrollStaffPosition> PayrollStaffPositions { get; set; }
        public virtual DbSet<PayrollDetail> PayrollDetails { get; set; } = null!;
        public virtual DbSet<PayrollInquiry> PayrollInquiries { get; set; } = null!;
        public virtual DbSet<RevenueCommissionPolicy> RevenueCommissionPolicies { get; set; } = null!;
        public virtual DbSet<RevenueCommissionTier> RevenueCommissionTiers { get; set; } = null!;

        //shift
        public virtual DbSet<DetailTimesheetName> DetailTimesheetNames { get; set; } = null!;

        public virtual DbSet<DetailTimesheetNameStaffPosition> DetailTimesheetNameStaffPositions { get; set; } = null!;

        public virtual DbSet<ShiftCatalog> ShiftCatalogs { get; set; } = null!;

        public virtual DbSet<ShiftWork> ShiftWorks { get; set; } = null!;

        public virtual DbSet<SummaryTimesheetName> SummaryTimesheetNames { get; set; } = null!;

        public virtual DbSet<SummaryTimesheetNameStaffPosition> SummaryTimesheetNameStaffPositions { get; set; } = null!;

        public virtual DbSet<SummaryTimesheetNameDetailTimesheetName> SummaryTimesheetNameDetailTimesheetNames { get; set; } = null!;

        //time keeping regulation

        public virtual DbSet<StandardWorkNumber> StandardWorkNumbers { get; set; } = null!;

        public virtual DbSet<TimekeepingSetting> TimekeepingSettings { get; set; } = null!;

        public virtual DbSet<TimekeepingLocation> TimekeepingLocations { get; set; } = null!;

        public virtual DbSet<ApplyOrganization> ApplyOrganizations { get; set; } = null!;

        public virtual DbSet<ApplyEmployeeTimekeepingSetting> ApplyEmployeeTimekeepingSettings { get; set; } = null!;

        public virtual DbSet<TimekeepingRegulation> TimekeepingRegulations { get; set; } = null!;

        public virtual DbSet<Timesheet> Timesheets { get; set; } = null!;

        public virtual DbSet<SummaryTimesheetNameEmployeeConfirm> SummaryTimesheetNameEmployeeConfirms { get; set; } = null!;

        //Official form
       
        public virtual DbSet<LeaveApplication> LeaveApplications { get; set; } = null!;

        public virtual DbSet<CheckInCheckOutApplication> CheckInCheckOutApplications { get; set; } = null!;

        public virtual DbSet<LeaveApplicationApprover> LeaveApplicationApprovers { get; set; } = null!;

        public virtual DbSet<LeaveApplicationReplacement> LeaveApplicationReplacements { get; set; } = null!;

        public virtual DbSet<LeaveApplicationRelatedPerson> LeaveApplicationRelatedPeople { get; set; } = null!;

        public virtual DbSet<ResignationLetter> ResignationLetters { get; set; } = null!;


        //Salary
        public virtual DbSet<KpiTable> KpiTables { get; set; } = null!;
        public virtual DbSet<KpiTableDetail> KpiTableDetails { get; set; } = null!;
        public virtual DbSet<KpiTablePosition> KpiTablePositions { get; set; } = null!;


        //Task Sprint 4 Công việc
        public virtual DbSet<Approval> Approvals { get; set; } = null!;
        public virtual DbSet<CheckList> CheckLists { get; set; } = null!;
        public virtual DbSet<GroupWork> GroupWorks { get; set; } = null!;
        public virtual DbSet<RepeatWork> RepeatWorks { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<TagWork> TagsWork { get; set; } = null!;
        public virtual DbSet<Work> Works { get; set; } = null!;


        //department
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<DepartmentEmployee> DepartmentEmployees { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; } = null!;
        public virtual DbSet<DepartmentRole> DepartmentRoles { get; set; } = null!;
        public virtual DbSet<ProjectRole> ProjectRoles { get; set; } = null!;
        public virtual DbSet<Delegation> Delegations { get; set; } = null!;
        public virtual DbSet<DelegationEmployee> DelegationEmployees { get; set; } = null!;
        public virtual DbSet<DelegationProject> DelegationProjects { get; set; } = null!;
        //comment
        public virtual DbSet<Comment> Comments { get; set; } = null!;

        public virtual DbSet<RemindWorkNotification> RemindWorkNotifications { get; set; } = null!;
        public virtual DbSet<RemindWork> RemindWorks { get; set; } = null!;
        public virtual DbSet<UserConnection> UserConnections { get; set; } = null!;
        public virtual DbSet<ScheduleJob> ScheduleJobs { get; set; } = null!;
        public virtual DbSet<ApprovalEmployee> ApprovalEmployees { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");

            builder.Entity<Role>().ToTable("Roles");

            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims").HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims")
            .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles")
            .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens")
               .HasKey(x => new { x.UserId });

            builder.Entity<IdentityUserRole<int>>()
               .HasOne<User>()
               .WithMany(u => u.UserRoles)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentityUserRole<int>>()
                .HasOne<Role>()
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkAssignment>().ToTable("WorkAssignments")
                .HasKey( x => new {x.WorkId, x.AssigneeId} );



            builder.Entity<Deduction>().HasQueryFilter(x => x.IsDeleted == false);
            //Cấu hình Global Query Filter để tự động loại bỏ bản ghi có IsDeleted = true
            builder.Entity<Contract>()
                .HasQueryFilter(o => o.IsDeleted == false);  // Lọc các bản ghi có IsDeleted = false
            builder.Entity<ShiftCatalog>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<ShiftWork>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<DetailTimesheetName>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<Employee>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<Tag>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<Work>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<GroupWork>()
             .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<RevenueCommissionPolicy>()
                .HasQueryFilter(s => s.IsDeleted == false);
            builder.Entity<RevenueCommissionTier>()
                .HasQueryFilter(s => s.IsDeleted == false);
            // Cấu hình Global Query Filter để tự động loại bỏ bản ghi có IsDeleted = true
            builder.Entity<Organization>()
                .HasQueryFilter(o => o.IsDeleted == false);  // Lọc các bản ghi có IsDeleted = false
           
            // Cấu hình bảng kết nối giữa StaffPosition và Organization
            builder.Entity<OrganizationPosition>()
                .HasKey(spo => new { spo.StaffPositionId, spo.OrganizationId });

            // Cấu hình bảng kết nối giữa StaffPosition và KpiTable
            builder.Entity<KpiTablePosition>()
                .HasKey(spo => new { spo.StaffPositionId, spo.KpiTableId });

            //builder.Entity<OrganizationPosition>()
            //    .HasOne(spo => spo.StaffPosition)
            //    .WithMany(spo => spo.OrganizationPositions)
            //    .HasForeignKey(spo => spo.StaffPositionId);

            //builder.Entity<OrganizationPosition>()
            //    .HasOne(spo => spo.Organization)
            //    .WithMany(o => o.StaffPositionOrganizations)
            //    .HasForeignKey(spo => spo.OrganizationId);

            // Mối quan hệ giữa Organization và Employee thông qua OrganizationLeader
            builder.Entity<OrganizationLeader>()
                .HasKey(ol => new { ol.OrganizationId, ol.EmployeeId });

            builder.Entity<OrganizationLeader>()
                .HasOne(ol => ol.Organization)
                .WithMany(o => o.OrganizationLeaders)
                .HasForeignKey(ol => ol.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction); // Cấm cascade khi xóa


            builder.Entity<OrganizationLeader>()
                .HasOne(ol => ol.Employee)
                .WithMany(e => e.OrganizationLeaders)
                .HasForeignKey(ol => ol.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction); // Cấm hành động cascade khi xóa;

            //builder.Entity<Employee>()
            //    .HasOne(e => e.Manager)
            //    .WithMany()
            //    .HasForeignKey(e => e.ManagerId)
            //    .OnDelete(DeleteBehavior.NoAction);
            //seeder 


            // Cấu hình quan hệ tự tham chiếu cho Manager
            //builder.Entity<Employee>()
            //    .HasOne(e => e.Manager)
            //    .WithMany()
            //    .HasForeignKey(e => e.ManagerId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ tự tham chiếu cho ManagerDirect
            builder.Entity<Employee>()
                .HasOne(e => e.ManagerDirect)
                .WithMany()
                .HasForeignKey(e => e.ManagerDirectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ tự tham chiếu cho ManagerIndirect
            builder.Entity<Employee>()
                .HasOne(e => e.ManagerIndirect)
                .WithMany()
                .HasForeignKey(e => e.ManagerIndirectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ tự tham chiếu cho EmployeeApprove
            builder.Entity<Employee>()
                .HasOne(e => e.EmployeeApprove)
                .WithMany()
                .HasForeignKey(e => e.EmployeeApproveId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PayrollSummaryTimesheet>()
                .HasKey(pst => new { pst.PayrollId, pst.SummaryTimesheetNameId });

            builder.Entity<PayrollSummaryTimesheet>()
                .HasOne(pst => pst.Payroll)
                .WithMany(p => p.PayrollSummaryTimesheets)
                .HasForeignKey(pst => pst.PayrollId);

            builder.Entity<PayrollSummaryTimesheet>()
                .HasOne(pst => pst.SummaryTimesheetName)
                .WithMany(p => p.PayrollSummaryTimesheets)
                .HasForeignKey(pst => pst.SummaryTimesheetNameId);

            builder.Entity<PayrollStaffPosition>()
                .HasKey(psp => new { psp.PayrollId, psp.StaffPositionId });

            builder.Entity<PayrollStaffPosition>()
                .HasOne(psp => psp.Payroll)
                .WithMany(p => p.PayrollStaffPositions)
                .HasForeignKey(psp => psp.PayrollId);

            builder.Entity<PayrollStaffPosition>()
                .HasOne(psp => psp.StaffPosition)
                .WithMany(p => p.PayrollStaffPositions)
                .HasForeignKey(psp => psp.StaffPositionId);

            // Employee -> PayrollDetail: 1-n
            builder.Entity<Employee>()
                .HasMany(e => e.PayrollDetails)
                .WithOne(pd => pd.Employee)
                .HasForeignKey(pd => pd.EmployeeId);

            // Payroll -> PayrollDetail: 1-n
            builder.Entity<Payroll>()
                .HasMany(p => p.PayrollDetails)
                .WithOne(pd => pd.Payroll)
                .HasForeignKey(pd => pd.PayrollId);

            builder.Entity<RevenueCommissionPolicy>()
                .HasMany(p => p.Tiers)
                .WithOne(t => t.Policy)
                .HasForeignKey(t => t.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);


            // Sprint 4 Công việc HRM
            builder.Entity<DelegationEmployee>().HasKey(de => new { de.EmployeeId, de.DelegationId });
            builder.Entity<DelegationProject>().HasKey(dp => new { dp.ProjectId, dp.DelegationId });
            builder.Entity<DepartmentEmployee>().HasKey(de => new { de.EmployeeId, de.DepartmentId});
            builder.Entity<ProjectEmployee>().HasKey(pe => new { pe.EmployeeId, pe.ProjectId });
            builder.Entity<TagWork>().HasKey(tw => new { tw.WorkId, tw.TagId });



            //Official form
            builder.Entity<LeaveApplicationApprover>().HasKey(laa => new { laa.ApproverId, laa.LeaveApplicationId });
            builder.Entity<LeaveApplicationReplacement>().HasKey(lar => new { lar.ReplacementId, lar.LeaveApplicationId });
            builder.Entity<LeaveApplicationRelatedPerson>().HasKey(lap => new { lap.RelatedPersonId, lap.LeaveApplicationId });

            builder.Entity<CheckInCheckOutApplication>()
                .HasQueryFilter(x => x.IsDeleted == null || x.IsDeleted == false);

            //TimeKeeping
            builder.Entity<DetailTimesheetNameStaffPosition>().HasKey(laa => new { laa.StaffPositionId, laa.DetailTimesheetNameId });
            builder.Entity<SummaryTimesheetNameDetailTimesheetName>().HasKey(laa => new { laa.SummaryTimesheetNameId, laa.DetailTimesheetNameId });
            builder.Entity<SummaryTimesheetNameStaffPosition>().HasKey(laa => new { laa.SummaryTimesheetNameId, laa.StaffPositionId });

            //Department
            builder.Entity<DepartmentEmployee>().HasKey(laa => new { laa.DepartmentId, laa.EmployeeId });
            builder.Entity<ProjectEmployee>().HasKey(laa => new { laa.ProjectId, laa.EmployeeId });

            builder.Entity<DelegationProject>().HasKey(laa => new { laa.DelegationId, laa.ProjectId });
            builder.Entity<DelegationEmployee>().HasKey(laa => new { laa.DelegationId, laa.EmployeeId });

            // work
            builder.Entity<RemindWorkNotification>()
                   .HasOne(r => r.Work)
                   .WithMany()
                   .HasForeignKey(r => r.WorkId)
                   .OnDelete(DeleteBehavior.NoAction); // Không cho phép cascade delete từ Work
                                                       // Cấu hình mối quan hệ với RemindWork
            builder.Entity<RemindWorkNotification>()
                .HasOne(r => r.RemindWork)
                .WithMany()
                .HasForeignKey(r => r.RemindWorkId)
                .OnDelete(DeleteBehavior.NoAction); // Không cho phép cascade delete từ RemindWork
                                                    // Mối quan hệ 1-n giữa Approval và ApprovalEmployee
            builder.Entity<Approval>()
                .HasMany(a => a.ApprovalEmployees) // Approval có nhiều ApprovalEmployee
                .WithOne(ae => ae.Approval) // ApprovalEmployee thuộc về một Approval
                .HasForeignKey(ae => ae.ApprovalId) // Khóa ngoại ApprovalId trong ApprovalEmployee
                .OnDelete(DeleteBehavior.NoAction); // Khi xóa Approval, xóa luôn ApprovalEmployee
            //seed
            builder.Seed();


        }



        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
               .Entries()
               .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                var dateCreatedProp = entityEntry.Entity.GetType().GetProperty(SystemConstant.DateCreatedField);
                if ((entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Modified)
                    && dateCreatedProp != null)
                {
                    dateCreatedProp.SetValue(entityEntry.Entity, DateTime.Now);
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
