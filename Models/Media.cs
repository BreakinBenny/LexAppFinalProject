using System.ComponentModel.DataAnnotations;

namespace SistaProjektSeptember2026.Models
{
	[Flags]
	public enum Genre
	{
		NoGenre = 0,
		Action = 1 << 0,
		[Display(Name = "Äventyr")]
		Adventure = 1 << 1,
		[Display(Name = "Animerad")]
		Animated = 1 << 2,
		Anime = 1 << 3,
		[Display(Name = "Julprogram")]
		Christmas = 1 << 4,
		[Display(Name = "Komedi")]
		Comedy = 1 << 5,
		[Display(Name = "Kriminal")]
		Criminal = 1 << 6,
		Drama = 1 << 7,
		[Display(Name = "Familj")]
		Family = 1 << 8,
		[Display(Name = "Historia")]
		History = 1 << 9,
		[Display(Name = "Skräck")]
		Horror = 1 << 10,
		[Display(Name = "Mysterium")]
		Mystery = 1 << 11,
		[Display(Name = "Romantik")]
		Romance = 1 << 12,
		[Display(Name = "Sci-Fi")]
		SciFi = 1 << 13,
		Thriller = 1 << 14,
		Fantasy = 1 << 15
	}

	[Flags]
	public enum AgeRating
	{
		[Display(Name = "Åldersgräns saknas")]
		Unknown = 0,
		[Display(Name = "Kan ses av alla åldrar")]
		AllAges = 1 << 0,
		[Display(Name = "Från 7 år")]
		Seven = 1 << 1,
		[Display(Name = "Från 11 år")]
		Eleven = 1 << 2,
		[Display(Name = "Från 15 år")]
		Fifteen = 1 << 3
	}
	
	public class Media
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "Titel")]
		public string Title { get; set; }
		[Display(Name = "Regi")]
		public string? Director { get; set; }
		[Display(Name = "Lanseringsår")]
		public int Year { get; set; }
		[Display(Name = "Genre(r)")]
		public Genre Genres { get; set; } = Genre.NoGenre;
		[Display(Name = "Medverkande")]
		public string? Actors { get; set; }
		[Display(Name = "Åldersgräns")]
		public AgeRating AgeGroup { get; set; }
		[Display(Name = "Recensioner")]
		public string[]? Reviews { get; set; }
	}
	public class Movie : Media
	{
		[Display(Name = "Längd (i minuter)")]
		public int Runtime { get; set; }	// IN MINUTES!
	}

	public class TVShow : Media
	{
		[Display(Name = "Säsonger")]
		public int Seasons { get; set; } = 1;
		[Display(Name = "Avsnitt per säsong")]
		public int EpisodesPerSeason { get; set; }
	}

	public class OMDbResponse {
		public string? imdbID { get; set; }
		public string? Title { get; set; }
		public string? Year { get; set; }
		public string? Genre { get; set; }
		public string? Actors { get; set; }
		public string? Poster { get; set; }
		public string? Plot { get; set; }
		public string? Director { get; set; }
		public string? Type { get; set; }   // "movie" eller "series", för att avvisa i händelse av att fel typ av media returneras
		public string? Rated { get; set; }
		public string? Response { get; set; }    // "True" eller "False"
		public string? Error { get; set; }   // ifall Response == "False"
	}
	public class OMDbResponseTVShow : OMDbResponse {
		public string totalSeasons { get; set; } = "1";
	}
	public class OMDbResponseMovie : OMDbResponse {
		public string? Runtime { get; set; }
	}
}