using Microsoft.EntityFrameworkCore;
using Lab4.Models;
using System.Reflection.Metadata;

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

       public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fan>().ToTable("Fan");
            modelBuilder.Entity<SportClub>()
         .HasMany(s => s.News)
         .WithOne(s => s.SportClub)
         .HasForeignKey(s => s.SportsClubId)
         .IsRequired();

            modelBuilder.Entity<Subscription>().HasKey(s => new { s.FanId, s.SportClubId });
        }
    }
}
