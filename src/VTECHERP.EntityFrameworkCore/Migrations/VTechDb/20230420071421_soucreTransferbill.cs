using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class soucreTransferbill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromDraft",
                table: "WarehouseTransferBills");

            migrationBuilder.AddColumn<int>(
                name: "CreatedFrom",
                table: "WarehouseTransferBills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedFrom",
                table: "WarehouseTransferBills");

            migrationBuilder.AddColumn<bool>(
                name: "IsFromDraft",
                table: "WarehouseTransferBills",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
