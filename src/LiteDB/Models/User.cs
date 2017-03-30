using LiteDB;
using System;

namespace DEA.SQLite.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        //Cash system data
        public double Cash { get; set; } = 0.0;

        public double TemporaryMultiplier { get; set; } = 1.0;

        public double InvestmentMultiplier { get; set; } = 1.0;

        public double MessageCooldown { get; set; } = 30000.0;

        //Cooldowns
        public DateTime Message { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Whore { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Jump { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Steal { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Rob { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Withdraw { get; set; } = DateTime.UtcNow.AddYears(-1);
    }
}
