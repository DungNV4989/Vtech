using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class modify_promotion_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApplyWithDiscount",
                table: "Promotions",
                newName: "NotApplyWithDiscount");

            migrationBuilder.AddColumn<int>(
                name: "MaxDiscountUnit",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountValue",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxDiscountUnit",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDiscountUnit",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxDiscountValue",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "MaxDiscountUnit",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "NotApplyWithDiscount",
                table: "Promotions",
                newName: "IsApplyWithDiscount");
        }
    }
}
