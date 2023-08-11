using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_accumulated_delivery_supplier_report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Suppliers");

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedBeforeCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedBeforeVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedBeforeCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedBeforeVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedVND",
                table: "SupplierOrderReports");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
