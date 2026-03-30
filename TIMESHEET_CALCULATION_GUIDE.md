# HỆ THỐNG TÍNH TOÁN CHẤM CÔNG - HRM

## 📋 Tổng Quan

Hệ thống đã được refactor để tính toán số giờ làm việc (`NumberOfWorkingHour`) hoàn toàn ở Backend, không phụ thuộc vào Frontend.

## 🔧 Các Thay Đổi Chính

### 1. **TimesheetCalculationService** (NEW)
Đường dẫn: `HRM_BE.Data/Services/TimesheetCalculationService.cs`

Service tập trung logic tính toán:
- ✅ `CalculateWorkingHours()` - Tính giờ làm việc thực tế
- ✅ `CalculateOvertimeHours()` - Tính giờ làm thêm (OT)
- ✅ `CalculateWorkDays()` - Tính ngày công

### 2. **Logic Tính Giờ Làm Việc**

```
1. start = MAX(checkIn, shiftStart)
2. end = MIN(checkOut, shiftEnd)
3. workingHours = end - start
4. Trừ nghỉ giữa ca (nếu overlap với TakeABreak)
5. Nếu thiếu check-in hoặc check-out → 0 giờ
```

### 3. **Logic Tính OT**

```
standardHours = ShiftCatalog.WorkingHours (mặc định 8h)

if (workingHours > standardHours && AllowOvertime == true):
    OT = workingHours - standardHours
else:
    OT = 0
```

### 4. **Request Models** (UPDATED)
❌ **ĐÃ XÓA** `NumberOfWorkingHour` khỏi:
- `CreateTimesheetRequest`
- `UpdateTimesheetRequest`

👉 Frontend KHÔNG CẦN gửi field này nữa. Backend sẽ tự động tính.

### 5. **Các File Đã Sửa**

#### ✅ Core Layer
- `HRM_BE.Core/IServices/ITimesheetCalculationService.cs` - Interface service
- `HRM_BE.Core/IRepositories/ITimesheetRepository.cs` - Thêm method RecalculateAllTimesheets
- `HRM_BE.Core/Models/Payroll-Timekeeping/TimekeepingRegulation/CreateTimesheetRequest.cs` - Xóa NumberOfWorkingHour
- `HRM_BE.Core/Models/Payroll-Timekeeping/TimekeepingRegulation/UpdateTimesheetRequest.cs` - Xóa NumberOfWorkingHour

#### ✅ Data Layer
- `HRM_BE.Data/Services/TimesheetCalculationService.cs` - Implementation service
- `HRM_BE.Data/Repositories/TimesheetRepository.cs` - Sử dụng service để tính
- `HRM_BE.Data/SeedWorks/RepositoryBase.cs` - Fix bug propertyInfoIsDeleted

#### ✅ API Layer
- `HRM_BE.Api/Providers/ScopedProvider.cs` - Đăng ký service
- `HRM_BE.Api/Controllers/Payroll-Timekeeping/TimekeepingRegulation/TimesheetController.cs` - Thêm endpoint RecalculateAll
- `HRM_BE.Api/Controllers/Official-Form/CheckInCheckOutApplicationController.cs` - Sử dụng service để tính

## 🚀 Cách Sử Dụng

### A. Tạo/Cập Nhật Chấm Công (Tự Động)

Các API sau sẽ TỰ ĐỘNG tính `NumberOfWorkingHour`:

```bash
# Tạo mới
POST /api/time-sheet/create
{
  "employeeId": 1,
  "shiftWorkId": 5,
  "date": "2024-03-30",
  "startTime": "08:30:00",
  "endTime": "17:00:00",
  "timeKeepingLeaveStatus": 0
}

# Cập nhật
PUT /api/time-sheet/update?id=123
{
  "startTime": "08:00:00",
  "endTime": "17:30:00"
}
```

### B. Recalculate Toàn Bộ Database

#### Cách 1: Sử dụng API (KHUYẾN NGHỊ)

```bash
# Recalculate tất cả
POST /api/time-sheet/recalculate-all

# Recalculate theo khoảng thời gian
POST /api/time-sheet/recalculate-all?startDate=2024-01-01&endDate=2024-12-31
```

#### Cách 2: Sử dụng SQL Script

Xem file: `Scripts/RecalculateTimesheets.sql`

```sql
-- Bước 1: Backup
SELECT ... INTO Timesheet_Backup_Before_Recalculate ...

-- Bước 2: Gọi API recalculate

-- Bước 3: Verify kết quả
SELECT ... FROM Timesheet_Backup_Before_Recalculate ...

-- Bước 4 (nếu cần): Rollback
UPDATE ... FROM Timesheet_Backup_Before_Recalculate ...
```

## 📊 Ví Dụ Tính Toán

### Case 1: Check-in đúng giờ, check-out đúng giờ
```
Ca làm việc: 08:00 - 17:15
Nghỉ trưa: 12:00 - 13:15
Check-in: 08:00
Check-out: 17:15

start = MAX(08:00, 08:00) = 08:00
end = MIN(17:15, 17:15) = 17:15
workingHours = 17:15 - 08:00 = 9.25h
Trừ nghỉ trưa = 9.25 - 1.25 = 8h

✅ NumberOfWorkingHour = 8h
```

### Case 2: Check-in muộn, check-out sớm
```
Ca làm việc: 08:00 - 17:15
Check-in: 09:30
Check-out: 16:00

start = MAX(09:30, 08:00) = 09:30
end = MIN(16:00, 17:15) = 16:00
workingHours = 16:00 - 09:30 = 6.5h
Trừ nghỉ trưa = 6.5 - 1.25 = 5.25h

✅ NumberOfWorkingHour = 5.25h
```

### Case 3: Check-in sớm, check-out muộn (có OT)
```
Ca làm việc: 08:00 - 17:15 (8h chuẩn)
Check-in: 07:30
Check-out: 19:00

start = MAX(07:30, 08:00) = 08:00 (cap theo ca)
end = MIN(19:00, 17:15) = 17:15 (cap theo ca)
workingHours = 17:15 - 08:00 = 9.25h
Trừ nghỉ trưa = 9.25 - 1.25 = 8h

✅ NumberOfWorkingHour = 8h
✅ OvertimeHours = 8 - 8 = 0h

⚠️ Lưu ý: Giờ làm thêm NGOÀI ca không được tính vì đã bị cap.
```

### Case 4: Thiếu check-out
```
Check-in: 08:00
Check-out: NULL

✅ NumberOfWorkingHour = 0h
```

## 🔍 Kiểm Tra Kết Quả

### Query SQL để verify:

```sql
-- Xem các bản ghi có vấn đề
SELECT 
    t.Id,
    e.FullName,
    t.Date,
    t.StartTime,
    t.EndTime,
    t.NumberOfWorkingHour,
    sc.WorkingHours AS StandardHours,
    CASE 
        WHEN t.NumberOfWorkingHour > sc.WorkingHours THEN 'Has OT'
        WHEN t.NumberOfWorkingHour = 0 AND t.StartTime IS NOT NULL THEN 'Missing data'
        ELSE 'Normal'
    END AS Status
FROM Timesheets t
INNER JOIN Employees e ON t.EmployeeId = e.Id
LEFT JOIN ShiftWorks sw ON t.ShiftWorkId = sw.Id
LEFT JOIN ShiftCatalogs sc ON sw.ShiftCatalogId = sc.Id
WHERE t.Date >= '2024-01-01'
ORDER BY t.Date DESC;
```

## ⚠️ Lưu Ý Quan Trọng

1. **Không Tin FE Nữa**
   - Request từ FE không còn field `NumberOfWorkingHour`
   - Backend sẽ tính toán hoàn toàn dựa trên:
     - StartTime
     - EndTime
     - ShiftWorkId (để lấy thông tin ca)

2. **Migration Frontend**
   - Xóa field `numberOfWorkingHour` khỏi form models
   - Xóa logic tính toán client-side
   - Chỉ gửi startTime, endTime, shiftWorkId

3. **Recalculate Database**
   - ⚠️ **PHẢI CHẠY** sau khi deploy code mới
   - Backup trước khi chạy
   - Gọi API: `POST /api/time-sheet/recalculate-all`

4. **Testing**
   - Test các case: đúng giờ, muộn, sớm, thiếu check-in/out
   - Verify với các ca khác nhau
   - Kiểm tra OT có chính xác không

## 📞 Hỗ Trợ

Nếu gặp vấn đề:
1. Check logs trong `Logs/app.log`
2. Verify ShiftCatalog có đầy đủ StartTime, EndTime, TakeABreak
3. Chạy SQL script backup trước khi recalculate
4. Liên hệ team dev để rollback nếu cần

## 🎯 Checklist Deploy

- [ ] Build và test local
- [ ] Backup database
- [ ] Deploy code mới
- [ ] Gọi API recalculate-all
- [ ] Verify kết quả bằng SQL
- [ ] Update Frontend (xóa NumberOfWorkingHour)
- [ ] Test end-to-end
- [ ] Monitor logs

---

**Ngày tạo**: 2024-03-30  
**Version**: 2.0  
**Tác giả**: HRM Development Team
