using DEA.Database.Models;
using System.Data.Entity;

namespace DEA.Database.Repository
{
    

    public partial class DEAContext : DbContext
    {
        public DEAContext()
            : base("name=DEA")
        {
        }

        public virtual DbSet<Gang> Gangs { get; set; }
        public virtual DbSet<Guild> Guilds { get; set; }
        public virtual DbSet<ModRole> ModRoles { get; set; }
        public virtual DbSet<Mute> Mutes { get; set; }
        public virtual DbSet<RankRole> RankRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gang>()
                .Property(e => e.LeaderId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.Id)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.DetailedLogsId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.GambleId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.ModLogId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.MutedRoleId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.NsfwId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Guild>()
                .Property(e => e.NsfwRoleId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<ModRole>()
                .Property(e => e.RoleId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Mute>()
                .Property(e => e.UserId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<RankRole>()
                .Property(e => e.RoleId)
                .HasPrecision(20, 0);

            modelBuilder.Entity<User>()
                .Property(e => e.UserId)
                .HasPrecision(20, 0);
        }
    }
}