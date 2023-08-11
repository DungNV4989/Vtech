using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addImportQuantity_SaleOrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirm",
                table: "SaleOrders");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Confirm",
                table: "SaleOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SaleOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Package",
                table: "SaleOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImportQuantity",
                table: "SaleOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SaleOrderLines",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Confirm",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Package",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ImportQuantity",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SaleOrderLines");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirm",
                table: "SaleOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
