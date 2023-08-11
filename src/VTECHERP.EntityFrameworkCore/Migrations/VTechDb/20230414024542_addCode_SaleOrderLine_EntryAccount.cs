using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addCode_SaleOrderLine_EntryAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "EntryAccountNumbers");

            migrationBuilder.CreateSequence<int>(
                name: "SaleOrderLineNumbers");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SaleOrderLines",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR SaleOrderLineNumbers,'0000000000')");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "EntryAccounts",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR EntryAccountNumbers,'0000000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "EntryAccountNumbers");

            migrationBuilder.DropSequence(
                name: "SaleOrderLineNumbers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "EntryAccounts");
        }
    }
}
