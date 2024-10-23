using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_gdhakal.Models;

namespace Fall2024_Assignment3_gdhakal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fall2024_Assignment3_gdhakal.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_gdhakal.Models.Movie> Movie { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_gdhakal.Models.MovieActor> MovieActor { get; set; } = default!;
    }
}
