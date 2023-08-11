using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addcodecolumntbBillCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "BillCustomerCode");

            migrationBuilder.CreateSequence<int>(
                name: "BillCustomerProductBonusCode");

            migrationBuilder.CreateSequence<int>(
                name: "BillCustomerProductCode");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR BillCustomerCode,'0000000000')");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "BillCustomerProducts",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR BillCustomerProductCode,'0000000000')");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "BillCustomerProductBonus",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR BillCustomerProductBonusCode,'0000000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "BillCustomerCode");

            migrationBuilder.DropSequence(
                name: "BillCustomerProductBonusCode");

            migrationBuilder.DropSequence(
                name: "BillCustomerProductCode");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "BillCustomerProducts");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "BillCustomerProductBonus");
        }
    }
}
