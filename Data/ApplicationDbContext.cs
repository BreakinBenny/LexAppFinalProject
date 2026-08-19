using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SistaProjektSeptember2026.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<SistaProjektSeptember2026.Models.TVShow> TVShow { get; set; } = default!;
    public DbSet<SistaProjektSeptember2026.Models.Movie> Movie { get; set; } = default!;
    }
}
