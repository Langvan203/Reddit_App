using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reddit_App.Migrations
{
    public partial class updateDB_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LikeStatus",
                table: "Likes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeStatus",
                table: "Likes");
        }
    }
}
