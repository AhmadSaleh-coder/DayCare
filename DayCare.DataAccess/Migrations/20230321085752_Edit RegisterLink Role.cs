using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayCare.DataAccess.Migrations
{
    public partial class EditRegisterLinkRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisterLinks_AspNetRoles_RoleId",
                table: "RegisterLinks");

            migrationBuilder.DropIndex(
                name: "IX_RegisterLinks_RoleId",
                table: "RegisterLinks");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "RegisterLinks");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "RegisterLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "RegisterLinks");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "RegisterLinks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterLinks_RoleId",
                table: "RegisterLinks",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisterLinks_AspNetRoles_RoleId",
                table: "RegisterLinks",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
