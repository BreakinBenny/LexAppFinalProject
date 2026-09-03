//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;

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
	public IActionResult Create()
	{
		return View(new Movie());
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
		return View(movie);
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
	[HttpDelete, ActionName("Delete")]
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
}