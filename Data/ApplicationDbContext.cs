using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectforSeptember.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
	{
		public DbSet<FinalProjectforSeptember.Models.TVShow> TVShow { get; set; } = default!;
		public DbSet<FinalProjectforSeptember.Models.Movie> Movie { get; set; } = default!;
	}
}