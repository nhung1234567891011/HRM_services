using AutoMapper;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Data.Company;
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Payroll_Timekeeping.TimekeepingRegulation;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.DetailTimeSheet;
using HRM_BE.Core.Models.Organization;
using HRM_BE.Core.Models.ShiftWork;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Core.Models.SumaryTimeSheet;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class SummaryTimeSheetRepository : RepositoryBase<SummaryTimesheetName, int>, ISummaryTimeSheetRepository
    {
        private readonly IMapper _mapper;
        public SummaryTimeSheetRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

            _mapper = mapper;
        }

        public async Task<SummaryTimeSheetDto> Create(CreateSummaryTimesheetRequest request)
        {
            var entity = _mapper.Map<SummaryTimesheetName>(request);
            var entityReturn = await CreateAsync(entity);
            return _mapper.Map<SummaryTimeSheetDto>(entityReturn);
        }

        public async Task Delete(int id)
        {
            var entity = await GetSummaryTimeSheetAndCheckExist(id);
            entity.IsDeleted = false;
            await UpdateAsync(entity);
        }

        public async Task<SummaryTimeSheetDto> GetById(int id)
        {
            var entity = await GetSummaryTimeSheetAndCheckExist(id);
            return _mapper.Map<SummaryTimeSheetDto>(entity);
        }

        public async Task<PagingResult<GetSummaryTimeSheetWithEmployeeDto>> GetSummaryTimeSheetPaging(int summaryTimeSheetId,int organizationId, string? keyWord, int? staffPositionId, string? sortBy, string? orderBy, int pageIndex, int pageSize)
        {
            var periodTime = await _dbContext.SummaryTimesheetNames.Where(s => s.Id == summaryTimeSheetId).Select(d => new
            {
                minStartDate = d.SummaryTimesheetNameDetailTimesheetNames.Min(s => s.DetailTimesheetName.StartDate),
                maxEndDate = d.SummaryTimesheetNameDetailTimesheetNames.Max(s => s.DetailTimesheetName.EndDate)
            }).FirstOrDefaultAsync();
            if (periodTime is null)
            {
                throw new EntityNotFoundException($"Bảng chấm công tổng hợp Id = {summaryTimeSheetId} chưa bao gồm bảng chấm công chi tiết nào");
            }
            
            var organizationDescendantIds = await GetAllChildOrganizationIds(organizationId);
            organizationDescendantIds.Add(organizationId);
            var query = _dbContext.Employees.Where( e => organizationDescendantIds.Contains(e.OrganizationId.Value) && e.AccountStatus == AccountStatus.Active).AsNoTracking();
            //query = query.Where(e => e.Timesheets.Any(t => t.Date >= periodTime.minStartDate && t.Date <= periodTime.maxEndDate));
            if (!string.IsNullOrEmpty(keyWord))
            {
                keyWord = keyWord.Trim(); // Loại bỏ khoảng trắng thừa đầu và cuối từ khóa
                query = query.Where(c => (c.LastName.Trim() + " " + c.FirstName.Trim()).Contains(keyWord) ||
                                         (c.FirstName.Trim() + " " + c.LastName.Trim()).Contains(keyWord));
            }
            if (staffPositionId.HasValue)
            {
                query = query.Where(c => c.StaffPositionId == staffPositionId);
            }
            query = query.Where(e => e.Contracts.Count(c => c.ExpiredStatus == false) == 1);

            //var totalWorkingDay = await query..CountAsync

            var data = await query.Select(e => new GetSummaryTimeSheetWithEmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                // Timesheets trong kỳ (lọc IsDeleted và Date)
                TotalHour = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date)
                    .Sum(t => t.NumberOfWorkingHour ?? 0),
                TotalWorkingDay = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None
                                && (
                                       (t.NumberOfWorkingHour ?? 0) > 0
                                       // Trường hợp quên chấm ra: vẫn coi là có đi làm nếu có check-in (StartTime) trong ngày
                                       || (t.NumberOfWorkingHour == null && t.StartTime.HasValue)
                                   ))
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .Count(),
                TotalLeaveDay = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.TimeKeepingLeaveStatus != TimeKeepingLeaveStatus.None)
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .Count(),
                TotalOtHour = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.ShiftWork.ShiftCatalog.AllowOvertime == true
                                && t.NumberOfWorkingHour.HasValue
                                && t.ShiftWork.ShiftCatalog.WorkingHours.HasValue
                                && t.NumberOfWorkingHour > t.ShiftWork.ShiftCatalog.WorkingHours)
                    .Sum(t => (t.NumberOfWorkingHour ?? 0) - (t.ShiftWork.ShiftCatalog.WorkingHours ?? 0)),
                DatePerMonth = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.NumberOfWorkingHour.HasValue
                                )
                    .Select(s => s.Date!.Value.Day)
                    .Distinct()
                    .Count(),
                // Nếu tính ngày công quy đổi: dùng giờ tiêu chuẩn (tối đa 8h/ngày) / 8, KHÔNG dùng TotalHour/8 (TotalHour gồm cả tăng ca; trùng với quy tắc PayrollDetail).
                EqualDay = 0,
                TotalExistLeaveDay = 0, // để mở rộng nếu cần tính số ngày phép tồn
                Status = e.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.SummaryTimesheetNameId == summaryTimeSheetId)
                    .Select(s => s.Status ?? SummaryTimesheetNameEmployeeConfirmStatus.None)
                    .FirstOrDefault(),
                ConfirmDate = e.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.SummaryTimesheetNameId == summaryTimeSheetId)
                    .Select(s => s.Date)
                    .FirstOrDefault(),
                StaffPosition = new GetDetailTimeSheetStaffPositionDto
                {
                    Id = e.StaffPosition.Id,
                    PositionName = e.StaffPosition.PositionName,
                    PositionCode = e.StaffPosition.PositionCode
                },
                Organization = new GetOrganizationForEmployeeDto
                {
                    Id = e.Organization.Id,
                    OrganizationName = e.Organization.OrganizationName
                },
                StartDate = periodTime.minStartDate.Value.Date,
                EndDate = periodTime.maxEndDate.Value.Date

            }).ToListAsync();
            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            //var employees = await _mapper.ProjectTo<GetSummaryTimeSheetWithEmployeeDto>(query).ToListAsync();

            var result = new PagingResult<GetSummaryTimeSheetWithEmployeeDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task<List<GetSummaryTimeSheetWithEmployeeDto>> GetSummaryTimeSheetWithEmployeeList(int summaryTimeSheetId, int organizationId, string? keyWord, int? staffPositionId, string? sortBy, string? orderBy)
        {
            var periodTime = await _dbContext.SummaryTimesheetNames
                .Where(s => s.Id == summaryTimeSheetId)
                .Select(d => new
                {
                    minStartDate = d.SummaryTimesheetNameDetailTimesheetNames.Min(s => s.DetailTimesheetName.StartDate),
                    maxEndDate = d.SummaryTimesheetNameDetailTimesheetNames.Max(s => s.DetailTimesheetName.EndDate)
                })
                .FirstOrDefaultAsync();

            if (periodTime is null)
            {
                throw new EntityNotFoundException($"Bảng chấm công tổng hợp Id = {summaryTimeSheetId} chưa bao gồm bảng chấm công chi tiết nào");
            }

            var organizationDescendantIds = await GetAllChildOrganizationIds(organizationId);
            organizationDescendantIds.Add(organizationId);

            var query = _dbContext.Employees
                .Where(e => organizationDescendantIds.Contains(e.OrganizationId.Value) && e.AccountStatus == AccountStatus.Active)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(keyWord))
            {
                keyWord = keyWord.Trim();
                query = query.Where(c => (c.LastName.Trim() + " " + c.FirstName.Trim()).Contains(keyWord) ||
                                         (c.FirstName.Trim() + " " + c.LastName.Trim()).Contains(keyWord));
            }

            if (staffPositionId.HasValue)
            {
                query = query.Where(c => c.StaffPositionId == staffPositionId);
            }

            query = query.Where(e => e.Contracts.Count(c => c.ExpiredStatus == false) == 1);

            // Giữ nguyên cách gọi ApplySorting như các repo khác trong dự án
            query = query.ApplySorting(sortBy, orderBy);

            var data = await query.Select(e => new GetSummaryTimeSheetWithEmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                TotalHour = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date)
                    .Sum(t => t.NumberOfWorkingHour ?? 0),
                TotalWorkingDay = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.TimeKeepingLeaveStatus == TimeKeepingLeaveStatus.None
                                && (
                                       (t.NumberOfWorkingHour ?? 0) > 0
                                       || (t.NumberOfWorkingHour == null && t.StartTime.HasValue)
                                   ))
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .Count(),
                TotalLeaveDay = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.TimeKeepingLeaveStatus != TimeKeepingLeaveStatus.None)
                    .Select(t => t.Date!.Value.Date)
                    .Distinct()
                    .Count(),
                TotalOtHour = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date
                                && t.ShiftWork.ShiftCatalog.AllowOvertime == true
                                && t.NumberOfWorkingHour.HasValue
                                && t.ShiftWork.ShiftCatalog.WorkingHours.HasValue
                                && t.NumberOfWorkingHour > t.ShiftWork.ShiftCatalog.WorkingHours)
                    .Sum(t => (t.NumberOfWorkingHour ?? 0) - (t.ShiftWork.ShiftCatalog.WorkingHours ?? 0)),
                DatePerMonth = e.Timesheets
                    .Where(t => t.IsDeleted != true
                                && t.Date.HasValue
                                && t.Date.Value.Date >= periodTime.minStartDate.Value.Date
                                && t.Date.Value.Date <= periodTime.maxEndDate.Value.Date)
                    .Select(s => s.Date!.Value.Day)
                    .Distinct()
                    .Count(),
                // EqualDay: nếu dùng sau này thì tính = tổng min(giờ/ngày, 8) / 8, không dùng TotalHour/8 (trùng quy tắc PayrollDetail).
                EqualDay = 0,
                TotalExistLeaveDay = 0,
                Status = e.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.SummaryTimesheetNameId == summaryTimeSheetId)
                    .Select(s => s.Status ?? SummaryTimesheetNameEmployeeConfirmStatus.None)
                    .FirstOrDefault(),
                ConfirmDate = e.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.SummaryTimesheetNameId == summaryTimeSheetId)
                    .Select(s => s.Date)
                    .FirstOrDefault(),
                StaffPosition = new GetDetailTimeSheetStaffPositionDto
                {
                    Id = e.StaffPosition.Id,
                    PositionName = e.StaffPosition.PositionName,
                    PositionCode = e.StaffPosition.PositionCode
                },
                Organization = new GetOrganizationForEmployeeDto
                {
                    Id = e.Organization.Id,
                    OrganizationName = e.Organization.OrganizationName
                },
                StartDate = periodTime.minStartDate.Value.Date,
                EndDate = periodTime.maxEndDate.Value.Date
            }).ToListAsync();

            return data;
        }

        public async Task<List<int>> GetAllChildOrganizationIds(int parentId)
        {
            // Lấy tất cả các tổ chức
            var allOrganizations = await _dbContext.Organizations.AsNoTracking().ToListAsync();

            // Gọi hàm đệ quy để tìm tất cả các Id con
            var result = new List<int>();
            GetChildIdsRecursive(parentId, allOrganizations, result);
            return result;
        }

        private void GetChildIdsRecursive(int parentId, List<Organization> allOrganizations, List<int> result)
        {
            // Lấy tất cả các con trực tiếp của parentId
            var children = allOrganizations.Where(o => o.OrganizatioParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child.Id); // Thêm Id của con vào danh sách kết quả
                GetChildIdsRecursive(child.Id, allOrganizations, result); // Gọi đệ quy cho các con
            }
        }
        public async Task<PagingResult<SummaryTimeSheetDto>> Paging(int? summaryTimesheetId, string? name, int? month, int? year, int? organizationId, string? sortBy, string? orderBy, int pageIndex, int pageSize)
        {
            var query = _dbContext.SummaryTimesheetNames
                .Include(s => s.SummaryTimesheetNameDetailTimesheetNames)
                    .ThenInclude(s => s.SummaryTimesheetName).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.TimekeepingSheetName.Contains(name));
            }
            if (month.HasValue && year.HasValue)
            {
                query = query.Where(s => s.SummaryTimesheetNameDetailTimesheetNames
                                    .Any(d => d.DetailTimesheetName.EndDate.Value.Month <= month && d.DetailTimesheetName.StartDate.Value.Month >= month && d.DetailTimesheetName.EndDate.Value.Year <= year && d.DetailTimesheetName.StartDate.Value.Year >= year));
            }
            if (organizationId.HasValue)
            {
                query = query.Where(c => c.OrganizationId == organizationId);
            }
            if (summaryTimesheetId.HasValue)
            {
                query = query.Where(s => s.Id == summaryTimesheetId);
            }
            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<SummaryTimeSheetDto>(query).ToListAsync();

            foreach (var item in data)
            {
                //var summaryConfirmed = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                //    .Where(s => s.SummaryTimesheetNameId == item.Id)
                //    .Select(e => e.SummaryTimesheetNameId).ToListAsync();

                var employeeConfirmedCount = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Confirm && s.SummaryTimesheetNameId == item.Id)
                    .Select(e => e.EmployeeId).CountAsync();

                //var periodTime = await _dbContext.SummaryTimesheetNames.Where(s => s.Id == item.Id)
                //    .Select(d => new
                //    {
                //        minStartDate = d.SummaryTimesheetNameDetailTimesheetNames.Min(s => s.DetailTimesheetName.StartDate),
                //        maxEndDate = d.SummaryTimesheetNameDetailTimesheetNames.Max(s => s.DetailTimesheetName.EndDate),
                //    }).FirstOrDefaultAsync();
                var organizationDescendantIds = await GetAllChildOrganizationIds(item.OrganizationId.Value);
                organizationDescendantIds.Add(item.OrganizationId.Value);

                var employeeCountItems = await _dbContext.Employees.Where( e => organizationDescendantIds.Contains(item.OrganizationId.Value) )
                    .Select( e => e.Id).CountAsync();
                
                bool isConfirmed = employeeConfirmedCount == employeeCountItems;

                bool isPending = employeeConfirmedCount > 0;

                // đã gửi tất cả nhưng chưa ai xác nhận
                bool notConfirmed = await _dbContext.SummaryTimesheetNameEmployeeConfirms
                    .Where(s => s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Confirm && s.SummaryTimesheetNameId == item.Id && s.Status == SummaryTimesheetNameEmployeeConfirmStatus.Pending)
                    .Select(e => e.EmployeeId).CountAsync() == employeeCountItems;

                if (isPending && isConfirmed == true)
                {
                    item.Status = SummaryTimesheetNameEmployeeConfirmStatus.Confirm;

                }
                else if ( isPending && notConfirmed == false )
                {
                    item.Status = SummaryTimesheetNameEmployeeConfirmStatus.Pending;
                }
                else if (notConfirmed)
                {
                    item.Status = SummaryTimesheetNameEmployeeConfirmStatus.SendedNotConfirm;
                }
                else
                {
                    item.Status = SummaryTimesheetNameEmployeeConfirmStatus.None;
                }
            }
            var result = new PagingResult<SummaryTimeSheetDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }

        public async Task Update(int id, UpdateSummaryTimeSheetRequest request)
        {
            var summaryTimesheet = await GetSummaryTimeSheetAndCheckExist(id);

            _mapper.Map(request, summaryTimesheet);
            // Xóa các StaffPosition cũ
            summaryTimesheet.SummaryTimesheetNameStaffPositions.Clear();
            summaryTimesheet.SummaryTimesheetNameDetailTimesheetNames.Clear();

            // Thêm danh sách StaffPosition mới từ request
            foreach (var staffPositionRequest in request.SummaryTimesheetNameStaffPositions)
            {
                var staffPosition = _dbContext.StaffPositions.Find(staffPositionRequest.StaffPositionId);
                if (staffPosition != null)
                {
                    var newStaffPosition = _mapper.Map<SummaryTimesheetNameStaffPosition>(staffPositionRequest);
                    newStaffPosition.SummaryTimesheetNameId = summaryTimesheet.Id;
                    summaryTimesheet.SummaryTimesheetNameStaffPositions.Add(newStaffPosition);
                }
            }
            foreach (var detailTimeSheetRequest in request.SummaryTimesheetNameDetailTimesheetNames)
            {
                var staffPosition = _dbContext.DetailTimesheetNames.Find(detailTimeSheetRequest.DetailTimesheetNameId);
                if (staffPosition != null)
                {
                    var newDetailTimeSheet = _mapper.Map<SummaryTimesheetNameDetailTimesheetName>(detailTimeSheetRequest);
                    newDetailTimeSheet.SummaryTimesheetNameId = summaryTimesheet.Id;
                    summaryTimesheet.SummaryTimesheetNameDetailTimesheetNames.Add(newDetailTimeSheet);
                }
            }
            // Lưu các thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();
        }
        private async Task<SummaryTimesheetName> GetSummaryTimeSheetAndCheckExist(int summaryTimeSheetId)
        {
            var summaryTimeSheet = await _dbContext.SummaryTimesheetNames
                .Include(s => s.Organization)
                .Include(s => s.SummaryTimesheetNameStaffPositions.Where(sp => sp.SummaryTimesheetNameId == summaryTimeSheetId)).ThenInclude(s => s.StaffPosition)
                .Include(s => s.SummaryTimesheetNameDetailTimesheetNames).ThenInclude(s => s.DetailTimesheetName).ThenInclude(s => s.DetailTimesheetNameStaffPositions)
                .SingleOrDefaultAsync(s => s.Id == summaryTimeSheetId);
            if (summaryTimeSheet is null)
                throw new EntityNotFoundException(nameof(SummaryTimesheetName), $"Id = {summaryTimeSheetId}");
            return summaryTimeSheet;
        }

        public async Task<List<GetSelectSummaryTimeSheetDto>> GetSelectSummaryTimeSheet()
        {

            var organizationId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypeConstant.OrganizationId).Value);
            
            var query = _dbContext.SummaryTimesheetNames.Where(s => s.OrganizationId == organizationId).AsNoTracking()
                .Include(s => s.SummaryTimesheetNameDetailTimesheetNames);

            var data = await query.Select(s => new GetSelectSummaryTimeSheetDto
            {
                Id = s.Id,
                Name = s.TimekeepingSheetName,
                StartDate = new DateTime(),
                EndDate = new DateTime(),
                TimekeepingMethod = s.TimekeepingMethod.HasValue
                    ? (s.TimekeepingMethod.Value == TimekeepingMethod.Hour ? "Giờ" : "Ngày")
                    : null
            }).ToListAsync();

            foreach ( var item in data)
            {
                var periodTime = await _dbContext.SummaryTimesheetNames.Where(s => s.Id == item.Id).Select(d => new
                {
                    minStartDate = d.SummaryTimesheetNameDetailTimesheetNames.Min(s => s.DetailTimesheetName.StartDate),
                    maxEndDate = d.SummaryTimesheetNameDetailTimesheetNames.Max(s => s.DetailTimesheetName.EndDate)
                }).FirstOrDefaultAsync();
                if (periodTime.minStartDate is null && periodTime.maxEndDate is null)
                {
                    throw new EntityNotFoundException($"Bảng chấm công tổng hợp Id = {item.Id} chưa bao gồm bảng chấm công chi tiết nào");
                }
                item.StartDate = periodTime.minStartDate.Value;
                item.EndDate = periodTime.maxEndDate.Value;
            }
            return data;

        }

        public async Task<List<GetSelectSummaryTimeSheetDto>> GetSelectSummaryTimeSheetForPayroll(int? organizationId, string? staffPositionIds)
        {
            // Mặc định lấy theo OrganizationId trong token (giống GetSelectSummaryTimeSheet hiện tại)
            var tokenOrganizationId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypeConstant.OrganizationId).Value);
            var resolvedOrganizationId = organizationId ?? tokenOrganizationId;

            List<int>? staffPositionIdList = null;
            if (!string.IsNullOrWhiteSpace(staffPositionIds))
            {
                staffPositionIdList = staffPositionIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => int.TryParse(s, out var id) ? (int?)id : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .ToList();

                if (staffPositionIdList.Count == 0)
                {
                    staffPositionIdList = null;
                }
            }

            // Chỉ lấy những bảng công tổng hợp đã có ít nhất 1 bảng công chi tiết có đủ Start/End
            var query = _dbContext.SummaryTimesheetNames
                .AsNoTracking()
                .Where(s => s.OrganizationId == resolvedOrganizationId)
                .Where(s => s.SummaryTimesheetNameDetailTimesheetNames.Any(d =>
                    d.DetailTimesheetName != null &&
                    d.DetailTimesheetName.StartDate.HasValue &&
                    d.DetailTimesheetName.EndDate.HasValue));

            // Lọc theo vị trí (nếu có)
            if (staffPositionIdList != null)
            {
                query = query.Where(s => s.SummaryTimesheetNameStaffPositions.Any(sp =>
                    sp.StaffPositionId.HasValue && staffPositionIdList.Contains(sp.StaffPositionId.Value)));
            }

            // Tính period (min/max) trong 1 query, tránh N+1 và tránh throw khi thiếu dữ liệu
            var data = await query
                .Select(s => new GetSelectSummaryTimeSheetDto
                {
                    Id = s.Id,
                    Name = s.TimekeepingSheetName,
                    StartDate = s.SummaryTimesheetNameDetailTimesheetNames
                        .Where(d => d.DetailTimesheetName.StartDate.HasValue)
                        .Min(d => d.DetailTimesheetName.StartDate!.Value),
                    EndDate = s.SummaryTimesheetNameDetailTimesheetNames
                        .Where(d => d.DetailTimesheetName.EndDate.HasValue)
                        .Max(d => d.DetailTimesheetName.EndDate!.Value),
                    TimekeepingMethod = s.TimekeepingMethod.HasValue
                        ? (s.TimekeepingMethod.Value == TimekeepingMethod.Hour ? "Giờ" : "Ngày")
                        : null
                })
                .OrderByDescending(x => x.EndDate)
                .ToListAsync();

            return data;
        }
    }
}
