using Microsoft.EntityFrameworkCore;
using Lab4.Models;

namespace Lab4.Data
{
    public class SportsDbContext: DbContext
    {

        public SportsDbContext(DbContextOptions<SportsDbContext> options) : base(options)
        {
        }

       public DbSet<Fan> Fans { get; set; }

       public DbSet<SportClub> SportClubs { get; set; }

       public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fan>().ToTable("Fan");
            modelBuilder.Entity<SportClub>().ToTable("Sport Club");
            modelBuilder.Entity<Subscription>().HasKey(s => new { s.FanId, s.SportClubId });
        }
    }
}
