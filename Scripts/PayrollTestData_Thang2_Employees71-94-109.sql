-- =============================================================================
-- Dữ liệu test tính lương – Tháng 2/2025
-- Nhân viên: 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86,
--            87, 88, 89, 90, 91, 92, 93, 94, 109
-- =============================================================================
-- Cách dùng:
-- 1. Đặt @OrgId trùng với OrganizationId của 25 nhân viên trên (đảm bảo họ cùng 1 đơn vị).
-- 2. Chạy toàn bộ script.
-- 3. Tạo Bảng lương qua API: POST /api/payroll/create (OrganizationId = @OrgId).
-- 4. Cập nhật tháng tính lương: UPDATE Payrolls SET CreatedAt = '2025-02-01' WHERE Id = <PayrollId>;
-- 5. Gọi tính lương: POST /api/payroll-detail/calculate-and-save-payroll-details?payrollId=<PayrollId>
-- =============================================================================

SET NOCOUNT ON;

-- Biến: sửa @OrgId nếu 25 nhân viên thuộc đơn vị khác
DECLARE @OrgId INT = 1;
DECLARE @Year INT = 2025;
DECLARE @Month INT = 2;
DECLARE @FromDate DATE = '2025-02-01';
DECLARE @ToDate DATE = '2025-02-28';

DECLARE @ShiftWorkId INT = NULL;
DECLARE @KpiTableId INT = NULL;
DECLARE @ContractTypeId INT = NULL;
DECLARE @ContractDurationId INT = NULL;
DECLARE @WorkingFormId INT = NULL;

-- Lấy ID tham chiếu (nếu có trong DB)
SELECT TOP 1 @ContractTypeId = Id FROM dbo.ContractTypes WHERE (IsDeleted = 0 OR IsDeleted IS NULL);
SELECT TOP 1 @ContractDurationId = Id FROM dbo.ContractDurations WHERE (IsDeleted = 0 OR IsDeleted IS NULL);
SELECT TOP 1 @WorkingFormId = Id FROM dbo.WorkingForms WHERE (IsDeleted = 0 OR IsDeleted IS NULL);

-- =============================================================================
-- 1. CONTRACTS (mỗi nhân viên 1 hợp đồng – lương, KPI, tỉ lệ, lương đóng BH)
-- =============================================================================
INSERT INTO dbo.Contracts (
    EmployeeId, UnitId, SalaryAmount, SalaryInsurance, SalaryRate, KpiSalary,
    EffectiveDate, ExpiryDate, ContractTypeStatus, SignStatus, ContractTypeId,
    ContractDurationId, WorkingFormId, IsDeleted, CreatedAt
)
VALUES
    (71, @OrgId, 15000000, 15000000, 100, 3000000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (72, @OrgId, 16000000, 16000000, 100, 3200000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (73, @OrgId, 15500000, 15500000, 100, 3100000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (74, @OrgId, 17000000, 17000000, 100, 3400000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (75, @OrgId, 14500000, 14500000, 100, 2900000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (76, @OrgId, 18000000, 18000000, 100, 3600000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (77, @OrgId, 15200000, 15200000, 100, 3040000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (78, @OrgId, 16500000, 16500000, 100, 3300000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (79, @OrgId, 15800000, 15800000, 100, 3160000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (80, @OrgId, 17200000, 17200000, 100, 3440000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (81, @OrgId, 14800000, 14800000, 100, 2960000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (82, @OrgId, 16200000, 16200000, 100, 3240000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (83, @OrgId, 15500000, 15500000, 100, 3100000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (84, @OrgId, 16800000, 16800000, 100, 3360000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (85, @OrgId, 15000000, 15000000, 100, 3000000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (86, @OrgId, 17500000, 17500000, 100, 3500000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (87, @OrgId, 15300000, 15300000, 100, 3060000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (88, @OrgId, 16600000, 16600000, 100, 3320000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (89, @OrgId, 15900000, 15900000, 100, 3180000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (90, @OrgId, 17300000, 17300000, 100, 3460000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (91, @OrgId, 14700000, 14700000, 100, 2940000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (92, @OrgId, 16100000, 16100000, 100, 3220000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (93, @OrgId, 15400000, 15400000, 100, 3080000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (94, @OrgId, 16900000, 16900000, 100, 3380000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE()),
    (109, @OrgId, 18500000, 18500000, 100, 3700000, @FromDate, '2026-12-31', 1, 0, @ContractTypeId, @ContractDurationId, @WorkingFormId, 0, GETDATE());

-- =============================================================================
-- 2. SHIFTWORKS (ngày công chuẩn – nếu chưa có cho @OrgId)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.ShiftWorks WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL))
BEGIN
    INSERT INTO dbo.ShiftWorks (
        OrganizationId, ShiftTableName, TotalWork, StartDate, EndDate,
        RecurrenceType, IsMonday, IsTuesday, IsWednesday, IsThursday, IsFriday,
        IsSaturday, IsSunday, IsDeleted, CreatedAt
    )
    VALUES (
        @OrgId, N'Ca hành chính T2/2025', 22, @FromDate, @ToDate,
        1, 1, 1, 1, 1, 1, 0, 0, 0, GETDATE()
    );
    SET @ShiftWorkId = SCOPE_IDENTITY();
END
ELSE
    SELECT TOP 1 @ShiftWorkId = Id FROM dbo.ShiftWorks WHERE OrganizationId = @OrgId AND (IsDeleted = 0 OR IsDeleted IS NULL);

-- =============================================================================
-- 3. TIMESHEETS – 15 ngày công trong tháng 2/2025 (T2–T6), 8h/ngày
-- Ngày: 03,04,05,06,07, 10,11,12,13,14, 17,18,19,20,21
-- =============================================================================
DECLARE @EmpIds TABLE (EmployeeId INT);
INSERT INTO @EmpIds (EmployeeId) VALUES (71),(72),(73),(74),(75),(76),(77),(78),(79),(80),(81),(82),(83),(84),(85),(86),(87),(88),(89),(90),(91),(92),(93),(94),(109);

DECLARE @WorkDays TABLE (Dt DATE);
INSERT INTO @WorkDays (Dt) VALUES
    ('2025-02-03'),('2025-02-04'),('2025-02-05'),('2025-02-06'),('2025-02-07'),
    ('2025-02-10'),('2025-02-11'),('2025-02-12'),('2025-02-13'),('2025-02-14'),
    ('2025-02-17'),('2025-02-18'),('2025-02-19'),('2025-02-20'),('2025-02-21');

INSERT INTO dbo.Timesheets (EmployeeId, ShiftWorkId, Date, NumberOfWorkingHour, TimeKeepingLeaveStatus, IsDeleted, CreatedAt)
SELECT e.EmployeeId, @ShiftWorkId, w.Dt, 8, 0, 0, GETDATE()
FROM @EmpIds e
CROSS JOIN @WorkDays w;

-- =============================================================================
-- 4. KpiTables – 1 bảng KPI tháng 2/2025
-- =============================================================================
INSERT INTO dbo.KpiTables (OrganizationId, NameKpiTable, FromDate, ToDate, IsDeleted, CreatedAt)
VALUES (@OrgId, N'KPI T2/2025', @FromDate, @ToDate, 0, GETDATE());
SET @KpiTableId = SCOPE_IDENTITY();

-- =============================================================================
-- 5. KpiTableDetails – % hoàn thành, thưởng, doanh thu từng nhân viên
-- =============================================================================
INSERT INTO dbo.KpiTableDetails (KpiTableId, EmployeeId, CompletionRate, Bonus, Revenue, IsDeleted, CreatedAt)
VALUES
    (@KpiTableId, 71, 85, 500000, 0, 0, GETDATE()),
    (@KpiTableId, 72, 90, 600000, 0, 0, GETDATE()),
    (@KpiTableId, 73, 88, 550000, 0, 0, GETDATE()),
    (@KpiTableId, 74, 92, 700000, 0, 0, GETDATE()),
    (@KpiTableId, 75, 82, 400000, 0, 0, GETDATE()),
    (@KpiTableId, 76, 95, 800000, 0, 0, GETDATE()),
    (@KpiTableId, 77, 86, 520000, 0, 0, GETDATE()),
    (@KpiTableId, 78, 91, 650000, 0, 0, GETDATE()),
    (@KpiTableId, 79, 89, 580000, 0, 0, GETDATE()),
    (@KpiTableId, 80, 93, 720000, 0, 0, GETDATE()),
    (@KpiTableId, 81, 84, 480000, 0, 0, GETDATE()),
    (@KpiTableId, 82, 90, 620000, 0, 0, GETDATE()),
    (@KpiTableId, 83, 87, 530000, 0, 0, GETDATE()),
    (@KpiTableId, 84, 91, 660000, 0, 0, GETDATE()),
    (@KpiTableId, 85, 85, 500000, 0, 0, GETDATE()),
    (@KpiTableId, 86, 94, 750000, 0, 0, GETDATE()),
    (@KpiTableId, 87, 86, 510000, 0, 0, GETDATE()),
    (@KpiTableId, 88, 90, 630000, 0, 0, GETDATE()),
    (@KpiTableId, 89, 88, 560000, 0, 0, GETDATE()),
    (@KpiTableId, 90, 92, 680000, 0, 0, GETDATE()),
    (@KpiTableId, 91, 83, 450000, 0, 0, GETDATE()),
    (@KpiTableId, 92, 89, 590000, 0, 0, GETDATE()),
    (@KpiTableId, 93, 87, 540000, 0, 0, GETDATE()),
    (@KpiTableId, 94, 91, 670000, 0, 0, GETDATE()),
    (@KpiTableId, 109, 96, 900000, 0, 0, GETDATE());

-- =============================================================================
-- 6. Deductions (tùy chọn – vài nhân viên có khấu trừ mẫu)
-- =============================================================================
INSERT INTO dbo.Deductions (EmployeeId, DeducationName, StandardType, Value, IsDeleted, CreatedAt)
VALUES
    (71, N'Khấu trừ tạm ứng', N'Cố định', 200000, 0, GETDATE()),
    (75, N'Khấu trừ tạm ứng', N'Cố định', 150000, 0, GETDATE()),
    (82, N'Khấu trừ tạm ứng', N'Cố định', 300000, 0, GETDATE());

-- =============================================================================
-- SAU KHI CHẠY SCRIPT
-- =============================================================================
-- 1. Tạo Bảng lương qua API:
--    POST /api/payroll/create
--    Body: { "organizationId": <@OrgId>, "payrollName": "Bảng lương T2/2025", "payrollStatus": 0, "payrollConfirmationStatus": 0 }
--
-- 2. Đặt tháng tính lương = tháng 2 (bắt buộc để khớp Timesheets + KPI):
--    UPDATE Payrolls SET CreatedAt = '2025-02-01' WHERE Id = <PayrollId>;
--
-- 3. Tính và lưu phiếu lương:
--    POST /api/payroll-detail/calculate-and-save-payroll-details?payrollId=<PayrollId>
--
-- 4. Kiểm tra: Xem danh sách PayrollDetail theo payrollId, đối chiếu
--    BaseSalary, ActualWorkDays, ReceivedSalary, KpiSalary, Bonus,
--    AllowanceMealTravel, ParkingAmount, OvertimeAmount, BhxhAmount,
--    UnionFeeAmount, TotalSalary, TotalReceivedSalary.
-- =============================================================================

PRINT N'Đã chèn Contracts, ShiftWorks, Timesheets, KpiTables, KpiTableDetails, Deductions cho tháng 2/2025.';
PRINT N'Tiếp theo: tạo Payroll (API), cập nhật Payrolls.CreatedAt = 2025-02-01, gọi API calculate-and-save-payroll-details.';
