using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LikeCommentDetail_IdComment",
                table: "LikeCommentDetail",
                column: "IdComment");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeCommentDetail_Comment_IdComment",
                table: "LikeCommentDetail",
                column: "IdComment",
                principalTable: "Comment",
                principalColumn: "Id");
        }
    }
}
