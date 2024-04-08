using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reddit_App.Migrations
{
    public partial class fixDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostID",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "TagID",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagID",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "PostID",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
