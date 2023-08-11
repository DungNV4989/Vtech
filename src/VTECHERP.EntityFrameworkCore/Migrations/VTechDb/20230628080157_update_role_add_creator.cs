using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class update_role_add_creator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateSequence<int>(
            //    name: "IdentityUserCode");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "MainStoreId",
            //    table: "AbpUsers",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            //migrationBuilder.AddColumn<string>(
            //    name: "UserCode",
            //    table: "AbpUsers",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    defaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AbpRoles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "False");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "CreatorId",
            //    table: "AbpRoles",
            //    type: "uniqueidentifier",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "IdentityUserCode");

            migrationBuilder.DropColumn(
                name: "MainStoreId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AbpRoles");

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
