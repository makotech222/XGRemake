using Microsoft.EntityFrameworkCore.Migrations;

namespace Xenogears.Database.Migrations
{
    public partial class GameState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameState", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GameState",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "GameStart" },
                    { 2, "Lahan_TalkedToDanOutside" },
                    { 3, "Lahan_TalkedToAlice" },
                    { 4, "UzukiHouse_TalkedToCitan" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameState");
        }
    }
}
