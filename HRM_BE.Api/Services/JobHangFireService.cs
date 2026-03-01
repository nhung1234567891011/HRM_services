using Hangfire;
using Hangfire.Processing;
using HRM_BE.Api.Hubs;
using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.Constants.Mail;
using HRM_BE.Core.Data;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Task;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Models.Mail;
using HRM_BE.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace HRM_BE.Api.Services
{
    public class JobHangFireService : IJobHangFireService
    {
        private readonly HrmContext _dbContext;
        private readonly IHubContext<RemindWorkHub> _remindWorkHub;
        private readonly IMailService _mailService;
        public JobHangFireService( HrmContext dbContext, IHubContext<RemindWorkHub> remindWorkHub, IMailService mailService)
        {
            _dbContext = dbContext;
            _remindWorkHub = remindWorkHub;
            _mailService = mailService;
        }

        public async Task CreateTimeSheet(int employeeId, DateTime date, int shiftWorkId, double workingHours,TimeKeepingLeaveStatus timeKeepingLeaveStatus)
        {
            await _dbContext.Timesheets.AddAsync(new Timesheet
            {
                EmployeeId = employeeId,
                Date = date,
                ShiftWorkId = shiftWorkId,
                NumberOfWorkingHour = workingHours,
                EarlyLeaveDuration = 0,
                LateDuration = 0,
                TimeKeepingLeaveStatus = timeKeepingLeaveStatus
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task TriggerChangeLeaveNumber(int employeeId, int contractId, double NumberOffLeave)
        {
            // Kiểm tra nhân viên có tồn tại và chưa bị xóa
            var employeeExists = await _dbContext.Employees
                .AnyAsync(e => e.Id == employeeId && (e.IsDeleted == null || e.IsDeleted == false));

            if (!employeeExists)
            {
                // Log lỗi và bỏ qua xử lý
                Console.WriteLine($"Nhân viên {employeeId} không tồn tại hoặc đã bị xóa. Bỏ qua tạo leave permission.");
                return;
            }

            // Kiểm tra hợp đồng có tồn tại và chưa bị xóa
            var contractExists = await _dbContext.Contracts
                .AnyAsync(c => c.Id == contractId && (c.IsDeleted == null || c.IsDeleted == false));

            if (!contractExists)
            {
                Console.WriteLine($"Hợp đồng {contractId} không tồn tại hoặc đã bị xóa. Bỏ qua tạo leave permission.");
                return;
            }

            var leavePermission = await _dbContext.LeavePermissions
                .Where(l => l.EmployeeId == employeeId && l.ContractId == contractId)
                .FirstOrDefaultAsync();

            if (leavePermission is null)
            {
                await _dbContext.LeavePermissions.AddAsync(new LeavePermission
                {
                    EmployeeId = employeeId,
                    ContractId = contractId,
                    Date = DateTime.Now,
                    LeavePerrmissionStatus = LeavePerrmissionStatus.Active,
                    NumerOfLeave = NumberOffLeave
                });
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                leavePermission.NumerOfLeave += NumberOffLeave;
                await _dbContext.SaveChangesAsync();
            }
        }

        public void ScheduleRecurringJob(int employeeId, int contractId, int NumberOfLeave, DateTime startDay, DateTime endDay, int DayInMonth)
        {
            var jobId = $"AddLeaveDays_Contract_{contractId}";

            RecurringJob.AddOrUpdate<JobHangFireService>(
                jobId, // ID công việc
                j => j.TriggerChangeLeaveNumber(employeeId, contractId, NumberOfLeave),
                Cron.Monthly(DayInMonth), // Lặp lại hàng tháng vào ngày cụ thể
                TimeZoneInfo.Local
            );
            // xoá chu kỳ lặp vào thời gian hết hạn hợp đồng
            BackgroundJob.Schedule(() => RecurringJob.RemoveIfExists(jobId), endDay.AddDays(1) - DateTime.Now);

        }
        private int CountDays(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date must be greater than or equal to start date.");

            return (endDate - startDate).Days + 1; // Thêm 1 để tính cả ngày bắt đầu
        }

        public async Task UpdateExpireContractStatus(int contractId)
        {
            var entity = await _dbContext.Contracts.FindAsync(contractId);
            if (entity is null)
            {
                // log and exit; contract no longer exists
                return;
            }

            entity.ExpiredStatus = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task ReSendRemindWorkNotification(int workId, int employeeId, string workName, RemindWorkType remindWorkType, DateTime? startDate, DateTime? dueDate, double? timeBefore, double? timeAfter)
        {
            await DeleteScheduledJobs(workId);
            var delayStartTime = startDate.Value.Hour - (timeBefore ?? 0);
            var delayEndTime = dueDate.Value.Hour - (timeAfter ?? 0);
            switch (remindWorkType)
            {
                case var _ when remindWorkType == RemindWorkType.RemindStart:

                    var startJobId = BackgroundJob.Schedule(() => HandleSendNotificationBeforeRemind(workId, $"Nhắc nhở, công việc {workName} sắp bắt đầu ", employeeId), startDate.Value.AddHours(-delayStartTime));
                    await SaveJobId(workId, startJobId, "StartReminder");
                    // gửi mail
                    await SendMailRemindWork(workId, "Thông báo công việc sắp bắt đầu", startDate.Value.AddHours(-delayStartTime));

                    break;

                case var _ when remindWorkType == RemindWorkType.RemindEnd:
                    var endJobId = BackgroundJob.Schedule(() => HandleSendNotificationAfterRemind(workId, $"Nhắc nhở, công việc {workName} sắp hết hạn ", employeeId), dueDate.Value.AddHours(-delayEndTime));
                    await SaveJobId(workId, $"EndReminder{workId}", "EndReminder");

                    // gửi mail
                    await SendMailRemindWork(workId,"Thông báo công việc sắp hết hạn", dueDate.Value.AddHours(-delayEndTime));

                    break;

                case var _ when remindWorkType == RemindWorkType.BothRemind:
                    var bothStartJobId = BackgroundJob.Schedule(() => HandleSendNotificationBeforeRemind(workId, $"Nhắc nhở, công việc {workName} sắp bắt đầu ", employeeId), startDate.Value.AddHours(-delayStartTime));
                    await SaveJobId(workId, bothStartJobId, "StartReminder");
                    //gửi mail
                    await SendMailRemindWork(workId, "Thông báo công việc sắp bắt đầu", startDate.Value.AddHours(-delayStartTime));


                    var bothEndJobId = BackgroundJob.Schedule(() => HandleSendNotificationAfterRemind(workId, $"Nhắc nhở, công việc {workName} sắp hết hạn ", employeeId), dueDate.Value.AddHours(-delayEndTime));
                    await SaveJobId(workId, bothEndJobId, "EndReminder");
                    // gửi mail 
                    await SendMailRemindWork(workId, "Thông báo công việc sắp hết hạn", dueDate.Value.AddHours(-delayEndTime));

                    break;

                default:
                    // Xử lý trường hợp không có flag nào hoặc lỗi
                    break;
            }




        }
        public async Task HandleSendNotificationBeforeRemind(int workId, string content,int employeeId)
        {
            var connectionId = await _dbContext.UserConnections.Where(uc => uc.EmployeeId == employeeId).ToListAsync();

            var notiRemindEnd = await _dbContext.RemindWorkNotifications.AddAsync(new RemindWorkNotification
            {
                NotificationType = NotificationType.RemindWork,
                RemindWorkId = workId,
                Content = content
            });
            foreach (var item in connectionId)
            {
                await _remindWorkHub.Clients.Client(item.ConnectionId).SendAsync("ReceiveRemindWorkNotification", new
                {
                    NotificationType = NotificationType.RemindWork,
                    RemindWorkId = workId,
                    Content = content
                });
            }
            

        }
        public async Task HandleSendNotificationAfterRemind(int workId, string content, int employeeId)
        {
            var connectionId = await _dbContext.UserConnections.Where(uc => uc.EmployeeId == employeeId).ToListAsync();

            var notiRemindEnd = await _dbContext.RemindWorkNotifications.AddAsync(new RemindWorkNotification
            {
                RemindWorkId = workId,
                Content = content
            });
            foreach (var item in connectionId)
            {
                await _remindWorkHub.Clients.Client(item.ConnectionId).SendAsync("ReceiveRemindWorkNotification", new
                {
                    NotificationType = NotificationType.RemindWork,
                    RemindWorkId = workId,
                    Content = content
                });
            }

        }
        private async Task SaveJobId(int workId, string jobId, string jobType)
        {
            _dbContext.ScheduleJobs.Add(new ScheduleJob
            {
                WorkId = workId,
                JobId = jobId,
                JobType = jobType
            });
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteScheduledJobs(int workId)
        {
            var jobIds = await _dbContext.ScheduleJobs
                .Where(j => j.WorkId == workId)
                .Select(j => j.JobId)
                .ToListAsync();

            foreach (var jobId in jobIds)
            {
                BackgroundJob.Delete(jobId);
            }

            // Xóa JobId khỏi bảng sau khi xóa
            _dbContext.ScheduleJobs.RemoveRange(
                _dbContext.ScheduleJobs.Where(j => j.WorkId == workId)
            );
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendMailRemindWork(int workId, string subject,DateTime delay)
        {
            var entity = await _dbContext.Works.FindAsync(workId);
            string workName = entity.Name;
            var project = await _dbContext.Projects.Where(p => p.Id == entity.ProjectId.Value).FirstOrDefaultAsync();
            string projectName = project.Name;
            var reporterEntity = await _dbContext.Employees.Where(p => p.Id == entity.ReporterId.Value).FirstOrDefaultAsync();
            string reporterName = reporterEntity.LastName + reporterEntity.FirstName;
            string workDueDate = entity.DueDate.ToString();
            string workUrl = "https://hrm.smomedia.vn/";
            // gửi mail khi có người thực hiện
            BackgroundJob.Schedule(() =>
                 _mailService.WorkSendMail(new SendMailRequest
                 {
                     Body = MailSendWorkBody.CreateBodyHTMLFormat(entity.Executor.CreatedName, reporterName, workName, workUrl, workDueDate, projectName),
                     ToEmail = entity.Executor.PersonalEmail,
                     Subject = subject
                 }),delay);
        }
    }
}
