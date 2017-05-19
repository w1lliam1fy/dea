using MongoDB.Bson;
using System;

namespace DEA.Database.Models
{
    public partial class User : Model
    {
        public User(ulong userId, ulong guildId)
        {
            UserId = userId;
            GuildId = guildId;
        }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public ulong SlaveOf { get; set; } = 0;

        public decimal Cash { get; set; } = 0;
        
        public decimal Bounty { get; set; } = 0;

        public int Health { get; set; } = 100;

        public BsonDocument Inventory { get; set; } = new BsonDocument();

        public DateTime LastMessage { get; set; } = DateTime.UtcNow.AddYears(-1);
    }
}
