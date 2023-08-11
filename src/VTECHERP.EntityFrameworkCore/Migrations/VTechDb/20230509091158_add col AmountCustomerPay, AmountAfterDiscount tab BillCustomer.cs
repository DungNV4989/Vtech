using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addcolAmountCustomerPayAmountAfterDiscounttabBillCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterDiscount",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCustomerPay",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAfterDiscount",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "AmountCustomerPay",
                table: "BillCustomers");
        }
    }
}
