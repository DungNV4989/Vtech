using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_supplier_order_report_paid_finalize_flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryFinalize",
                table: "SupplierOrderReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidFinalize",
                table: "SupplierOrderReports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeliveryFinalize",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "IsPaidFinalize",
                table: "SupplierOrderReports");
        }
    }
}
