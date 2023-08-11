using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addcolSourceIdtableTransportInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionSource",
                table: "TransportInfomations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "TransportInfomations",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionSource",
                table: "TransportInfomations");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "TransportInfomations");
        }
    }
}
