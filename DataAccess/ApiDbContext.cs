using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApiDbContext : IdentityDbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> Options) : base(Options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<CurrentWeather> currentWeather { get; set; }
        public DbSet<UserCurrentWeather> userCurrentWeather { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserCurrentWeather>()
                .HasKey(b => new { b.UserId, b.CurrentWeatherId });

            builder.Entity<UserCurrentWeather>()
                .HasOne(b => b.User)
                .WithMany(b => b.userCurrentWeather)
                .HasForeignKey(b => b.UserId);

            builder.Entity<UserCurrentWeather>()
                .HasOne(b => b.currentWeather)
                .WithMany(c => c.userCurrentWeather)
                .HasForeignKey(b => b.CurrentWeatherId);

            base.OnModelCreating(builder);
        }
    }
}
