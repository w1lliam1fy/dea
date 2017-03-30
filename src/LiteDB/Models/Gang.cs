using LiteDB;
using System;
using System.Collections.Generic;

namespace DEA.SQLite.Models
{
    public class Gang
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public ulong LeaderId { get; set; }

        public ulong GuildId { get; set; }

        public List<ulong> Members { get; set; }

        public double Wealth { get; set; } = 0.0;

        public DateTime Raid { get; set; } = DateTime.UtcNow.AddYears(-1);
    }
}
