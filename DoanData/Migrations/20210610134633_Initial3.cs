using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                nullable: true,
                defaultValue: "10-06-2021 20:46:33",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "10-06-2021 14:48:59");

            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "Notification",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "LoginExternal",
                table: "Notification",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Notification",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Watched",
                table: "Notification",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_FromUserId",
                table: "Notification",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_FromUserId",
                table: "Notification",
                column: "FromUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_FromUserId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_FromUserId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "LoginExternal",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Watched",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "10-06-2021 14:48:59",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "10-06-2021 20:46:33");
        }
    }
}
