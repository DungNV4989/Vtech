using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_table_warehouse_bill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "WarehousingBills");

            migrationBuilder.RenameColumn(
                name: "PaymentDiscountType",
                table: "WarehousingBills",
                newName: "BillDiscountType");

            migrationBuilder.RenameColumn(
                name: "PaymentDiscountAmount",
                table: "WarehousingBills",
                newName: "VATRate");

            migrationBuilder.RenameColumn(
                name: "CustomerPhone",
                table: "WarehousingBills",
                newName: "AudiencePhone");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "WarehousingBills",
                newName: "AudienceName");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "WarehousingBills",
                newName: "AudienceId");

            migrationBuilder.AddColumn<string>(
                name: "AudienceCode",
                table: "WarehousingBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BillDiscountAmount",
                table: "WarehousingBills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BillDiscountRate",
                table: "WarehousingBills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRate",
                table: "WarehousingBillProducts",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceCode",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "BillDiscountAmount",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "BillDiscountRate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "WarehousingBillProducts");

            migrationBuilder.RenameColumn(
                name: "VATRate",
                table: "WarehousingBills",
                newName: "PaymentDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "BillDiscountType",
                table: "WarehousingBills",
                newName: "PaymentDiscountType");

            migrationBuilder.RenameColumn(
                name: "AudiencePhone",
                table: "WarehousingBills",
                newName: "CustomerPhone");

            migrationBuilder.RenameColumn(
                name: "AudienceName",
                table: "WarehousingBills",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "AudienceId",
                table: "WarehousingBills",
                newName: "CustomerId");

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierId",
                table: "WarehousingBills",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
