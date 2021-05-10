using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "LikeVideoDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LikeVideoDetail_VideoId",
                table: "LikeVideoDetail",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeVideoDetail_Video_VideoId",
                table: "LikeVideoDetail",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikeVideoDetail_Video_VideoId",
                table: "LikeVideoDetail");

            migrationBuilder.DropIndex(
                name: "IX_LikeVideoDetail_VideoId",
                table: "LikeVideoDetail");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "LikeVideoDetail");
        }
    }
}
