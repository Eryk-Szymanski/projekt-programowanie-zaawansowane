using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Wallet_SenderWalletId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_SenderWalletId",
                table: "Transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SenderWalletId",
                table: "Transaction",
                column: "SenderWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Wallet_SenderWalletId",
                table: "Transaction",
                column: "SenderWalletId",
                principalTable: "Wallet",
                principalColumn: "Id");
        }
    }
}
