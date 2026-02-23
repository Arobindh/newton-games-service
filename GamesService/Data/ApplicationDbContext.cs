using Microsoft.EntityFrameworkCore;
using GamesService.Models;

namespace GamesService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Genre).IsRequired();
                entity.Property(e => e.AgeRating).IsRequired();
                entity.Property(e => e.Author).IsRequired();
            });
        }
    }
}
