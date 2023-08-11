using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class rename_ColumninProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lenght",
                table: "Products",
                newName: "Length");

            migrationBuilder.RenameColumn(
                name: "BranchWhosalePrice",
                table: "Products",
                newName: "BranchWholeSalePrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Length",
                table: "Products",
                newName: "Lenght");

            migrationBuilder.RenameColumn(
                name: "BranchWholeSalePrice",
                table: "Products",
                newName: "BranchWhosalePrice");
        }
    }
}
