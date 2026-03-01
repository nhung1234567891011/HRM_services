-- Chạy script này nếu API trả 500 khi gọi fetch-payroll-details
-- (do thiếu cột UnionFeeAmount sau khi thêm tính Quỹ công đoàn).
-- Chạy trên đúng database HRM đang dùng (đổi [dbo] nếu dùng schema khác).

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PayrollDetails')
    AND name = 'UnionFeeAmount'
)
BEGIN
    ALTER TABLE dbo.PayrollDetails
    ADD UnionFeeAmount decimal(18,2) NULL;
    PRINT N'Đã thêm cột UnionFeeAmount vào bảng PayrollDetails.';
END
ELSE
    PRINT N'Cột UnionFeeAmount đã tồn tại.';
