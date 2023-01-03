using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    public partial class init2211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SE",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Exercise",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SE_UserId",
                table: "SE",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_UserId",
                table: "Exercise",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_AspNetUsers_UserId",
                table: "Exercise",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SE_AspNetUsers_UserId",
                table: "SE",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_AspNetUsers_UserId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_SE_AspNetUsers_UserId",
                table: "SE");

            migrationBuilder.DropIndex(
                name: "IX_SE_UserId",
                table: "SE");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_UserId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SE");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Exercise");
        }
    }
}
