using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.AgencyDb
{
    public partial class update_role_add_creator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AbpRoles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "False");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AbpRoles",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AbpRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AbpRoles",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "False",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
