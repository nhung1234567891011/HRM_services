using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOvertimeHourToTimesheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OvertimeHour",
                table: "Timesheets",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "9f822486-db65-43a7-8550-640e3629ba47", "AQAAAAIAAYagAAAAEGEbi0wS+1RjEfqlkJKEeBPOoDfG7UXWcE6PZItKesNka1VzjH1eETsJqs0XEmV/ww==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OvertimeHour",
                table: "Timesheets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5dca8aaf-74d9-40ea-a755-7d316956617b", "AQAAAAIAAYagAAAAEFge7GEAa/a8ErLw6ZjWaen5eEexyN8t69E6DcuHelSPBeuuJyqrkJMSp7htDcOLww==" });
        }
    }
}
