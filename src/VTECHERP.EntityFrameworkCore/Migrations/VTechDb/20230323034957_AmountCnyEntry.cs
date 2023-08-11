using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class AmountCnyEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "EntryAccounts",
                newName: "AmountVnd");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCny",
                table: "EntryAccounts",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountCny",
                table: "EntryAccounts");

            migrationBuilder.RenameColumn(
                name: "AmountVnd",
                table: "EntryAccounts",
                newName: "Amount");
        }
    }
}
