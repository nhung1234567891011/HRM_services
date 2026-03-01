using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryComponentCalcRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseSource",
                table: "SalaryComponents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalcType",
                table: "SalaryComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CapAmount",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedAmount",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RatePercent",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitAmount",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "82a7a345-5e79-4520-97f9-3f345f03f43b", "AQAAAAIAAYagAAAAEJPIReV21Rg7baz5PdQmMoG0ue6ZM4mFE0trX4JOUc1rGTfzLoDtO98D8pV2F3ouEg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseSource",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "CalcType",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "CapAmount",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "FixedAmount",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "RatePercent",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "UnitAmount",
                table: "SalaryComponents");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "56dbb0af-f2c4-4aae-8acf-50451823c2e1", "AQAAAAIAAYagAAAAEJHT8KCU//9IXNbvsCHNym3uU2v14YMBgA7rFa28tgD992Xs5N2prQhlyECrQYdv6g==" });
        }
    }
}
