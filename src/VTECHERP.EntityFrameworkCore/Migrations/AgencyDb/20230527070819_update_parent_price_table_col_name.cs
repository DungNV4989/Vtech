using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class update_parent_price_table_col_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentPriceChartId",
                table: "PriceTables",
                newName: "ParentPriceTableId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppliedTo",
                table: "PriceTables",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentPriceTableId",
                table: "PriceTables",
                newName: "ParentPriceChartId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppliedTo",
                table: "PriceTables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
