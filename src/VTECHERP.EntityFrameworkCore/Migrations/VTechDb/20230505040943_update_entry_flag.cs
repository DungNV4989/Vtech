using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_entry_flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromBillCustomer",
                table: "WarehousingBills",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromCustomerReturn",
                table: "WarehousingBills",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AudienceId",
                table: "Debts",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromBillCustomer",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "IsFromCustomerReturn",
                table: "WarehousingBills");

            migrationBuilder.DropColumn(
                name: "AudienceId",
                table: "Debts");
        }
    }
}
