-- =============================================================================
-- Script dữ liệu test cho tính lương (Hoa hồng, BHXH, Quỹ công đoàn)
-- Chạy trên database HRM (đổi tên schema nếu cần, ví dụ: [HRM-Demo-1].dbo)
-- Tháng test: 03/2025. Sau khi chạy: vào Bảng lương chi tiết của Payroll vừa tạo
-- và bấm "Cập nhật phiếu lương" để tính Hoa hồng, BHXH, Tổng lương thực nhận.
-- =============================================================================

SET NOCOUNT ON;

DECLARE @OrgId INT;
DECLARE @SalePositionId INT;
DECLARE @EmployeeId INT;
DECLARE @ContractId INT;
DECLARE @ShiftCatalogId INT;
DECLARE @ShiftWorkId INT;
DECLARE @KpiTableId INT;
DECLARE @PolicyId INT;
DECLARE @PayrollId INT;
DECLARE @ContractTypeId INT;
DECLARE @WorkingFormId INT;
DECLARE @NatureOfLaborId INT;

-- 1. Lấy đơn vị (tổ chức) đầu tiên
SELECT TOP 1 @OrgId = Id FROM dbo.Organizations WHERE (IsDeleted = 0 OR IsDeleted IS NULL);
IF @OrgId IS NULL
BEGIN
    RAISERROR('Chưa có Organization. Tạo đơn vị trước hoặc chèn Organizations.', 16, 1);
    RETURN;
END

-- 2. Tạo hoặc lấy vị trí SALE (để áp dụng hoa hồng)
IF NOT EXISTS (SELECT 1 FROM dbo.StaffPositions WHERE PositionCode = N'SALE' AND (IsDeleted = 0 OR IsDeleted IS NULL))
BEGIN
    INSERT INTO dbo.StaffPositions (PositionCode, PositionName, StaffPositionStatus, CreatedAt, IsDeleted)
    VALUES (N'SALE', N'Nhân viên kinh doanh', 1, GETDATE(), 0);
    SET @SalePositionId = SCOPE_IDENTITY();
    INSERT INTO dbo.OrganizationPositions (StaffPositionId, OrganizationId)
    VALUES (@SalePositionId, @OrgId);
END
ELSE
    SELECT TOP 1 @SalePositionId = Id FROM dbo.StaffPositions WHERE PositionCode = N'SALE' AND (IsDeleted = 0 OR IsDeleted IS NULL);

-- 3. Lấy ContractType, WorkingForm, NatureOfLabor (cần cho Contract)
SELECT TOP 1 @ContractTypeId = Id FROM dbo.ContractTypes WHERE (IsDeleted = 0 OR IsDeleted IS NULL);
SELECT TOP 1 @WorkingFormId = Id FROM dbo.WorkingForms WHERE (IsDeleted = 0 OR IsDeleted IS NULL);
SELECT TOP 1 @NatureOfLaborId = Id FROM dbo.NatureOfLabor WHERE (IsDeleted = 0 OR IsDeleted IS NULL);

-- 4. Tạo nhân viên test (SALE)
INSERT INTO dbo.Employees (
    OrganizationId, StaffPositionId, EmployeeCode, FirstName, LastName,
    AccountStatus, WorkingStatus, IsDeleted, CreatedAt
)
VALUES (
    @OrgId, @SalePositionId, N'TEST_SALE001', N'Minh', N'Nguyễn Văn',
    1, 0, 0, GETDATE()
);
SET @EmployeeId = SCOPE_IDENTITY();

-- 5. Hợp đồng: lương 25.000.000, lương đóng BH 25.000.000, tỉ lệ 100%, KPI 5.000.000
INSERT INTO dbo.Contracts (
    EmployeeId, UnitId, SalaryAmount, SalaryInsurance, SalaryRate, KpiSalary,
    ContractTypeStatus, ContractTypeId, WorkingFormId, NatureOfLaborId,
    SignStatus, ExpiredStatus, EffectiveDate, ExpiryDate, IsDeleted, CreatedAt
)
VALUES (
    @EmployeeId, @OrgId, 25000000, 25000000, 100, 5000000,
    0, @ContractTypeId, @WorkingFormId, @NatureOfLaborId,
    0, 0, '2025-01-01', '2026-12-31', 0, GETDATE()
);
SET @ContractId = SCOPE_IDENTITY();

-- 6. Ca làm việc + ShiftWork (ngày công chuẩn 22)
IF NOT EXISTS (SELECT 1 FROM dbo.ShiftCatalogs WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL))
BEGIN
    INSERT INTO dbo.ShiftCatalogs (OrganizationId, Code, Name, WorkingHours, WorkingDays, IsDeleted, CreatedAt)
    VALUES (@OrgId, N'HC', N'Hành chính', 8, 1, 0, GETDATE());
    SET @ShiftCatalogId = SCOPE_IDENTITY();
END
ELSE
    SELECT TOP 1 @ShiftCatalogId = Id FROM dbo.ShiftCatalogs WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.ShiftWorks WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL))
BEGIN
    INSERT INTO dbo.ShiftWorks (OrganizationId, ShiftCatalogId, ShiftTableName, TotalWork, StartDate, EndDate, IsDeleted, CreatedAt)
    VALUES (@OrgId, @ShiftCatalogId, N'Ca test', 22, '2025-03-01', '2025-03-31', 0, GETDATE());
    SET @ShiftWorkId = SCOPE_IDENTITY();
END
ELSE
    SELECT TOP 1 @ShiftWorkId = Id FROM dbo.ShiftWorks WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL);

-- 7. Chấm công tháng 03/2025 (20 ngày x 8h, 2 ngày x 9h để có tăng ca)
INSERT INTO dbo.Timesheets (ShiftWorkId, EmployeeId, [Date], NumberOfWorkingHour, IsDeleted, CreatedAt)
SELECT @ShiftWorkId, @EmployeeId, d, 8, 0, GETDATE()
FROM (VALUES
    ('2025-03-03'),('2025-03-04'),('2025-03-05'),('2025-03-06'),('2025-03-07'),
    ('2025-03-10'),('2025-03-11'),('2025-03-12'),('2025-03-13'),('2025-03-14'),
    ('2025-03-17'),('2025-03-18'),('2025-03-19'),('2025-03-20'),('2025-03-21'),
    ('2025-03-24'),('2025-03-25'),('2025-03-26'),('2025-03-27')
) AS T(d);
INSERT INTO dbo.Timesheets (ShiftWorkId, EmployeeId, [Date], NumberOfWorkingHour, IsDeleted, CreatedAt)
VALUES (@ShiftWorkId, @EmployeeId, '2025-03-28', 9, 0, GETDATE()),
       (@ShiftWorkId, @EmployeeId, '2025-03-31', 9, 0, GETDATE());

-- 8. Bảng KPI tháng 03/2025 + chi tiết (doanh thu 100tr để có hoa hồng)
INSERT INTO dbo.KpiTables (OrganizationId, NameKpiTable, FromDate, ToDate, IsDeleted, CreatedAt)
VALUES (@OrgId, N'KPI 03/2025', '2025-03-01', '2025-03-31', 0, GETDATE());
SET @KpiTableId = SCOPE_IDENTITY();

INSERT INTO dbo.KpiTableDetails (KpiTableId, EmployeeId, EmployeeCode, EmployeeName, Revenue, CompletionRate, Bonus, IsDeleted, CreatedAt)
VALUES (@KpiTableId, @EmployeeId, N'TEST_SALE001', N'Nguyễn Văn Minh', 100000000, 90, 500000, 0, GETDATE());

-- 9. Thành phần lương: BHXH 8%, Quỹ công đoàn 1%, Phụ cấp đi lại 1tr, Gửi xe 3k/ngày
IF NOT EXISTS (SELECT 1 FROM dbo.SalaryComponents WHERE OrganizationId = @OrgId AND ComponentCode = N'BHXH' AND (IsDeleted = 0 OR IsDeleted IS NULL))
    INSERT INTO dbo.SalaryComponents (OrganizationId, ComponentName, ComponentCode, Nature, Characteristic, CalcType, BaseSource, RatePercent, [Status], IsDeleted, CreatedAt, Description)
    VALUES (@OrgId, N'BHXH', N'BHXH', 1, 0, 2, 1, 8, 0, 0, GETDATE(), N'Bảo hiểm xã hội 8%');

IF NOT EXISTS (SELECT 1 FROM dbo.SalaryComponents WHERE OrganizationId = @OrgId AND ComponentCode = N'QUY_CONG_DOAN' AND (IsDeleted = 0 OR IsDeleted IS NULL))
    INSERT INTO dbo.SalaryComponents (OrganizationId, ComponentName, ComponentCode, Nature, Characteristic, CalcType, BaseSource, RatePercent, [Status], IsDeleted, CreatedAt, Description)
    VALUES (@OrgId, N'Quỹ công đoàn', N'QUY_CONG_DOAN', 1, 0, 2, 1, 1, 0, 0, GETDATE(), N'Quỹ công đoàn 1%');

IF NOT EXISTS (SELECT 1 FROM dbo.SalaryComponents WHERE OrganizationId = @OrgId AND ComponentCode = N'ALLOWANCE_MEAL_TRAVEL' AND (IsDeleted = 0 OR IsDeleted IS NULL))
    INSERT INTO dbo.SalaryComponents (OrganizationId, ComponentName, ComponentCode, Nature, Characteristic, CalcType, FixedAmount, [Status], IsDeleted, CreatedAt, Description)
    VALUES (@OrgId, N'Phụ cấp đi lại ăn trưa', N'ALLOWANCE_MEAL_TRAVEL', 0, 0, 0, 1000000, 0, 0, GETDATE(), N'Cố định 1.000.000');

IF NOT EXISTS (SELECT 1 FROM dbo.SalaryComponents WHERE OrganizationId = @OrgId AND ComponentCode = N'PARKING' AND (IsDeleted = 0 OR IsDeleted IS NULL))
    INSERT INTO dbo.SalaryComponents (OrganizationId, ComponentName, ComponentCode, Nature, Characteristic, CalcType, UnitAmount, [Status], IsDeleted, CreatedAt, Description)
    VALUES (@OrgId, N'Tiền gửi xe', N'PARKING', 0, 0, 1, 3000, 0, 0, GETDATE(), N'3.000/ngày đi làm');

-- 10. Chính sách hoa hồng SALE: 0-50tr 5%, trên 50tr 7%
IF NOT EXISTS (SELECT 1 FROM dbo.RevenueCommissionPolicies WHERE OrganizationId = @OrgId AND TargetType = 0 AND (IsDeleted = 0 OR IsDeleted IS NULL))
BEGIN
    INSERT INTO dbo.RevenueCommissionPolicies (OrganizationId, TargetType, EffectiveFrom, EffectiveTo, [Status], IsDeleted, CreatedAt)
    VALUES (@OrgId, 0, '2025-01-01', '2025-12-31', 0, 0, GETDATE());
    SET @PolicyId = SCOPE_IDENTITY();
    INSERT INTO dbo.RevenueCommissionTiers (PolicyId, FromAmount, ToAmount, RatePercent, SortOrder, IsDeleted, CreatedAt)
    VALUES (@PolicyId, 0, 50000000, 5, 1, 0, GETDATE()),
           (@PolicyId, 50000000, NULL, 7, 2, 0, GETDATE());
END

-- 11. Khấu trừ test (200.000)
INSERT INTO dbo.Deductions (EmployeeId, DeducationName, Value, IsDeleted, CreatedAt)
VALUES (@EmployeeId, N'Khấu trừ test', 200000, 0, GETDATE());

-- 12. Bảng lương tháng 03/2025
INSERT INTO dbo.Payrolls (OrganizationId, PayrollName, PayrollStatus, PayrollConfirmationStatus, CreatedAt, IsDeleted)
VALUES (@OrgId, N'Bảng lương test 03/2025', 0, 0, '2025-03-15', 0);
SET @PayrollId = SCOPE_IDENTITY();

INSERT INTO dbo.PayrollStaffPositions (PayrollId, StaffPositionId)
VALUES (@PayrollId, @SalePositionId);

-- Kết quả
SELECT @OrgId AS OrganizationId, @EmployeeId AS EmployeeId, @ContractId AS ContractId, @PayrollId AS PayrollId;
PRINT N'Đã tạo dữ liệu test. PayrollId = ' + CAST(@PayrollId AS NVARCHAR(10));
PRINT N'Mở màn Bảng lương chi tiết (PayrollId=' + CAST(@PayrollId AS NVARCHAR(10)) + N') và bấm "Cập nhật phiếu lương" để xem Hoa hồng, BHXH, Quỹ công đoàn.';
