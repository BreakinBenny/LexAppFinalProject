using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;

namespace SistaProjektSeptember2026.Views.Movies
{
	public class CreateModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public OMDbResponseMovie? OMDbFetch { get; set; }
		public async Task<IActionResult> OnGetAsync(string apikey, string t)
		{
			if (!string.IsNullOrWhiteSpace(apikey) || !string.IsNullOrWhiteSpace(t))
			{
				var client = _httpClientFactory.CreateClient();
				var url = $"http://www.omdbapi.com/?apikey={apikey}&t={t}";

				OMDbFetch = await client.GetFromJsonAsync<OMDbResponseMovie>(url);
				if (OMDbFetch.imdbID != null)
					Console.WriteLine($"\nKLART!\nHär har du din film med ID {OMDbFetch.imdbID}! :-)");
			}
			return Page();
		}

		public async Task<ActionResult<Movie>> GetMovieFromOMDb([FromQuery] string apikey, [FromQuery] string title, [FromQuery] bool save = false)
		{
			if (string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(title))
				return BadRequest("API-nyckel eller filmtitel saknas!");

			var client = _httpClientFactory.CreateClient();
			var url = $"http://www.omdbapi.com/?apikey={Uri.EscapeDataString(apikey)}&t={Uri.EscapeDataString(title)}";
			using var resp = await client.GetAsync(url);
			if (!resp.IsSuccessStatusCode)
				return StatusCode((int)resp.StatusCode, "Felmeddelande från OMDb.");

			var json = await resp.Content.ReadAsStringAsync();
			var omdb = JsonSerializer.Deserialize<OMDbResponseMovie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			if (omdb == null || string.Equals(omdb.Response, "False", StringComparison.OrdinalIgnoreCase))
				return BadRequest(omdb?.Error ?? "Ingen data från OMDb.");
			if (omdb.Type != "movie")
				return BadRequest($"{omdb.Title} är inte en film!");

			var OMDbFetch = new Movie
			{
				Title = omdb.Title,
				AgeGroup = AgeGroupParse(omdb.Rated),
				Genres = GenreParse(omdb.Genre),
				Director = omdb.Director,
				Actors = omdb.Actors,
				Year = TryParseYear(omdb.Year),
				Runtime = TryParseRuntimeMinutes(omdb.Runtime)
			};
			Console.WriteLine($"\nKLART!\nHär har du din film med ID {omdb.imdbID}! :-)");

			if (save) { _context.Movie.Add(OMDbFetch); await _context.SaveChangesAsync(); }

			return OMDbFetch;
		}

		private static AgeRating AgeGroupParse(string rated) {
			if (string.IsNullOrWhiteSpace(rated))
				return AgeRating.Unknown;

			rated = rated.Trim().ToUpperInvariant();

			return rated switch
			{
				"N/A" => AgeRating.Unknown,
				"G" => AgeRating.AllAges,
				"PG" => AgeRating.Seven,
				"TV-Y" => AgeRating.Seven,
				"PG-13" => AgeRating.Eleven,
				"R" => AgeRating.Fifteen

			};
		}
		private static readonly Dictionary<string, Genre> _genreMap = new(StringComparer.OrdinalIgnoreCase)	{
			{ "Action", Genre.Action },
			{ "Adventure", Genre.Adventure },
			{ "Animated", Genre.Animated },
			{ "Anime", Genre.Anime },
			{ "Christmas", Genre.Christmas },
			{ "Comedy", Genre.Comedy },
			{ "Criminal", Genre.Criminal },
			{ "Drama", Genre.Drama },
			{ "Family", Genre.Family },
			{ "History", Genre.History },
			{ "Horror", Genre.Horror },
			{ "Mystery", Genre.Mystery },
			{ "Romance", Genre.Romance },
			{ "SciFi", Genre.SciFi },
			{ "Thriller", Genre.Thriller },
			{ "Fantasy", Genre.Fantasy }
		};
		
		private static Genre GenreParse(string omdbGenreCsv) {
			if (string.IsNullOrWhiteSpace(omdbGenreCsv))
				return Genre.NoGenre;

			Genre result = Genre.NoGenre;
			var genreParts = omdbGenreCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			foreach (var part in genreParts)
			{
				if (_genreMap.TryGetValue(part, out var g))
				{
					result |= g;
					continue;
				}

				// Fallback, försöka normalisera och prova direkt parsing av enum (Finns mellanslag och bindestreck här? Nej tack!)
				var normalized = part.Replace("-", " ").Replace(" ", "");
				if (Enum.TryParse<Genre>(normalized, ignoreCase: true, out var parsed))
				{
					result |= parsed;
					continue;
				}
				// Okända genres ignorerar vi...
			}

			return result;
		}

		private static int TryParseYear(string year) {
			if (string.IsNullOrWhiteSpace(year)) return 0;  // Inget år funnet, returnera 0 som standardvärde!
			var parts = year.Split('–', '—'); // The year of release or premiere is relevant

			return int.TryParse(parts[0], out var y) ? y : 0;
		}
		private static int TryParseRuntimeMinutes(string runtime) {
			if (string.IsNullOrWhiteSpace(runtime)) return 0;
			var minutes = Regex.Match(runtime, @"(\d+)");
			return minutes.Success && int.TryParse(minutes.Groups[1].Value, out var mins) ? mins : 0; // Return 0 if parsing fails
		}
	}
}