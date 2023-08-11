using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class update_stock_price_product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestWarehousingBillId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "StockPrice",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "StockPriceBeforeLatest",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "CostPriceBeforeLatest",
                table: "Products",
                newName: "StockPriceBeforeLatest");

            migrationBuilder.AddColumn<decimal>(
                name: "StockPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "StockPriceBeforeLatest",
                table: "Products",
                newName: "CostPriceBeforeLatest");

            migrationBuilder.AddColumn<Guid>(
                name: "LatestWarehousingBillId",
                table: "StoreProducts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StockPrice",
                table: "StoreProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StockPriceBeforeLatest",
                table: "StoreProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
