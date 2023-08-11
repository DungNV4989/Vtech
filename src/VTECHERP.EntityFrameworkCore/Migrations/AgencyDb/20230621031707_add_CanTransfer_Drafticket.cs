﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class add_CanTransfer_Drafticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanTransfer",
                table: "DraftTicketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanTransfer",
                table: "DraftTicketProducts");
        }
    }
}
