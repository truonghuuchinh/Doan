using Microsoft.EntityFrameworkCore.Migrations;

namespace DoanData.Migrations
{
    public partial class Initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Action_ActionId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_ActionId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "UserRole");

            migrationBuilder.AddColumn<int>(
                name: "appUserId",
                table: "UserRole",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_appUserId",
                table: "UserRole",
                column: "appUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_appUserId",
                table: "UserRole",
                column: "appUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_appUserId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_appUserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "appUserId",
                table: "UserRole");

            migrationBuilder.AddColumn<int>(
                name: "ActionId",
                table: "UserRole",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreateDate",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "00/00/00 00:0:00");

            migrationBuilder.CreateTable(
                name: "Function",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionsId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Action_Function_FunctionsId",
                        column: x => x.FunctionsId,
                        principalTable: "Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_ActionId",
                table: "UserRole",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Action_FunctionsId",
                table: "Action",
                column: "FunctionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Action_ActionId",
                table: "UserRole",
                column: "ActionId",
                principalTable: "Action",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
