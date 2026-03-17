using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Shift;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Salary.KpiTable;
using HRM_BE.Core.Models.Staff;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    //public  class KpiTableRepository
    //{
    //}
    public class KpiTableRepository : RepositoryBase<KpiTable, int>, IKpiTableRepository
    {
        private readonly IMapper _mapper;
        public KpiTableRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<KpiTableDto> GetById(int KpiTableId)
        {
            // Tìm kiếm KpiTable và bao gồm các chi tiết liên quan
            var KpiTable = await _dbContext.KpiTables
                .Include(s => s.Organization) // Bao gồm tổ chức liên quan
                .Include( s => s.KpiTablePositions).ThenInclude(x=> x.StaffPosition)
                .Include(s => s.KpiTableDetails) // Bao gồm chi tiết KPI
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == KpiTableId); // Tìm kiếm theo ID

            // Kiểm tra nếu không tìm thấy
            if (KpiTable == null)
            {
                throw new KeyNotFoundException($"KPI Table with ID {KpiTableId} not found.");
            }

            // Ánh xạ sang DTO
            return _mapper.Map<KpiTableDto>(KpiTable);
        }
        public async Task<PagingResult<KpiTableDto>> Paging(string? name, int? organizationId, string? sortBy, string? orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.KpiTables.Include(s => s.Organization).AsNoTracking();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.NameKpiTable.Contains(name));
            }
            if (organizationId.HasValue)
            {
                query = query.Where(c => c.OrganizationId == organizationId);
            }
            
            // Áp dụng sắp xếp
            query = query.ApplySorting(sortBy, orderBy);
            // Tính tổng số bản ghi
            int total = await query.CountAsync();
            // Áp dụng phân trang
            query = query.ApplyPaging(pageIndex, pageSize);

            var data = await _mapper.ProjectTo<KpiTableDto>(query).ToListAsync();
            var result = new PagingResult<KpiTableDto>(data, pageIndex, pageSize, sortBy, orderBy, total);

            return result;
        }
        public async Task<KpiTableDto> Create(CreateKpiTableRequest request)
        {
            // Kiểm tra trùng lặp KpiTable trước khi thêm mới
            var existingKpiTable = await _dbContext.KpiTables
                .FirstOrDefaultAsync(kt => kt.NameKpiTable == request.NameKpiTable );
            if (existingKpiTable != null)
                throw new EntityAlreadyExistsException($"Tên bảng {request.NameKpiTable} đã tồn tại");

            // Bước 1: Tạo KpiTable từ request và lưu vào database
            var kpiTable = _mapper.Map<KpiTable>(request);
            await _dbContext.KpiTables.AddAsync(kpiTable);
            await _dbContext.SaveChangesAsync();

            // Bước 2: Xử lý KpiTablePositions nếu có
            if (request.KpiTablePositions?.Any() == true)
            {
                var newKpiTablePositions = request.KpiTablePositions
                    .Where(item => !_dbContext.KpiTablePositions.Any(op => op.KpiTableId == kpiTable.Id && op.StaffPositionId == item.StaffPositionId))
                    .Select(item => new KpiTablePosition
                    {
                        KpiTableId = kpiTable.Id,
                        StaffPositionId = item.StaffPositionId
                    })
                    .ToList();

                if (newKpiTablePositions.Any())
                {
                    await _dbContext.KpiTablePositions.AddRangeAsync(newKpiTablePositions);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Bước 3: Truy vấn danh sách nhân viên
            var employees = await _dbContext.Employees
                .Where(e => e.OrganizationId == request.OrganizationId)
                .ToListAsync();

            // Bước 4: Tạo danh sách KpiTableDetail cho từng nhân viên
            var kpiTableDetails = employees.Select(employee => new KpiTableDetail
            {
                KpiTableId = kpiTable.Id,
                EmployeeId = employee.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}" // Sửa lỗi ghép tên
                                                                           // Thêm các thông tin cần thiết khác tại đây
            }).ToList();

            // Thêm và lưu KpiTableDetails vào database
            await _dbContext.KpiTableDetails.AddRangeAsync(kpiTableDetails);
            await _dbContext.SaveChangesAsync();

            // Bước 5: Trả về KpiTableDto sau khi hoàn thành
            return _mapper.Map<KpiTableDto>(kpiTable);
        }


        private async Task<KpiTable> GetKpiTableAndCheckExist(int KpiTableId)
        {
            var KpiTable = await _dbContext.KpiTables.Include(s => s.Organization).SingleOrDefaultAsync(s => s.Id == KpiTableId);
            if (KpiTable is null)
                throw new EntityNotFoundException(nameof(KpiTable), $"Id = {KpiTableId}");
            return KpiTable;
        }


        //public async Task<StaffPositionDto> Create(CreateStaffPositionRequest request)
        //{
        //    await CheckStaffPositionExist(request.PositionCode);
        //    var enitty = _mapper.Map<StaffPosition>(request);
        //    var staffPosition = await CreateAsync(enitty);

        //    var organizationPositions = new List<OrganizationPosition>();

        //    foreach (var item in request.OrganizationPositions)
        //    {
        //        // Kiểm tra nếu OrganizationPosition đã tồn tại
        //        var existingOrganizationPosition = await _dbContext.OrganizationPositions
        //            .FirstOrDefaultAsync(op => op.StaffPositionId == staffPosition.Id && op.OrganizationId == item.OrganizationId);

        //        if (existingOrganizationPosition == null)
        //        {
        //            // Nếu chưa tồn tại, thêm vào danh sách
        //            organizationPositions.Add(new OrganizationPosition
        //            {
        //                StaffPositionId = staffPosition.Id,
        //                OrganizationId = item.OrganizationId
        //            });
        //        }
        //    }
        //    // Nếu có OrganizationPositions cần thêm, thêm tất cả một lần
        //    if (organizationPositions.Any())
        //    {
        //        await _dbContext.OrganizationPositions.AddRangeAsync(organizationPositions);
        //        await _dbContext.SaveChangesAsync();  // Lưu tất cả một lần
        //    }

        //    var entityReturn = _mapper.Map<StaffPositionDto>(enitty);
        //    return entityReturn;
        //}


        public async Task Delete(int KpiTableId)
        {
            var KpiTable = await GetKpiTableAndCheckExist(KpiTableId);
            KpiTable.IsDeleted = true;
            await UpdateAsync(KpiTable);

        }

        public async Task HardDelete(int KpiTableId)
        {
            var kpiTable = await _dbContext.KpiTables
                .Where(x => x.Id == KpiTableId)
                .Include(x => x.KpiTablePositions)
                .Include(x => x.KpiTableDetails)
                .FirstOrDefaultAsync();

            if (kpiTable == null)
            {
                throw new EntityNotFoundException(nameof(KpiTable), $"Id = {KpiTableId}");
            }

            if (kpiTable.KpiTablePositions?.Any() == true)
            {
                _dbContext.KpiTablePositions.RemoveRange(kpiTable.KpiTablePositions);
            }

            if (kpiTable.KpiTableDetails?.Any() == true)
            {
                _dbContext.KpiTableDetails.RemoveRange(kpiTable.KpiTableDetails);
            }

            _dbContext.KpiTables.Remove(kpiTable);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<KpiTableDto> GetByShiftWorkId(int shiftWorkId)
        {
            var kpiTable = await _dbContext.ShiftWorks.FirstOrDefaultAsync(sw => sw.Id == shiftWorkId);
            if (kpiTable is null)
                throw new EntityNotFoundException(nameof(KpiTable), $"ShiftWorkId = {shiftWorkId}");
            return _mapper.Map<KpiTableDto>(kpiTable);
        }

        //public async Task Update(int KpiTableId, UpdateKpiTableRequest request)
        //{
        //    var KpiTable = await GetKpiTableAndCheckExist(KpiTableId);
        //    await UpdateAsync(_mapper.Map(request, KpiTable));
        //}

        //public async Task Update(int id, UpdateKpiTableRequest request)
        //{
        //    var entity = await GetKpiTableAndCheckExist(id);

        //    // Lấy tất cả các KpiTablePosition hiện tại của StaffPosition
        //    var currentKpiTablePosition = await _dbContext.KpiTablePositions
        //        .Where(x => x.StaffPositionId == entity.Id)
        //        .ToListAsync();


        //    // Tạo danh sách các OrganizationPositions mới từ yêu cầu
        //    var newKpiTablePositions = request.KpiTablePositions;

        //    // Tạo một danh sách các OrganizationPositions cần xóa (các vị trí đã bị xóa trong request)
        //    var positionsToRemove = currentKpiTablePosition
        //        .Where(existing => !newKpiTablePositions.Any(item => item.StaffPositionId == existing.StaffPositionId))
        //        .ToList();

        //    // Tạo một danh sách các OrganizationPositions cần thêm (các vị trí mới trong request)
        //    var positionsToAdd = newKpiTablePositions
        //        .Where(newItem => !currentKpiTablePosition.Any(existing => existing.StaffPositionId == newItem.StaffPositionId))
        //        .ToList();

        //    // Cập nhật các OrganizationPosition đã tồn tại (các vị trí không thay đổi, chỉ cần đảm bảo StaffPositionId đúng)
        //    foreach (var existingPosition in currentKpiTablePosition)
        //    {
        //        var newPosition = newKpiTablePositions.FirstOrDefault(item => item.StaffPositionId == existingPosition.StaffPositionId);
        //        if (newPosition != null)
        //        {
        //            // Cập nhật các thuộc tính nếu cần, ví dụ: thay đổi OrganizationId hoặc các thuộc tính khác
        //            existingPosition.StaffPositionId = entity.Id;  // Cập nhật lại StaffPositionId nếu cần
        //                                                           // (Ví dụ: Cập nhật các thuộc tính khác nếu có)
        //        }
        //    }

        //    // Xóa các OrganizationPosition không còn trong request
        //    _dbContext.KpiTablePositions.RemoveRange(positionsToRemove);

        //    // Thêm các OrganizationPosition mới
        //    foreach (var item in positionsToAdd)
        //    {
        //        var newKpiTablePosition = new KpiTablePosition
        //        {
        //            KpiTableId = entity.Id,
        //            StaffPositionId = item.StaffPositionId
        //        };
        //        await _dbContext.KpiTablePositions.AddAsync(newKpiTablePosition);
        //    }
        //    // Cập nhật StaffPosition
        //    _mapper.Map(request, entity);

        //    // Lưu các thay đổi vào cơ sở dữ liệu
        //    await _dbContext.SaveChangesAsync();

        //}

        public async Task Update(int id, UpdateKpiTableRequest request)
        {

            var existingKpiTable = await _dbContext.KpiTables
             .FirstOrDefaultAsync(kt => kt.NameKpiTable == request.NameKpiTable);
            if (existingKpiTable != null)
                throw new EntityAlreadyExistsException($"Tên bảng {request.NameKpiTable} đã tồn tại");
            // Kiểm tra và lấy thông tin thực thể KpiTable
            var entity = await GetKpiTableAndCheckExist(id);

            // Lấy danh sách KpiTablePositions hiện tại từ cơ sở dữ liệu
            var currentKpiTablePositions = await _dbContext.KpiTablePositions
                .Where(x => x.KpiTableId == entity.Id)
                .ToListAsync();

            // Lấy danh sách KpiTablePositions từ request
            var newKpiTablePositions = request.KpiTablePositions;

            // Danh sách các vị trí cần xóa (không tồn tại trong request mới)
            var positionsToRemove = currentKpiTablePositions
                .Where(existing => !newKpiTablePositions.Any(item => item.StaffPositionId == existing.StaffPositionId))
                .ToList();

            // Danh sách các vị trí cần thêm (không tồn tại trong danh sách hiện tại)
            var positionsToAdd = newKpiTablePositions
                .Where(newItem => !currentKpiTablePositions.Any(existing => existing.StaffPositionId == newItem.StaffPositionId))
                .ToList();

            // Danh sách các vị trí cần cập nhật (có thay đổi trong request mới)
            foreach (var existingPosition in currentKpiTablePositions)
            {
                var newPosition = newKpiTablePositions.FirstOrDefault(item => item.StaffPositionId == existingPosition.StaffPositionId);
                if (newPosition != null)
                {
                    // Cập nhật các thuộc tính nếu cần
                    if (existingPosition.KpiTableId != entity.Id)
                    {
                        existingPosition.KpiTableId = entity.Id;
                    }
                    // Cập nhật thêm các thuộc tính khác nếu có thay đổi
                }
            }

            // Xóa các vị trí không còn trong request
            if (positionsToRemove.Any())
            {
                _dbContext.KpiTablePositions.RemoveRange(positionsToRemove);
            }

            // Thêm các vị trí mới từ request
            if (positionsToAdd.Any())
            {
                var newPositions = positionsToAdd.Select(item => new KpiTablePosition
                {
                    KpiTableId = entity.Id,
                    StaffPositionId = item.StaffPositionId
                });

                await _dbContext.KpiTablePositions.AddRangeAsync(newPositions);
            }

            // Cập nhật thực thể chính KpiTable
            _mapper.Map(request, entity);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();
        }


      
    }
}
