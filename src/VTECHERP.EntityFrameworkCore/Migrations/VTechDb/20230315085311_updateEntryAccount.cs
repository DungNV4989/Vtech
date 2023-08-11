using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updateEntryAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "IsDebtAccount",
                table: "EntryAccounts");

            migrationBuilder.AddColumn<string>(
                name: "DocumentCode",
                table: "EntryAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "EntryAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "EntryAccounts",
                type: "nvarchar(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentCode",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "PayableAccountId",
                table: "EntryAccounts");

            migrationBuilder.DropColumn(
                name: "ReceivableAccountId",
                table: "EntryAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "EntryAccounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDebtAccount",
                table: "EntryAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
