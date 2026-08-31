using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistaProjektSeptember2026.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updated_database_with_AgeGroup_support : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Runtime = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Genres = table.Column<int>(type: "int", nullable: false),
                    Actors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgeGroup = table.Column<int>(type: "int", nullable: false),
                    Reviews = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TVShow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Seasons = table.Column<byte>(type: "tinyint", nullable: false),
                    EpisodesPerSeason = table.Column<byte>(type: "tinyint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Genres = table.Column<int>(type: "int", nullable: false),
                    Actors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgeGroup = table.Column<int>(type: "int", nullable: false),
                    Reviews = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVShow", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "TVShow");
        }
    }
}
