using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    NumOfSeries = table.Column<int>(type: "int", nullable: false),
                    NumOfReps = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SE_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercise",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SE_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SE_ExerciseId",
                table: "SE",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_SE_SessionId",
                table: "SE",
                column: "SessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SE");
        }
    }
}
