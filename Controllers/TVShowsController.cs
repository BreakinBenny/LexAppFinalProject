//using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;

//[Route("/[controller]")]
//[ApiController]
public class TVShowsController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IHttpClientFactory _httpClientFactory;

	public TVShowsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
	{
		_context = context;
		_httpClientFactory = httpClientFactory;
	}

	// GET: TVSHOWS
	public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString)
	{
		ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "TitleDesc" : "";
		ViewData["DateSortParm"] = sortOrder == "Year" ? "YearDesc" : "Year";
		ViewData["CurrentFilter"] = searchString;
		var tvshows = from show in _context.TVShow select show;
		if (!String.IsNullOrEmpty(searchString))
		{
			searchString.ToUpper();
			tvshows = tvshows.Where(show => show.Title.ToUpper().Contains(searchString));
		}

		switch (sortOrder)
		{
			case "TitleDesc":
				tvshows = tvshows.OrderByDescending(show => show.Title);
				break;
			case "Year":
				tvshows = tvshows.OrderBy(show => show.Year);
				break;
			case "YearDesc":
				tvshows = tvshows.OrderByDescending(show => show.Year);
				break;
			default:
				tvshows = tvshows.OrderBy(show => show.Title);
				break;
		}

		if (_context == null)
			return NotFound();

		return View(await tvshows.AsNoTracking().ToListAsync());
	}
	/*
		[HttpGet("{id}")]
		public async Task<ActionResult<TVShow>> GetTVShow(int id)
		{
			if (_context == null)
				return NotFound();

			var tvshow = await _context.TVShow.FindAsync(id);
			if (tvshow == null)
				return BadRequest("TV Show not found!");

			return Ok(tvshow);
		}
	*/

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

	private static AgeRating AgeGroupParse(string rated)
	{
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
	private static readonly Dictionary<string, Genre> _genreMap = new(StringComparer.OrdinalIgnoreCase)
	{
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
		{ "Thriller", Genre.Thriller }
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

	// GET: TVSHOWS/Details/5
	[HttpGet]
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
			return NotFound();

		var tvshow = await _context.TVShow
			.FirstOrDefaultAsync(m => m.Id == id);
		if (tvshow == null)
			return NotFound();
		/*
		if (tvshow.Reviews != null) { 
			string ReviewsString = null;
			foreach (var Review in tvshow.Reviews)
				ReviewsString += string.Join(", ", Review);
		}
		*/
		return View(tvshow);
	}

	// GET: TVSHOWS/Create
	//[Authorize(Roles = "Administrator")]
	public IActionResult Create()
	{
		return View(new TVShow());
	}

	// POST: TVSHOWS/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Create([Bind("Seasons,EpisodesPerSeason,Id,Title,Year,Genres,Actors,AgeGroup,Reviews,Director")] TVShow tvshow)
	{
		if (ModelState.IsValid)
		{
			_context.Add(tvshow);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(tvshow);
	}

	// GET: TVSHOWS/Edit/5
	[HttpGet]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
			return NotFound();

		var tvshow = await _context.TVShow.FindAsync(id);
		if (tvshow == null)
			return NotFound();

		return View(tvshow);
	}

	// POST: TVSHOWS/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Edit(int? id, [Bind("Seasons,EpisodesPerSeason,Id,Title,Year,Genres,Actors,AgeGroup,Reviews,Director")] TVShow tvshow)
	{
		if (id != tvshow.Id)
			return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(tvshow);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TVShowExists(tvshow.Id))
					return NotFound();
				else
					throw;
			}
			return RedirectToAction(nameof(Index));
		}
		return View(tvshow);
	}

	// GET: TVSHOWS/Delete/5
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
			return NotFound();

		var tvshow = await _context.TVShow
			.FirstOrDefaultAsync(m => m.Id == id);
		if (tvshow == null)
			return NotFound();

		return View(tvshow);
	}

	// POST: TVSHOWS/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> DeleteConfirmed(int? id)
	{
		var tvshow = await _context.TVShow.FindAsync(id);
		if (tvshow != null)
			_context.TVShow.Remove(tvshow);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool TVShowExists(int? id)
	{
		return _context.TVShow.Any(e => e.Id == id);
	}
}