using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "LikeVideoDetail");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "Comment");

            migrationBuilder.CreateTable(
                name: "LikeCommentDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reaction = table.Column<string>(nullable: true, defaultValue: "NoAction"),
                    UserId = table.Column<int>(nullable: false),
                    VideoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeCommentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeCommentDetail_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikeCommentDetail_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeCommentDetail_UserId",
                table: "LikeCommentDetail",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeCommentDetail_VideoId",
                table: "LikeCommentDetail",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeCommentDetail");

            migrationBuilder.AddColumn<string>(
                name: "CreateDate",
                table: "LikeVideoDetail",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "00/00/00 00:00:00");

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "NoAction");
        }
    }
}
