using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_warehousing_bill_table_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseTransferBillId",
                table: "WarehousingBills",
                newName: "SourceId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFromOrderConfirmation",
                table: "WarehousingBills",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromWarehouseTransfer",
                table: "WarehousingBills",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromOrderConfirmation",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "IsFromWarehouseTransfer",
                table: "WarehousingBills");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "WarehousingBills",
                newName: "WarehouseTransferBillId");
        }
    }
}
