using Microsoft.EntityFrameworkCore;

namespace AkkaMjrTwo.StatisticsEngine.ReadModels
{
    public class GameStatisticsContext : DbContext
    {
        public DbSet<GameStatistic> Statistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("StatisticsDatabase");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameStatistic>()
                        .HasKey(c => new { c.GameId, c.PlayerId });
        }
    }

    public class GameStatistic
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public int NumberRolled { get; set; }

        public bool Winner { get; set; }
    }
}
