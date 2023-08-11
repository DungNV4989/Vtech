using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addCodeEntryLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "EntryLogNumbers");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "EntryLogs",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR EntryLogNumbers,'0000000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "EntryLogNumbers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "EntryLogs");
        }
    }
}
