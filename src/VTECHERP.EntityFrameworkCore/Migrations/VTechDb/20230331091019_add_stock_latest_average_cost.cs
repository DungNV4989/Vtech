using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_stock_latest_average_cost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageCostBeforeLatest",
                table: "StoreProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "LatestWarehousingBillId",
                table: "StoreProducts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityBeforeLatest",
                table: "StoreProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConfigCode",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageCostBeforeLatest",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "LatestWarehousingBillId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "QuantityBeforeLatest",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ConfigCode",
                table: "Entries");
        }
    }
}
