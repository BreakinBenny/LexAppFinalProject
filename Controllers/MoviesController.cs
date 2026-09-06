//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;
using System.Text.RegularExpressions;

//[Route("/[controller]")]
//[ApiController]
public class MoviesController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IHttpClientFactory _httpClientFactory;

	public MoviesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
	{
		_context = context;
		_httpClientFactory = httpClientFactory;
	}

	// GET: MOVIES
	public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString)
	{
		ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "TitleDesc" : "";
		ViewData["DateSortParm"] = sortOrder == "Year" ? "YearDesc" : "Year";
		ViewData["CurrentFilter"] = searchString;
		var movies = from movie in _context.Movie select movie;
		if (!String.IsNullOrEmpty(searchString))
		{
			searchString.ToUpper();
			movies = movies.Where(show => show.Title.ToUpper().Contains(searchString));
		}

		switch (sortOrder)
		{
			case "TitleDesc":
				movies = movies.OrderByDescending(show => show.Title);
				break;
			case "Year":
				movies = movies.OrderBy(show => show.Year);
				break;
			case "YearDesc":
				movies = movies.OrderByDescending(show => show.Year);
				break;
			default:
				movies = movies.OrderBy(show => show.Title);
				break;
		}

		if (_context == null)
			return NotFound();
		
		return View(await movies.AsNoTracking().ToListAsync());
	}

	// GET: MOVIES/Details/5
	[HttpGet]
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
			return NotFound();

		var movie = await _context.Movie
			.FirstOrDefaultAsync(m => m.Id == id);
		if (movie == null)
			return NotFound();
		/*
		string ReviewsString = null;
		if (movie.Reviews != null)
		{
			foreach (var Review in movie.Reviews)
				ReviewsString += string.Join(", ", movie.Reviews);
		}
		*/
		return View(movie);
	}

	// GET: MOVIES/Create
	//[Authorize(Roles = "Administrator")]
	[HttpGet]
	public async Task<IActionResult> Create([FromQuery] string apikey, [FromQuery] string t)
	{
		if (!string.IsNullOrWhiteSpace(apikey) && !string.IsNullOrWhiteSpace(t))
		{
			var client = _httpClientFactory.CreateClient();
			var url = $"http://www.omdbapi.com/?apikey={apikey}&t={t}";

			Console.WriteLine("API-nyckel och filmtitel angavs, söker efter film...");
			var OMDbMovie = await client.GetFromJsonAsync<OMDbResponseMovie>(url);
			if (OMDbMovie != null && OMDbMovie.imdbID != null && bool.Parse(OMDbMovie.Response))
			{
				Console.WriteLine($"Hittade {OMDbMovie.Title} på IMDB! ({OMDbMovie.imdbID})");
				if (OMDbMovie.Type != "movie") Console.Write($" ...{OMDbMovie.Title} är dock ingen film, men låt gå.");
			}
			else
			{
				Console.WriteLine($"Hittade ingen film som heter {OMDbMovie.Title}! Stavade du fel, eller vill du skriva titeln på dess ursprungsspråk om det finns?");
				return View();
			}

			var movie = new Movie {
				Title = OMDbMovie.Title,
				AgeGroup = AgeGroupParse(OMDbMovie.Rated),
				Genres = GenreParse(OMDbMovie.Genre),
				Director = OMDbMovie.Director,
				Actors = OMDbMovie.Actors,
				Year = TryParseYear(OMDbMovie.Year),
				Runtime = TryParseRuntimeMinutes(OMDbMovie.Runtime)
			};
			if(ModelState.IsValid)
			{
				_context.Add(movie);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		}

		return View();
	}

	// POST: MOVIES/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Create([Bind("Id,Title,Runtime,Year,Genres,Actors,AgeGroup,Reviews,Director")] Movie movie)
	{
		if (ModelState.IsValid)
		{
			_context.Add(movie);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View();
	}

	// GET: MOVIES/Edit/5
	[HttpGet]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
			return NotFound();

		var movie = await _context.Movie.FindAsync(id);
		if (movie == null)
			return NotFound();
		/*
		if (movie.Reviews != null) {
			string ReviewsString = null;
			foreach (var Review in movie.Reviews)
				ReviewsString += string.Join("], ", Review);
		}
		*/
		return View(movie);
	}

	// POST: MOVIES/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Edit(int? id, [Bind("Runtime,Id,Title,Genres,Year,Actors,AgeGroup,Reviews,Director")] Movie movie)
	{
		if (id != movie.Id)
			return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(movie);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MovieExists(movie.Id))
					return NotFound();
				else
					throw;
			}
			return RedirectToAction(nameof(Index));
		}
		return View(movie);
	}

	// GET: MOVIES/Delete/5
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
			return NotFound();

		var movie = await _context.Movie
			.FirstOrDefaultAsync(m => m.Id == id);
		if (movie == null)
			return NotFound();

		return View(movie);
	}

	// POST: MOVIES/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> DeleteConfirmed(int? id)
	{
		var movie = await _context.Movie.FindAsync(id);
		if (movie != null)
			_context.Movie.Remove(movie);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool MovieExists(int? id)
	{
		return _context.Movie.Any(e => e.Id == id);
	}

	private static AgeRating AgeGroupParse(string rated)
	{
		if (string.IsNullOrWhiteSpace(rated) || rated == "Not Rated" || rated == "N/A")
			return AgeRating.Unknown;

		rated = rated.Trim().ToUpperInvariant();

		return rated switch
		{
			"G" => AgeRating.AllAges,
			"PG" => AgeRating.Seven,
			"TV-Y" => AgeRating.Seven,
			"PG-13" => AgeRating.Eleven,
			"R" => AgeRating.Fifteen

		};
	}
	private static readonly Dictionary<string, Genre> _genreMap = new(StringComparer.OrdinalIgnoreCase) {
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

	private static Genre GenreParse(string omdbGenreCsv)
	{
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

	private static int TryParseYear(string year)
	{
		if (string.IsNullOrWhiteSpace(year)) return 0;  // Inget år funnet, returnera 0 som standardvärde!
		var parts = year.Split('–', '—'); // The year of release or premiere is relevant

		return int.TryParse(parts[0], out var y) ? y : 0;
	}
	private static int TryParseRuntimeMinutes(string runtime)
	{
		if (string.IsNullOrWhiteSpace(runtime)) return 0;
		var minutes = Regex.Match(runtime, @"(\d+)");
		return minutes.Success && int.TryParse(minutes.Groups[1].Value, out var mins) ? mins : 0; // Return 0 if parsing fails
	}
}