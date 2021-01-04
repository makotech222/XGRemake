using Microsoft.EntityFrameworkCore.Migrations;

namespace Xenogears.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAnimations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    StartSprite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkipBetweenFiles = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAnimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterAnimations_Characters_CharacterName",
                        column: x => x.CharacterName,
                        principalTable: "Characters",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Characters",
                column: "Name",
                values: new object[]
                {
                    "Fei",
                    "Elly",
                    "Citan",
                    "Bart",
                    "Billy",
                    "Rico",
                    "Esmeralda",
                    "Chu-Chu",
                    "Maria",
                    "Adult Esmeralda"
                });

            migrationBuilder.InsertData(
                table: "CharacterAnimations",
                columns: new[] { "Id", "CharacterName", "Count", "Name", "SkipBetweenFiles", "StartSprite" },
                values: new object[,]
                {
                    { 1, "Fei", 1, "Idle", 0, "FieldPCSprites2\\003922_bin\\0000.png" },
                    { 18, "Citan", 4, "Climb", 0, "FieldPCSprites1\\000429_bin\\0100.png" },
                    { 17, "Citan", 6, "Jump", 0, "FieldPCSprites1\\000429_bin\\0000.png" },
                    { 16, "Citan", 8, "Run", 0, "FieldPCSprites1\\000429_bin\\0060.png" },
                    { 15, "Citan", 8, "Walk", 6, "FieldPCSprites1\\000429_bin\\0120.png" },
                    { 14, "Citan", 1, "Idle", 0, "FieldPCSprites2\\003923_bin\\0040.png" },
                    { 13, "Elly", 3, "Action1", 1, "FieldPCSprites2\\003923_bin\\0000.png" },
                    { 12, "Elly", 6, "Jump", 0, "FieldPCSprites1\\000428_bin\\0000.png" },
                    { 11, "Elly", 8, "Run", 0, "FieldPCSprites1\\000428_bin\\0150.png" },
                    { 10, "Elly", 8, "Walk", 0, "FieldPCSprites1\\000428_bin\\0055.png" },
                    { 9, "Elly", 1, "Idle", 3, "FieldPCSprites2\\003923_bin\\0003.png" },
                    { 8, "Fei", 4, "Action3", 7, "FieldPCSprites1\\000427_bin\\0152.png" },
                    { 7, "Fei", 3, "Action2", 8, "FieldPCSprites1\\000427_bin\\0149.png" },
                    { 6, "Fei", 4, "Action1", 7, "FieldPCSprites1\\000427_bin\\0145.png" },
                    { 5, "Fei", 4, "Climb", 0, "FieldPCSprites1\\000427_bin\\0005.png" },
                    { 4, "Fei", 8, "Jump", 0, "FieldPCSprites1\\000427_bin\\0105.png" },
                    { 3, "Fei", 8, "Run", 0, "FieldPCSprites1\\000427_bin\\0065.png" },
                    { 2, "Fei", 8, "Walk", 0, "FieldPCSprites1\\000427_bin\\0025.png" },
                    { 19, "Citan", 4, "Action1", 0, "FieldPCSprites1\\000429_bin\\0040.png" },
                    { 20, "Citan", 6, "Action2", 8, "FieldPCSprites1\\000429_bin\\0128.png" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAnimations_CharacterName",
                table: "CharacterAnimations",
                column: "CharacterName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAnimations");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
