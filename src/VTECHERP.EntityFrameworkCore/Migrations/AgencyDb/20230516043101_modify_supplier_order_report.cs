using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
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

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PriceTables");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "PriceTables");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PriceTables");

            migrationBuilder.DropColumn(
                name: "CreatedFrom",
                table: "DraftTickets");

            migrationBuilder.DropColumn(
                name: "TransferStatus",
                table: "DraftTickets");

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

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PriceTables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasCod",
                table: "Debts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Debts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlerStoreIds",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountCodeBanking",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterDiscount",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCustomerPay",
                table: "BillCustomers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoteForProductBonus",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceTableCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTableCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceTableStore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTableStore", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceTableCustomer");

            migrationBuilder.DropTable(
                name: "PriceTableStore");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PriceTables");

            migrationBuilder.DropColumn(
                name: "HasCod",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "HandlerStoreIds",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "AccountCodeBanking",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "AmountAfterDiscount",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "AmountCustomerPay",
                table: "BillCustomers");

            migrationBuilder.DropColumn(
                name: "NoteForProductBonus",
                table: "BillCustomers");

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

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "PriceTables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                table: "PriceTables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "PriceTables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedFrom",
                table: "DraftTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransferStatus",
                table: "DraftTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
