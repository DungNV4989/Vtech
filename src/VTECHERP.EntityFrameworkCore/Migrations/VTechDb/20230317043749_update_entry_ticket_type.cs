using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_entry_ticket_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "Entries",
                newName: "TicketType");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "Debts",
                newName: "TicketType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketType",
                table: "Entries",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "TicketType",
                table: "Debts",
                newName: "DocumentType");
        }
    }
}
