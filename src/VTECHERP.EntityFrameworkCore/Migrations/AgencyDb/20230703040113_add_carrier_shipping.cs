using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class add_carrier_shipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarrierShippingCode",
                table: "TransportInfomations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrierShippingCode",
                table: "BillCustomers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarrierShippingInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransportInformationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderProvinceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderDistrictName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderWard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderWardName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverProvinceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverDistrictName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverWard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverWardName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VTShippingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarrierShippingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsCOD = table.Column<bool>(type: "bit", nullable: true),
                    CODValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPayingShippingFee = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CarrierShippingInformation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrierShippingInformation");

            migrationBuilder.DropColumn(
                name: "CarrierShippingCode",
                table: "TransportInfomations");

            migrationBuilder.DropColumn(
                name: "CarrierShippingCode",
                table: "BillCustomers");
        }
    }
}
