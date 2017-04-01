using Microsoft.EntityFrameworkCore;

namespace DEA.SQLite.Models
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Mute> Mute { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Gang> Gang { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql($"Host={Config.CREDENTIALS.PostgreServer};User Id={Config.CREDENTIALS.PostgreUserId};" +
                                     $"Password={Config.CREDENTIALS.PostgrePassword};Database={Config.CREDENTIALS.PostgreDatabase};");
        }

    }
}