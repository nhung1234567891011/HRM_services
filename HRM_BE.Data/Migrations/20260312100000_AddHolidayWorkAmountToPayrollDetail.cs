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
            migrationBuilder.AddColumn<decimal>(
                name: "HolidayWorkAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HolidayWorkAmount",
                table: "PayrollDetails");
        }
    }
}
