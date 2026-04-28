using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuStat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSessionParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SessionAccuracySum",
                table: "PlayerStats",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SessionBpmSum",
                table: "PlayerStats",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SessionStarRateSum",
                table: "PlayerStats",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionAccuracySum",
                table: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "SessionBpmSum",
                table: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "SessionStarRateSum",
                table: "PlayerStats");
        }
    }
}
