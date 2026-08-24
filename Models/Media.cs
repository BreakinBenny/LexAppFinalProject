using System.ComponentModel.DataAnnotations;

namespace SistaProjekt_September2026.Models
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
		public byte? Runtime { get; set; }	// IN MINUTES!
	}
	public class TVShow : Media
	{
		[Display(Name = "Säsonger")]
		public byte Seasons { get; set; } = 1;
		[Display(Name = "Avsnitt per säsong")]
		public byte? EpisodesPerSeason { get; set; }
	}
}