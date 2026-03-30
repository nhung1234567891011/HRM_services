IF COL_LENGTH('dbo.PayrollDetails', 'HolidayWorkAmount') IS NULL
BEGIN
    ALTER TABLE dbo.PayrollDetails
    ADD HolidayWorkAmount DECIMAL(18,2) NULL;
END;
GO
