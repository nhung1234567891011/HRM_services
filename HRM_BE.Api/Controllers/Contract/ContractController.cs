using AutoMapper;
using Hangfire;
using HRM_BE.Api.Services;
using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Data.Content;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Profile;
using HRM_BE.Core.ISeedWorks;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Content.Banner;
using HRM_BE.Core.Models.Contract;
using HRM_BE.Core.Models.Profile;
using HRM_BE.Core.Models.Profile.ContractType;
using HRM_BE.Core.Models.Staff;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Net.WebSockets;
using System.Reflection;

namespace HRM_BE.Api.Controllers.Profile
{
    [Route("api/contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobHangFireService _jobHangFireService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private const int NumberOfLeave = 1;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUserService _userService;
        public ContractController(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IJobHangFireService jobHangFireService, IRecurringJobManager recurringJobManager, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _jobHangFireService = jobHangFireService;
            _recurringJobManager = recurringJobManager;
            _userService = userService;
        }

        [HttpGet("get-all")]
        public async Task<List<ContractDTO>> GetAll([FromQuery] GetContractPagingRequest request)
        {
            var result = await _unitOfWork.Contracts.GetAll(request.NameEmployee, request.Unit, request.UnitId, request.ExpiredStatus, request.SortBy, request.OrderBy);
            return result;
        }

        [HttpGet("paging")]
        public async Task<PagingResult<ContractDTO>> Paging([FromQuery] GetContractPagingRequest request)
        {
            var result = await _unitOfWork.Contracts.Paging(request.NameEmployee,request.Unit,request.UnitId,request.ExpiredStatus,request.SortBy, request.OrderBy, request.PageIndex, request.PageSize);
            return result;
        }
        [HttpPost("create")]
        public async Task<ApiResult<ContractDTO>> Create([FromForm] CreateContractRequest request)
        {
            //var contract = _mapper.Map<CreateContractRequest, Contract>(request);

            if (request.AttachmentFile?.Length > 0)
            {
                request.Attachment = await _fileService.UploadFileAsync(request.AttachmentFile, PathFolderConstant.Contract);
            }
            else
            {
                request.Attachment = null;
            }
            request.Code = await _unitOfWork.PrefixConfigs.GetAndUpdatePrefix("contract","SMOHD");
            var contract = await _unitOfWork.Contracts.Create(request);
            // xử lý background job khi hết hạn tự động đổi status
            // Lấy ngày hiện tại
            var currentDate = DateTime.Now;

            // Lấy ngày cuối cùng của tháng hợp đồng hết hạn
            var lastDayOfMonth = new DateTime(contract.ExpiryDate.Value.Year, contract.ExpiryDate.Value.Month, DateTime.DaysInMonth(contract.ExpiryDate.Value.Year, contract.ExpiryDate.Value.Month));

            // Tính khoảng thời gian từ hiện tại đến ngày cuối cùng của tháng
            //var expireDelayContract = lastDayOfMonth.Date.AddDays(1).AddTicks(-1) - currentDate;
            //BackgroundJob.Schedule<JobHangFireService>(p => p.UpdateExpireContractStatus(contract.Id.Value),expireDelayContract);

            // ################## Tạo số số ngày nghỉ cho nhân viên #####################################################################
            //if (contract.ContractTypeStatus == ContractTypeStatus.Official)
            //{
            //    var user = await _userService.GetUserInfoAsync();
            //    var rootOrganizationId = _unitOfWork.Organizations.GetRootOrganizationId(user.Organization.Id);

            //    var regularLeave = await _unitOfWork.GeneralLeaveRegulation.GetById(rootOrganizationId); // lấy ra quy định nghỉ

            //    var startDay = contract.EffectiveDate.Value.AddMonths(regularLeave.SeniorityMonths);
            //    var endDay = contract.ExpiryDate.Value;
            //    var delay = startDay - DateTime.Now;

            //    // Lên lịch công việc bắt đầu sau khoảng thời gian từ bây giờ đến startDay
            //    //BackgroundJob.Schedule(() => ScheduleRecurringJob(contract.EmployeeId.Value, contract.Id.Value, regularLeave.NumberOfDaysOff.Value, startDay, endDay, regularLeave.AdmissionDay.Value), delay);
            //    int numberDecrease = 1;
            //    if (regularLeave.MonthlyLeaveAccrual == MonthlyLeaveAccrual.AccrueLeaveMonthly)
            //    {
            //        numberDecrease = 1;
            //    }
            //    BackgroundJob.Schedule<JobHangFireService>( p => p.ScheduleRecurringJob(contract.EmployeeId.Value, contract.Id.Value, numberDecrease, startDay, endDay, regularLeave.AdmissionDay.Value),delay);
            //}

            return ApiResult<ContractDTO>.Success("Thêm hợp đồng thành công", contract);
        }


        [HttpGet("get-by-id")]
        public async Task<ContractDTO> GetById([FromQuery] EntityIdentityRequest<int> request)
        {
            var contract = await _unitOfWork.Contracts.GetById(request.Id);
            //var expireDelayContract = contract.ExpiryDate.Value - DateTime.Now;
            // BackgroundJob.Schedule<JobHangFireService>(p => p.UpdateExpireContractStatus(contract.Id.Value), expireDelayContract);
            //if (contract.ContractTypeStatus == ContractTypeStatus.Official)
            //{
            //    var user = await _userService.GetUserInfoAsync();
            //    var regularLeave = await _unitOfWork.GeneralLeaveRegulation.GetById(user.Organization.Id);    // lấy ra quy định nghỉ

            //    var startDay = contract.EffectiveDate.Value.AddMonths(regularLeave.SeniorityMonths);
            //    var endDay = contract.ExpiryDate.Value;
            //    var delay = startDay - DateTime.Now;

            //    // Lên lịch công việc bắt đầu sau khoảng thời gian từ bây giờ đến startDay
            //    //BackgroundJob.Schedule(() => ScheduleRecurringJob(contract.EmployeeId.Value, contract.Id.Value, regularLeave.NumberOfDaysOff.Value, startDay, endDay, regularLeave.AdmissionDay.Value), delay);
            //    int numberDecrease = 1;
            //    if (regularLeave.MonthlyLeaveAccrual == MonthlyLeaveAccrual.AccrueLeaveMonthly)
            //    {
            //        numberDecrease = 1;
            //    }
            //    BackgroundJob.Schedule<JobHangFireService>(p => p.ScheduleRecurringJob(contract.EmployeeId.Value, contract.Id.Value, numberDecrease, startDay, endDay, regularLeave.AdmissionDay.Value), delay);
            //}

            return contract;
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, [FromForm]  UpdateContractRequest request)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
            {
                throw new BadHttpRequestException("Không tìm thấy hợp đồng cần cập nhật.");
            }

            if (contract.ExpiredStatus == true)
            {
                throw new BadHttpRequestException("Hợp đồng đã chấm dứt/hết hiệu lực, không được chỉnh sửa thông tin chính.");
            }

            if (request.AttachmentFile?.Length > 0)
            {
                request.Attachment = await _fileService.UploadFileAsync(request.AttachmentFile, PathFolderConstant.Contract);
                if(request.Attachment!=null && request.Attachment!="")
                {
                    await _fileService.DeleteFileAsync(contract.Attachment);

                }
            }
            else
            {
                request.Attachment = contract.Attachment;
            }
            await _unitOfWork.Contracts.Update(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật hợp đồng thành công", true));
        }
        [HttpPut("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.Contracts.Delete(id);
            return Ok(ApiResult<bool>.Success("Xoá hợp đồng thành công",true));
        }
        [HttpPut("delete-range")]
        public async Task<IActionResult> DeleteRange(ListEntityIdentityRequest<int> request)
        {
            await _unitOfWork.Contracts.DeleteRange(request);
            return Ok(ApiResult<bool>.Success("Xoá nhiều hợp đồng thành công", true));
        }
        [HttpPut("update-expired-status")]
        public async Task<IActionResult> UpdateExpiredStatus(int id, [FromBody] UpdateContractExpiredStatusRequest request)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
            {
                throw new BadHttpRequestException("Không tìm thấy hợp đồng cần chấm dứt.");
            }

            if (contract.ExpiredStatus == true)
            {
                throw new BadHttpRequestException("Hợp đồng đã ở trạng thái hết hiệu lực, không thể chấm dứt lại.");
            }

            await _unitOfWork.Contracts.UpdateExpiredStatus(id, request);
            return Ok(ApiResult<bool>.Success("Cập nhật trạng thái hợp đồng thành công", true));
        }

    }
}
