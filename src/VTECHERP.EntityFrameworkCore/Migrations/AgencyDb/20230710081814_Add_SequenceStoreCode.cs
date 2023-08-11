using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class Add_SequenceStoreCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "StoreNumbers");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "CONCAT('CH-',FORMAT(NEXT VALUE FOR StoreNumbers,'0000000000'))",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "StoreNumbers");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "CONCAT('CH-',FORMAT(NEXT VALUE FOR StoreNumbers,'0000000000'))");
        }
    }
}
