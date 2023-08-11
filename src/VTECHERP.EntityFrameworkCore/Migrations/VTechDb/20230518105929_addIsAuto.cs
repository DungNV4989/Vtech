using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addIsAuto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAuto",
                table: "TransportInfomations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuto",
                table: "TransportInfomations");
        }
    }
}
