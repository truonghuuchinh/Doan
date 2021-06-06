using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<int>(
                name: "Comment",
                table: "LikeCommentDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "LikeCommentDetail");

            migrationBuilder.AddColumn<int>(
                name: "IdComment",
                table: "LikeCommentDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
