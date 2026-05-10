using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuStat.Data.Migrations
{
    /// <inheritdoc />
    public partial class PpUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeatmapHash",
                table: "Beatmaps");

            migrationBuilder.AlterColumn<int>(
                name: "PpGained",
                table: "PlayerStats",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "DayInitialPp",
                table: "PlayerStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "OsuBeatmapId",
                table: "Beatmaps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayInitialPp",
                table: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "OsuBeatmapId",
                table: "Beatmaps");

            migrationBuilder.AlterColumn<double>(
                name: "PpGained",
                table: "PlayerStats",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "BeatmapHash",
                table: "Beatmaps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
