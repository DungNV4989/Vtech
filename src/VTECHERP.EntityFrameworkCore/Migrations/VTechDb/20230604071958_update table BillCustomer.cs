using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updatetableBillCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BillVoucherDiscountValue",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherDiscountUnit",
                table: "BillCustomers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VoucherDiscountValue",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "VoucherId",
                table: "BillCustomers",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillVoucherDiscountValue",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherDiscountUnit",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherDiscountValue",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "BillCustomers");
        }
    }
}
