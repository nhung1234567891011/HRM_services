using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHolidayWorkAmountToPayrollDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CheckInCheckOutApplications')
                BEGIN
                    CREATE TABLE [CheckInCheckOutApplications] (
                        [Id] int NOT NULL IDENTITY,
                        [EmployeeId] int NOT NULL,
                        [ApproverId] int NOT NULL,
                        [Date] datetime2 NOT NULL,
                        [CheckType] int NOT NULL,
                        [CheckInCheckOutStatus] int NOT NULL,
                        [ShiftCatalogId] int NOT NULL,
                        [Reason] nvarchar(max) NOT NULL,
                        [Description] nvarchar(max) NULL,
                        [TimeCheckIn] time NULL,
                        [TimeCheckOut] time NULL,
                        [CreatedBy] int NULL,
                        [CreatedName] nvarchar(max) NULL,
                        [CreatedAt] datetime2 NULL,
                        [UpdatedBy] int NULL,
                        [UpdatedName] nvarchar(max) NULL,
                        [UpdatedAt] datetime2 NULL,
                        [IsDeleted] bit NULL,
                        CONSTRAINT [PK_CheckInCheckOutApplications] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_CheckInCheckOutApplications_Employees_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_CheckInCheckOutApplications_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_CheckInCheckOutApplications_ShiftCatalogs_ShiftCatalogId] FOREIGN KEY ([ShiftCatalogId]) REFERENCES [ShiftCatalogs] ([Id]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_CheckInCheckOutApplications_ApproverId] ON [CheckInCheckOutApplications] ([ApproverId]);
                    CREATE INDEX [IX_CheckInCheckOutApplications_EmployeeId] ON [CheckInCheckOutApplications] ([EmployeeId]);
                    CREATE INDEX [IX_CheckInCheckOutApplications_ShiftCatalogId] ON [CheckInCheckOutApplications] ([ShiftCatalogId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'PayrollDetails') AND name = 'HolidayWorkAmount'
                )
                BEGIN
                    ALTER TABLE [PayrollDetails] ADD [HolidayWorkAmount] decimal(18,2) NULL;
                END
            ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5dca8aaf-74d9-40ea-a755-7d316956617b", "AQAAAAIAAYagAAAAEFge7GEAa/a8ErLw6ZjWaen5eEexyN8t69E6DcuHelSPBeuuJyqrkJMSp7htDcOLww==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CheckInCheckOutApplications')
                BEGIN
                    DROP TABLE [CheckInCheckOutApplications];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'PayrollDetails') AND name = 'HolidayWorkAmount'
                )
                BEGIN
                    ALTER TABLE [PayrollDetails] DROP COLUMN [HolidayWorkAmount];
                END
            ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "11ba50d2-d2ba-4baf-88f8-aac2211149f1", "AQAAAAIAAYagAAAAEG+Ck50K4ZM60oONKyb1D1BlJLNvlOiwUGx8B+CilFW97FhUWAxQwRD1ZGQbreF1EQ==" });
        }
    }
}
