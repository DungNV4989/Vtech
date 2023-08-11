using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_table_warehouse_bill_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceCode",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "AudienceName",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "AudiencePhone",
                table: "WarehousingBills");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudienceCode",
                table: "WarehousingBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudienceName",
                table: "WarehousingBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudiencePhone",
                table: "WarehousingBills",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
