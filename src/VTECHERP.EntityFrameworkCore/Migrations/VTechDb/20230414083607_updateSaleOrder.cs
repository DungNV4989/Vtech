using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updateSaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Confirm",
                table: "SaleOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CompleteBy",
                table: "SaleOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompleteDate",
                table: "SaleOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConfirmBy",
                table: "SaleOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmDate",
                table: "SaleOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConpleteType",
                table: "SaleOrders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompleteBy",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "CompleteDate",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmBy",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmDate",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ConpleteType",
                table: "SaleOrders");

            migrationBuilder.AlterColumn<int>(
                name: "Confirm",
                table: "SaleOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
