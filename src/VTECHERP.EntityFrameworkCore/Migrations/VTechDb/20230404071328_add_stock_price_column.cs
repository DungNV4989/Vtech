using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_stock_price_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAverageCost",
                table: "WarehousingBills",
                newName: "TotalStockPrice");

            migrationBuilder.RenameColumn(
                name: "AverageCostBeforeLatest",
                table: "StoreProducts",
                newName: "StockPriceBeforeLatest");

            migrationBuilder.RenameColumn(
                name: "AverageCost",
                table: "StoreProducts",
                newName: "StockPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "UpdatedStockPrice",
                table: "WarehousingBillProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedStockQuantity",
                table: "WarehousingBillProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedStockPrice",
                table: "WarehousingBillProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedStockQuantity",
                table: "WarehousingBillProducts");

            migrationBuilder.RenameColumn(
                name: "TotalStockPrice",
                table: "WarehousingBills",
                newName: "TotalAverageCost");

            migrationBuilder.RenameColumn(
                name: "StockPriceBeforeLatest",
                table: "StoreProducts",
                newName: "AverageCostBeforeLatest");

            migrationBuilder.RenameColumn(
                name: "StockPrice",
                table: "StoreProducts",
                newName: "AverageCost");
        }
    }
}
