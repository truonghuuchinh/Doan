using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                nullable: true,
                defaultValue: "10-06-2021 14:48:59",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "10-06-2021 14:46:43");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Notification",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "CreateDate",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "10-06-2021 14:46:43",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "10-06-2021 14:48:59");
        }
    }
}
