using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_supplier_order_report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedBeforeCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedFinalCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtBeforeCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtFinalCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryBeforeCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFinalCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidCNY",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmDate",
                table: "SaleOrderLineConfirms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSurplus",
                table: "SaleOrderLineConfirms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequestQuantity",
                table: "SaleOrderLineConfirms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "SaleOrderLineConfirms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "SurplusQuantity",
                table: "SaleOrderLineConfirms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedImportedQuantity",
                table: "SaleOrderLineConfirms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupplierOrderReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebtBeforeCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidBeforeCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAccumulatedCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidFinalCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtFinalCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InDeliveryBeforeCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InDeliveryFinalCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedBeforeCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedFinalCNY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtBeforeVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidBeforeVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAccumulatedVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidFinalVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtFinalVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InDeliveryBeforeVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InDeliveryFinalVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedBeforeVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmedFinalVND = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_SupplierOrderReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierOrderReports");

            migrationBuilder.DropColumn(
                name: "ConfirmedBeforeCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmedFinalCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DebtBeforeCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DebtFinalCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryBeforeCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryFinalCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "PaidCNY",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmDate",
                table: "SaleOrderLineConfirms");

            migrationBuilder.DropColumn(
                name: "IsSurplus",
                table: "SaleOrderLineConfirms");

            migrationBuilder.DropColumn(
                name: "RequestQuantity",
                table: "SaleOrderLineConfirms");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SaleOrderLineConfirms");

            migrationBuilder.DropColumn(
                name: "SurplusQuantity",
                table: "SaleOrderLineConfirms");

            migrationBuilder.DropColumn(
                name: "UpdatedImportedQuantity",
                table: "SaleOrderLineConfirms");
        }
    }
}
