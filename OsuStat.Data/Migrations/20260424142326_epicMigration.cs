using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuStat.Data.Migrations
{
    /// <inheritdoc />
    public partial class epicMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeatmapHash",
                table: "Beatmaps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeatmapHash",
                table: "Beatmaps");
        }
    }
}
