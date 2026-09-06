using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;

namespace SistaProjektSeptember2026.Views.TVShows
{
	public class CreateTVShowModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public OMDbResponseTVShow? OMDbFetch { get; set; }
		public async Task<IActionResult> OnGetAsync(string apikey, string t)
		{
			OMDbFetch = new OMDbResponseTVShow();
			if (!string.IsNullOrWhiteSpace(apikey) || !string.IsNullOrWhiteSpace(t))
			{
				var client = _httpClientFactory.CreateClient();
				var url = $"http://www.omdbapi.com/?apikey={apikey}&t={t}";

				OMDbFetch = await client.GetFromJsonAsync<OMDbResponseTVShow>(url);
				if (OMDbFetch.imdbID != null)
					Console.WriteLine($"\nKLART!\nHär har du din TV-serie med ID {OMDbFetch.imdbID}! :-)");

				return RedirectToPage("./Index");
			}
			Console.WriteLine("Kör på CSHTML.CS");
			return Page();
		}
/*			
		// GET: TVSHOWS/OMDb?apikey=YOUR_API_KEY&t=TITLE
		[HttpGet("omdb")]
		public async Task<ActionResult<TVShow>> GetTVShowFromOMDb([FromQuery] string apikey, [FromQuery] string title, [FromQuery] bool save = false)
		{
			if (string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(title))
				return BadRequest("API-nyckel eller TV-seriens titel saknas!");

			var client = _httpClientFactory.CreateClient();
			var url = $"http://www.omdbapi.com/?apikey={Uri.EscapeDataString(apikey)}&t={Uri.EscapeDataString(title)}";
			using var resp = await client.GetAsync(url);
			if (!resp.IsSuccessStatusCode)
				return StatusCode((int)resp.StatusCode, "Felmeddelande från OMDb.");

			var json = await resp.Content.ReadAsStringAsync();
			var omdb = JsonSerializer.Deserialize<OMDbResponseTVShow>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			if (omdb == null || string.Equals(omdb.Response, "False", StringComparison.OrdinalIgnoreCase))
				return BadRequest(omdb?.Error ?? "Ingen data från OMDb.");
			if (omdb.Type != "series")
				return BadRequest($"{omdb.Title} är inte en TV-serie!");

			var OMDbTVShow = new TVShow
			{
				Title = omdb.Title,
				AgeGroup = AgeGroupParse(omdb.Rated),
				Genres = GenreParse(omdb.Genre),
				Director = omdb.Director,
				Actors = omdb.Actors,
				Year = TryParseYear(omdb.Year),
				Seasons = int.Parse(omdb.totalSeasons)
			};
			Console.WriteLine($"\nKLART!\nHär har du din TV-serie med ID {omdb.imdbID}! :-)");

			if (save) { _context.TVShow.Add(OMDbTVShow); await _context.SaveChangesAsync(); }

			return Ok(OMDbTVShow);
		}
		*/
	}
}