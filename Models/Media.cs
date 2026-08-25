using System.ComponentModel.DataAnnotations;

[Flags]
enum Genre : ushort
{
	NoGenre = 0,
	Action = 1,
	[Display(Name = "Äventyr")]
	Adventure = 2,
	[Display(Name = "Animerad")]
	Animated = 4,
	Anime = 8,
	[Display(Name = "Julprogram")]
	Christmas = 16,
	[Display(Name = "Komedi")]
	Comedy = 32,
	[Display(Name = "Kriminal")]
	Criminal = 64,
	Drama = 128,
	[Display(Name = "Historia")]
	History = 256,
	[Display(Name = "Skräck")]
	Horror = 512,
	[Display(Name = "Mysterium")]
	Mystery = 1024,
	[Display(Name = "Romantik")]
	Romance = 2048,
	[Display(Name = "Sci-Fi")]
	SciFi = 4096,
	Thriller = 8092
}

namespace SistaProjektSeptember2026.Models
{
	public class Media
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "Titel")]
		public string Title { get; set; }
		[Display(Name = "Lanseringsår")]
		public string? Year { get; set; }
		[Display(Name = "Genre(r)")]
		public string? Genres { get; set; }
		[Display(Name = "Medverkande")]
		public string[]? Actors { get; set; }
		[Display(Name = "Åldersgrupp(er)")]
		public string? AgeGroup { get; set; }
		[Display(Name = "Recensioner")]
		public string[]? Reviews { get; set; }
	}
	public class Movie : Media
	{
		[Display(Name = "Längd (i minuter)")]
		public short? Runtime { get; set; }	// IN MINUTES!
	}
	public class TVShow : Media
	{
		[Display(Name = "Säsonger")]
		public byte Seasons { get; set; } = 1;
		[Display(Name = "Avsnitt per säsong")]
		public byte? EpisodesPerSeason { get; set; }
	}
}