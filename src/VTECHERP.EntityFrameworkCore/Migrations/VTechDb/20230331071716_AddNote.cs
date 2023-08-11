using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class AddNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "WarehousingBillProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "WarehousingBillProducts");
        }
    }
}
