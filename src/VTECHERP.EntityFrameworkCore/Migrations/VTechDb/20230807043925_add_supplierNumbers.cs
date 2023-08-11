using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class add_supplierNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "SupplierNumbers");

            migrationBuilder.AlterColumn<string>(
                name: "Squence",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "CONCAT('NCC-',FORMAT(NEXT VALUE FOR SupplierNumbers,'0000000000'))",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "SupplierNumbers");

            migrationBuilder.AlterColumn<string>(
                name: "Squence",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "CONCAT('NCC-',FORMAT(NEXT VALUE FOR SupplierNumbers,'0000000000'))");
        }
    }
}
