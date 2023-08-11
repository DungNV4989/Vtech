using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class remove_column_warehousing_bill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillDiscountRate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "WarehousingBillProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BillDiscountRate",
                table: "WarehousingBills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATRate",
                table: "WarehousingBills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRate",
                table: "WarehousingBillProducts",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
