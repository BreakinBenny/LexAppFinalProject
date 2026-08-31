//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistaProjektSeptember2026.Data;
using SistaProjektSeptember2026.Models;

//[Route("/[controller]")]
//[ApiController]
public class TVShowsController : Controller
{
	private readonly ApplicationDbContext _context;

	public TVShowsController(ApplicationDbContext context)
	{
		_context = context;
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