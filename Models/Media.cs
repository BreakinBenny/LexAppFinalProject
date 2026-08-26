using System.ComponentModel.DataAnnotations;

namespace SistaProjektSeptember2026.Models
{
	[Flags]
	public enum Genre : ushort
	{
		NoGenre = 0,
		Action = 1,	// 1 << 0
		[Display(Name = "Äventyr")]
		Adventure = 2,	// 1 << 1
		[Display(Name = "Animerad")]
		Animated = 4,	// 1 << 2
		Anime = 8,	// 1 << 3
		[Display(Name = "Julprogram")]
		Christmas = 16,	// 1 << 4
		[Display(Name = "Komedi")]
		Comedy = 32,	// 1 << 5
		[Display(Name = "Kriminal")]
		Criminal = 64,	// 1 << 6
		Drama = 128,	// 1 << 7
		[Display(Name = "Familj")]
		Family = 256,	// 1 << 8
		[Display(Name = "Historia")]
		History = 512,	// 1 << 9
		[Display(Name = "Skräck")]
		Horror = 1024,	// 1 << 10
		[Display(Name = "Mysterium")]
		Mystery = 2048,	// 1 << 11
		[Display(Name = "Romantik")]
		Romance = 4096,	// 1 << 12
		[Display(Name = "Sci-Fi")]
		SciFi = 8192,	// 1 << 13
		Thriller = 16384 // 1 << 14
	}
	
	public class Media
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "Titel")]
		public string Title { get; set; }
		[Display(Name = "Lanseringsår")]
		public string? Year { get; set; }
		[Display(Name = "Genre(r)")]
		public Genre Genres { get; set; } = Genre.NoGenre;
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