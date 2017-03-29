using DEA.SQLite.Models.Submodels;
using LiteDB;

namespace DEA.SQLite.Models
{
    public class User
    {
        [BsonId]
        public ulong Id { get; set; }

        public double Cash { get; set; } = 0.0;

        public Cooldowns Cooldowns { get; set; }

        public double TemporaryMultiplier { get; set; } = 1.0;

        public double InvestmentMultiplier { get; set; } = 1.0;

        public double MessageCooldown { get; set; } = 30000.0;
    }
}
