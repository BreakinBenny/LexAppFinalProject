using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistaProjekt_September2026.Data;
using SistaProjekt_September2026.Models;

//[Route("/[controller]")]
//[ApiController]
public class MoviesController : Controller
{
	private readonly ApplicationDbContext _context;

	public MoviesController(ApplicationDbContext context)
	{
		_context = context;
	}

	// GET: MOVIES
	public async Task<IActionResult> Index()
	{
		if (_context == null)
			return NotFound();

		return View(await _context.Movie.ToListAsync());
	}
/*
	[HttpGet("{id}")]
	public async Task<ActionResult<Movie>> GetMovie(int id)
	{
		if (_context == null)
			return NotFound();

		var movie = await _context.Movie.FindAsync(id);
		if (movie == null)
			return BadRequest("Movie not found!");

		return Ok(movie);
	}
*/
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

		return View(movie);
	}

	// GET: MOVIES/Create
	//[Authorize(Roles = "Administrator(s)")]
	public IActionResult Create()
	{
		return View();
	}

	// POST: MOVIES/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator(s)")]
	public async Task<IActionResult> Create([Bind("Runtime,Id,Title,Year,Genres,Actors,AgeGroup,Reviews")] Movie movie)
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
	[HttpPut]
	//[Authorize(Roles = "Administrator(s)")]
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
			return NotFound();

		var movie = await _context.Movie.FindAsync(id);
		if (movie == null)
			return NotFound();

		return View(movie);
	}

	// POST: MOVIES/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPut]
	[ValidateAntiForgeryToken]
	//[Authorize(Roles = "Administrator(s)")]
	public async Task<IActionResult> Edit(int? id, [Bind("Runtime,Id,Title,Year,Genres,Actors,AgeGroup,Reviews")] Movie movie)
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
	//[Authorize(Roles = "Administrator(s)")]
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
	//[Authorize(Roles = "Administrator(s)")]
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