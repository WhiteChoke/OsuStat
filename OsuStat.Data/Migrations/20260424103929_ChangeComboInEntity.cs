using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuStat.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeComboInEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_Date",
                table: "PlayerStats",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerStats_Date",
                table: "PlayerStats");
        }
    }
}
