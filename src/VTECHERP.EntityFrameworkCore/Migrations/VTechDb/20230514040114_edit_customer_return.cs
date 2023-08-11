using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class edit_customer_return : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCareName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeSellName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CustomerReturns",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "CustomerReturns");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CustomerReturns");

            migrationBuilder.DropColumn(
                name: "EmployeeCareName",
                table: "CustomerReturns");

            migrationBuilder.DropColumn(
                name: "EmployeeSellName",
                table: "CustomerReturns");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CustomerReturns");
        }
    }
}
