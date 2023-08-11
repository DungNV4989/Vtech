using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addColumetoDayConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDayAllowCreatePayRecieve",
                table: "DayConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDayAllowDeleteEntry",
                table: "DayConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDayAllowCreatePayRecieve",
                table: "DayConfigurations");

            migrationBuilder.DropColumn(
                name: "NumberOfDayAllowDeleteEntry",
                table: "DayConfigurations");
        }
    }
}
