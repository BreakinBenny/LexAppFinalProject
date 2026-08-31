using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistaProjektSeptember2026.Models;

namespace SistaProjektSeptember2026.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public const string ApiKey = "238306e2";
        public DbSet<TVShow> TVShow { get; set; } = default!;
		public DbSet<Movie> Movie { get; set; } = default!;
	}
}