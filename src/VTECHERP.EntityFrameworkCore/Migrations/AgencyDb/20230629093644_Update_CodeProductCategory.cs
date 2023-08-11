using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class Update_CodeProductCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "CONCAT('DMSP-',FORMAT(NEXT VALUE FOR ProductCategoryNumbers,'0000000000'))",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "FORMAT(NEXT VALUE FOR ProductCategoryNumbers,'0000000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR ProductCategoryNumbers,'0000000000')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "CONCAT('DMSP-',FORMAT(NEXT VALUE FOR ProductCategoryNumbers,'0000000000'))");
        }
    }
}
