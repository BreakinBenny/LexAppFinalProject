using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistaProjekt_September2026.Models;

namespace SistaProjekt_September2026.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
	{
		public DbSet<TVShow> TVShow { get; set; } = default!;
		public DbSet<Movie> Movie { get; set; } = default!;
	}
}