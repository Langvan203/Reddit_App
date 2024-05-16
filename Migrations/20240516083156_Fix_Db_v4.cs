using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reddit_App.Migrations
{
    public partial class Fix_Db_v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Userss",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Userss");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Comments");
        }
    }
}
