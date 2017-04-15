using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DEA.Database.Models
{
    public partial class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public decimal Cash { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public decimal InvestmentMultiplier { get; set; } = 1;

        public decimal TemporaryMultiplier { get; set; } = 1;

        //Cooldowns

        public double MessageCooldown { get; set; } = Config.DEFAULT_MESSAGE_COOLDOWN;

        public DateTime Whore { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Withdraw { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Jump { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Message { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Rob { get; set; } = DateTime.UtcNow.AddYears(-1);

        public DateTime Steal { get; set; } = DateTime.UtcNow.AddYears(-1);
    }
}
