using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistaProjektSeptember2026.Data.Migrations
{
	/// <inheritdoc />
	public partial class Movies_and_TV_show_tables : Migration
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
					Runtime = table.Column<byte>(type: "tinyint", nullable: true),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Genres = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Actors = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AgeGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
					Seasons = table.Column<byte>(type: "tinyint", nullable: true),
					EpisodesPerSeason = table.Column<byte>(type: "tinyint", nullable: true),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Genres = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Actors = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AgeGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
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