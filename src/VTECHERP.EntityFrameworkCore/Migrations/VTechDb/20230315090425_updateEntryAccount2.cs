using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updateEntryAccount2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayableAccountId",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "ReceivableAccountId",
                table: "EntryAccounts");

            migrationBuilder.AddColumn<string>(
                name: "PayableAccountCode",
                table: "EntryAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivableAccountCode",
                table: "EntryAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayableAccountCode",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "ReceivableAccountCode",
                table: "EntryAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "PayableAccountId",
                table: "EntryAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceivableAccountId",
                table: "EntryAccounts",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
