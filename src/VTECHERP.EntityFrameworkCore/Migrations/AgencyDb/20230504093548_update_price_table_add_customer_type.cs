using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class update_price_table_add_customer_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                 name: "PriceChartId",
                 table: "PriceTableProducts",
                 newName: "PriceTableId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                table: "PriceTables",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "PriceTables");

            migrationBuilder.RenameColumn(
                name: "PriceTableId",
                table: "PriceTableProducts",
                newName: "PriceChartId");
        }
    }
}
