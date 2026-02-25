using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayrollAndTimesheetForOtAndSalaryComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "CommissionManualAmount",
                table: "KpiTableDetails");

            migrationBuilder.DropColumn(
                name: "IsCommissionManual",
                table: "KpiTableDetails");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "KpiTableDetails");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "451641b8-9401-42c7-a60e-ae91f3e4de54", "AQAAAAIAAYagAAAAEAp8iP56EDulFgf7/xt8TqYEHYbP52O822xOvINwFKTB5vo9o7y0yMhqKVHHCY47Sw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "PayrollDetails",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionManualAmount",
                table: "KpiTableDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCommissionManual",
                table: "KpiTableDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "KpiTableDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fef9f03b-697e-49db-a0ce-626ab996179b", "AQAAAAIAAYagAAAAEFVg1aufBr5kvUF6vJDhib7sQD7cYb2OtRo6BFr+pk2zvNnkX81FkKRiXrcwo2GOcQ==" });
        }
    }
}
