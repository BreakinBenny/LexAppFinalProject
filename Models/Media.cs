using System.ComponentModel.DataAnnotations;

namespace FinalProjectforSeptember.Models
{
	public class Media
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		[Display(Name = "Release Year")]
		public string? Date { get; set; }
		public string[]? Actors { get; set; }
		[Display(Name = "Age Group(s)")]
		public string? AgeGroup { get; set; }
		public string[]? Reviews { get; set; }
	}
	public class Movie : Media
	{
		[Display(Name = "Runtime (in minutes)")]
		public byte? Runtime { get; set; }	// IN MINUTES!
	}
	public class TVShow : Media
	{
		public byte? Seasons { get; set; }
		[Display(Name = "Episodes per season")]
		public byte? EpisodesPerSeason { get; set; }
	}
}