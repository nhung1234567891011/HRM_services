-- Thêm cột BhtnAmount, BhytAmount vào bảng PayrollDetails
-- Chạy script này nếu chưa chạy migration AddBhtnBhytToPayrollDetail

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PayrollDetails')
    AND name = 'BhtnAmount'
)
BEGIN
    ALTER TABLE dbo.PayrollDetails
    ADD BhtnAmount decimal(18,2) NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PayrollDetails')
    AND name = 'BhytAmount'
)
BEGIN
    ALTER TABLE dbo.PayrollDetails
    ADD BhytAmount decimal(18,2) NULL;
END
GO

-- Đánh dấu migration đã chạy để lần sau 'dotnet ef database update' không chạy lại
IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20260301110000_AddBhtnBhytToPayrollDetail')
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (N'20260301110000_AddBhtnBhytToPayrollDetail', N'8.0.0');
END
GO
