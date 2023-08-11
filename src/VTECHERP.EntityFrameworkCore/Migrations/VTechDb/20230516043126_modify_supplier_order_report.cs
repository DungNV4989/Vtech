using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class modify_supplier_order_report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedBeforeCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedBeforeVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAccumulatedVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedBeforeCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedBeforeVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedFinalVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DebtBeforeCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DebtBeforeVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DebtCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DebtFinalVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DebtVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DeliveryCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "DeliveryVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "InDeliveryBeforeCNY",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "InDeliveryBeforeVND",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "IsDeliveryFinalize",
                table: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "IsPaidFinalize",
                table: "SupplierOrderReports");

            migrationBuilder.RenameColumn(
                name: "PaidVND",
                table: "SupplierOrderReports",
                newName: "PaidSinceLastReportCNY");

            migrationBuilder.RenameColumn(
                name: "PaidFinalVND",
                table: "SupplierOrderReports",
                newName: "PaidInDateCNY");

            migrationBuilder.RenameColumn(
                name: "PaidFinalCNY",
                table: "SupplierOrderReports",
                newName: "DeliveryToDateCNY");

            migrationBuilder.RenameColumn(
                name: "PaidCNY",
                table: "SupplierOrderReports",
                newName: "DeliveryInDateCNY");

            migrationBuilder.RenameColumn(
                name: "PaidBeforeVND",
                table: "SupplierOrderReports",
                newName: "DeliveryFinalCNY");

            migrationBuilder.RenameColumn(
                name: "PaidBeforeCNY",
                table: "SupplierOrderReports",
                newName: "DebtToDateCNY");

            migrationBuilder.RenameColumn(
                name: "PaidAccumulatedVND",
                table: "SupplierOrderReports",
                newName: "DebtInDateCNY");

            migrationBuilder.RenameColumn(
                name: "PaidAccumulatedCNY",
                table: "SupplierOrderReports",
                newName: "ConfirmedToDateCNY");

            migrationBuilder.RenameColumn(
                name: "InDeliveryFinalVND",
                table: "SupplierOrderReports",
                newName: "ConfirmedSinceLastReportCNY");

            migrationBuilder.RenameColumn(
                name: "InDeliveryFinalCNY",
                table: "SupplierOrderReports",
                newName: "ConfirmedInDateCNY");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidSinceLastReportCNY",
                table: "SupplierOrderReports",
                newName: "PaidVND");

            migrationBuilder.RenameColumn(
                name: "PaidInDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidFinalVND");

            migrationBuilder.RenameColumn(
                name: "DeliveryToDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidFinalCNY");

            migrationBuilder.RenameColumn(
                name: "DeliveryInDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidCNY");

            migrationBuilder.RenameColumn(
                name: "DeliveryFinalCNY",
                table: "SupplierOrderReports",
                newName: "PaidBeforeVND");

            migrationBuilder.RenameColumn(
                name: "DebtToDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidBeforeCNY");

            migrationBuilder.RenameColumn(
                name: "DebtInDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidAccumulatedVND");

            migrationBuilder.RenameColumn(
                name: "ConfirmedToDateCNY",
                table: "SupplierOrderReports",
                newName: "PaidAccumulatedCNY");

            migrationBuilder.RenameColumn(
                name: "ConfirmedSinceLastReportCNY",
                table: "SupplierOrderReports",
                newName: "InDeliveryFinalVND");

            migrationBuilder.RenameColumn(
                name: "ConfirmedInDateCNY",
                table: "SupplierOrderReports",
                newName: "InDeliveryFinalCNY");

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedBeforeCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedBeforeVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedAccumulatedVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedBeforeCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedBeforeVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedFinalVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtBeforeCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtBeforeVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtFinalVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InDeliveryBeforeCNY",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InDeliveryBeforeVND",
                table: "SupplierOrderReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryFinalize",
                table: "SupplierOrderReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidFinalize",
                table: "SupplierOrderReports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
