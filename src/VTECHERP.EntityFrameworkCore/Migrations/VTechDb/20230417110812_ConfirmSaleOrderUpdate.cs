using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class ConfirmSaleOrderUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImportQuantity",
                table: "SaleOrderLineConfirms",
                newName: "Quantity");

            migrationBuilder.AlterColumn<decimal>(
                name: "RatePrice",
                table: "SaleOrderLineConfirms",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "SaleOrderLineConfirms",
                newName: "ImportQuantity");

            migrationBuilder.AlterColumn<int>(
                name: "RatePrice",
                table: "SaleOrderLineConfirms",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
