using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updateEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Audience",
                table: "Entries",
                newName: "AudienceType");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentType",
                table: "EntryAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AudienceCode",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceCode",
                table: "Entries");

            migrationBuilder.RenameColumn(
                name: "AudienceType",
                table: "Entries",
                newName: "Audience");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentType",
                table: "EntryAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
