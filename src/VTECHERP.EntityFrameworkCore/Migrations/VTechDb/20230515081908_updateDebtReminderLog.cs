using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class updateDebtReminderLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "HandlerEmployeeId",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "HandlerEmployeeName",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "HandlerStoreId",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "HandlerStoreIds",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "HandlerStoreName",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "SupportEmployeeId",
                table: "DebtReminderLogs");

            migrationBuilder.DropColumn(
                name: "SupportEmployeeName",
                table: "DebtReminderLogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "DebtReminderLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HandlerEmployeeId",
                table: "DebtReminderLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlerEmployeeName",
                table: "DebtReminderLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HandlerStoreId",
                table: "DebtReminderLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlerStoreIds",
                table: "DebtReminderLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlerStoreName",
                table: "DebtReminderLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupportEmployeeId",
                table: "DebtReminderLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportEmployeeName",
                table: "DebtReminderLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
