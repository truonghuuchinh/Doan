using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                nullable: true,
                defaultValue: "10-06-2021 14:46:43",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "00/00/00 00:0:00");

            migrationBuilder.AddColumn<string>(
                name: "AvartarUser",
                table: "Notification",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoterImg",
                table: "Notification",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "Notification",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_VideoId",
                table: "Notification",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Video_VideoId",
                table: "Notification",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Video_VideoId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_VideoId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "AvartarUser",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "PoterImg",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "00/00/00 00:0:00",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "10-06-2021 14:46:43");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
