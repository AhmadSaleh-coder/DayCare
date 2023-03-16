using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayCare.DataAccess.Migrations
{
    public partial class AddIsDeletedToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "isDeleted",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "AspNetUsers");
        }
    }
}
