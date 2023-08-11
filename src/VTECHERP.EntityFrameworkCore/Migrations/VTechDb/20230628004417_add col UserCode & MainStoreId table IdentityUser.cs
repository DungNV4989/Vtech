using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VTECHERP.Migrations.VTechDb
{
    public partial class addcolUserCodeMainStoreIdtableIdentityUser : Migration
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
            //    nullable: false,
            //    defaultValueSql: "FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");
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
        }
    }
}
