using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addcolAccountCodeBankingNoteForProductBonustabBillCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountCodeBanking",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoteForProductBonus",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCodeBanking",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "NoteForProductBonus",
                table: "BillCustomers");
        }
    }
}
