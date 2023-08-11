using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addCostPricebillCustomerProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CustomerReturns");

            migrationBuilder.AlterDatabase(
                collation: "SQL_Latin1_General_CP1_CI_AI");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "BillCustomerProducts",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "BillCustomerProducts");

            migrationBuilder.AlterDatabase(
                oldCollation: "SQL_Latin1_General_CP1_CI_AI");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
