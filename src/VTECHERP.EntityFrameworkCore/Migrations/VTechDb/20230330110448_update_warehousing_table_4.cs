using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_warehousing_table_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BankPaymentHaveValueDate",
                table: "WarehousingBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CashPaymentHaveValueDate",
                table: "WarehousingBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAverageCost",
                table: "WarehousingBills",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "VATHaveValueDate",
                table: "WarehousingBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageCost",
                table: "StoreProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankPaymentHaveValueDate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "CashPaymentHaveValueDate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "TotalAverageCost",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "VATHaveValueDate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "AverageCost",
                table: "StoreProducts");
        }
    }
}
