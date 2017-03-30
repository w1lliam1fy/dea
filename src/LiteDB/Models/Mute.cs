using LiteDB;
using System;

namespace DEA.SQLite.Models
{
    public class Mute
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ulong GuildId { get; set; }

        public ulong UserId { get; set; }

        public double MuteLength { get; set; } = Config.DEFAULT_MUTE_TIME.TotalMilliseconds;

        public DateTime MutedAt { get; set; } = DateTime.UtcNow;
    }
}