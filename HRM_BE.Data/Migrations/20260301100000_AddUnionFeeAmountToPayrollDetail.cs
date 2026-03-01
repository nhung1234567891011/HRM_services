using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUnionFeeAmountToPayrollDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnionFeeAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnionFeeAmount",
                table: "PayrollDetails");
        }
    }
}
