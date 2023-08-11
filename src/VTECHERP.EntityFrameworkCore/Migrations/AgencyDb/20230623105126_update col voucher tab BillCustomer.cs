using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class updatecolvouchertabBillCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoucherApplyFor",
                table: "BillCustomers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherApplyProductCategoryIds",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherApplyProductIds",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherApplyStoreIds",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VoucherBillMaxValue",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VoucherBillMinValue",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VoucherMaxDiscountValue",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VoucherNotApplyWithDiscount",
                table: "BillCustomers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherApplyFor",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherApplyProductCategoryIds",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherApplyProductIds",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherApplyStoreIds",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherBillMaxValue",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherBillMinValue",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherMaxDiscountValue",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "VoucherNotApplyWithDiscount",
                table: "BillCustomers");
        }
    }
}
