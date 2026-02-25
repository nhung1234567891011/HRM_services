using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollDetailOtAndSalaryComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AllowanceMealTravel",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhxhAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OvertimeAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ParkingAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "56dbb0af-f2c4-4aae-8acf-50451823c2e1", "AQAAAAIAAYagAAAAEJHT8KCU//9IXNbvsCHNym3uU2v14YMBgA7rFa28tgD992Xs5N2prQhlyECrQYdv6g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowanceMealTravel",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "BhxhAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "OvertimeAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "ParkingAmount",
                table: "PayrollDetails");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "451641b8-9401-42c7-a60e-ae91f3e4de54", "AQAAAAIAAYagAAAAEAp8iP56EDulFgf7/xt8TqYEHYbP52O822xOvINwFKTB5vo9o7y0yMhqKVHHCY47Sw==" });
        }
    }
}
