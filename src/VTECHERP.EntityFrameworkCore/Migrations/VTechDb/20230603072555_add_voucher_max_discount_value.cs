using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_voucher_max_discount_value : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplyProducIds",
                table: "Promotions",
                newName: "ApplyProductIds");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountValue",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDiscountValue",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "ApplyProductIds",
                table: "Promotions",
                newName: "ApplyProducIds");
        }
    }
}
