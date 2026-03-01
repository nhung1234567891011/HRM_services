using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRM_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRevenueCommissionConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "KpiTableDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RevenueCommissionPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueCommissionPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevenueCommissionPolicies_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RevenueCommissionTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueCommissionTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevenueCommissionTiers_RevenueCommissionPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "RevenueCommissionPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "11ba50d2-d2ba-4baf-88f8-aac2211149f1", "AQAAAAIAAYagAAAAEG+Ck50K4ZM60oONKyb1D1BlJLNvlOiwUGx8B+CilFW97FhUWAxQwRD1ZGQbreF1EQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_RevenueCommissionPolicies_OrganizationId",
                table: "RevenueCommissionPolicies",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueCommissionTiers_PolicyId",
                table: "RevenueCommissionTiers",
                column: "PolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RevenueCommissionTiers");

            migrationBuilder.DropTable(
                name: "RevenueCommissionPolicies");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "KpiTableDetails");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "82a7a345-5e79-4520-97f9-3f345f03f43b", "AQAAAAIAAYagAAAAEJPIReV21Rg7baz5PdQmMoG0ue6ZM4mFE0trX4JOUc1rGTfzLoDtO98D8pV2F3ouEg==" });
        }
    }
}
