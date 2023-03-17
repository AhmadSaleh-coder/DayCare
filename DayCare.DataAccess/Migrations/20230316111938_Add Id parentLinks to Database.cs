using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayCare.DataAccess.Migrations
{
    public partial class AddIdparentLinkstoDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ParentLinks",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ParentLinks",
                newName: "id");
        }
    }
}
