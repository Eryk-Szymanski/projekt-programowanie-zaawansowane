using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    public partial class init22111444 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_AspNetUsers_UserId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_UserId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Exercise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Exercise",
                type: "nvarchar(450)",
                nullable: true);

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
        }
    }
}
