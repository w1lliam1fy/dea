using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DEA.Database.Models
{

    public class Gang
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public ulong LeaderId { get; set; }

        public ulong GuildId { get; set; }

        public decimal Wealth { get; set; } = 0;

        public ulong[] Members { get; set; } = new ulong[] { };

        public DateTime Raid { get; set; } = DateTime.UtcNow.AddYears(-1);
        
    }
}