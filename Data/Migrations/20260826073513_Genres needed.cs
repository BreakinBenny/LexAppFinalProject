using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistaProjektSeptember2026.Data.Migrations
{
	/// <inheritdoc />
	public partial class Genresneeded : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<byte>(
				name: "Seasons",
				table: "TVShow",
				type: "tinyint",
				nullable: false,
				defaultValue: (byte)0,
				oldClrType: typeof(byte),
				oldType: "tinyint",
				oldNullable: true);

			migrationBuilder.AlterColumn<short>(
				name: "Runtime",
				table: "Movie",
				type: "smallint",
				nullable: true,
				oldClrType: typeof(byte),
				oldType: "tinyint",
				oldNullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<byte>(
				name: "Seasons",
				table: "TVShow",
				type: "tinyint",
				nullable: true,
				oldClrType: typeof(byte),
				oldType: "tinyint");

			migrationBuilder.AlterColumn<byte>(
				name: "Runtime",
				table: "Movie",
				type: "tinyint",
				nullable: true,
				oldClrType: typeof(short),
				oldType: "smallint",
				oldNullable: true);
		}
	}
}
