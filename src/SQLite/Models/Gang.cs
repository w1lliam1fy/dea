using LiteDB;
using System.Collections.Generic;

namespace DEA.SQLite.Models
{
    public class Gang
    {
        [BsonId]
        public int Id { get; set; }

        public string Name { get; set; }

        public ulong LeaderId { get; set; }

        public ulong GuildId { get; set; }

        public List<ulong> Members { get; set; }

        public double Wealth { get; set; } = 0.0;
    }
}
