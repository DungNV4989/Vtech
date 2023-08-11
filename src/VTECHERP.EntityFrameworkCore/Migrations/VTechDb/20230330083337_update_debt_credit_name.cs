using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_debt_credit_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivableAccountCode",
                table: "EntryAccounts",
                newName: "DebtAccountCode");

            migrationBuilder.RenameColumn(
                name: "PayableAccountCode",
                table: "EntryAccounts",
                newName: "CreditAccountCode");

            migrationBuilder.RenameColumn(
                name: "Receivable",
                table: "Debts",
                newName: "Debts");

            migrationBuilder.RenameColumn(
                name: "Payable",
                table: "Debts",
                newName: "Credits");

            migrationBuilder.RenameColumn(
                name: "PeriodReceivable",
                table: "DebtReports",
                newName: "PeriodDebt");

            migrationBuilder.RenameColumn(
                name: "PeriodPayable",
                table: "DebtReports",
                newName: "PeriodCredit");

            migrationBuilder.RenameColumn(
                name: "EndOfPeriodReceivable",
                table: "DebtReports",
                newName: "EndOfPeriodDebt");

            migrationBuilder.RenameColumn(
                name: "EndOfPeriodPayable",
                table: "DebtReports",
                newName: "EndOfPeriodCredit");

            migrationBuilder.RenameColumn(
                name: "BeginOfPeriodReceivable",
                table: "DebtReports",
                newName: "BeginOfPeriodDebt");

            migrationBuilder.RenameColumn(
                name: "BeginOfPeriodPayable",
                table: "DebtReports",
                newName: "BeginOfPeriodCredit");

            migrationBuilder.AddColumn<Guid>(
                name: "AudienceId",
                table: "Entries",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceId",
                table: "Entries");

            migrationBuilder.RenameColumn(
                name: "DebtAccountCode",
                table: "EntryAccounts",
                newName: "ReceivableAccountCode");

            migrationBuilder.RenameColumn(
                name: "CreditAccountCode",
                table: "EntryAccounts",
                newName: "PayableAccountCode");

            migrationBuilder.RenameColumn(
                name: "Debts",
                table: "Debts",
                newName: "Receivable");

            migrationBuilder.RenameColumn(
                name: "Credits",
                table: "Debts",
                newName: "Payable");

            migrationBuilder.RenameColumn(
                name: "PeriodDebt",
                table: "DebtReports",
                newName: "PeriodReceivable");

            migrationBuilder.RenameColumn(
                name: "PeriodCredit",
                table: "DebtReports",
                newName: "PeriodPayable");

            migrationBuilder.RenameColumn(
                name: "EndOfPeriodDebt",
                table: "DebtReports",
                newName: "EndOfPeriodReceivable");

            migrationBuilder.RenameColumn(
                name: "EndOfPeriodCredit",
                table: "DebtReports",
                newName: "EndOfPeriodPayable");

            migrationBuilder.RenameColumn(
                name: "BeginOfPeriodDebt",
                table: "DebtReports",
                newName: "BeginOfPeriodReceivable");

            migrationBuilder.RenameColumn(
                name: "BeginOfPeriodCredit",
                table: "DebtReports",
                newName: "BeginOfPeriodPayable");
        }
    }
}
