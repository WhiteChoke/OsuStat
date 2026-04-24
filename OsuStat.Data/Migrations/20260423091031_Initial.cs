using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuStat.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beatmaps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Artist = table.Column<string>(type: "TEXT", nullable: false),
                    Mapper = table.Column<string>(type: "TEXT", nullable: false),
                    Bpm = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<string>(type: "TEXT", nullable: false),
                    StarRate = table.Column<double>(type: "REAL", nullable: false),
                    Hp = table.Column<double>(type: "REAL", nullable: false),
                    Cs = table.Column<double>(type: "REAL", nullable: false),
                    Ar = table.Column<double>(type: "REAL", nullable: false),
                    BgPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayTimeMin = table.Column<double>(type: "REAL", nullable: false),
                    MapPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    PpGained = table.Column<double>(type: "REAL", nullable: false),
                    AvgBpm = table.Column<double>(type: "REAL", nullable: false),
                    AvgStarRate = table.Column<double>(type: "REAL", nullable: false),
                    AvgAccuracy = table.Column<double>(type: "REAL", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PpGained = table.Column<double>(type: "REAL", nullable: false),
                    Combo = table.Column<short>(type: "INTEGER", nullable: false),
                    Accuracy = table.Column<double>(type: "REAL", nullable: false),
                    Mods = table.Column<string>(type: "TEXT", nullable: false),
                    Grade = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BeatmapId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plays_Beatmaps_BeatmapId",
                        column: x => x.BeatmapId,
                        principalTable: "Beatmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plays_BeatmapId",
                table: "Plays",
                column: "BeatmapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "Plays");

            migrationBuilder.DropTable(
                name: "Beatmaps");
        }
    }
}
