using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DEA.Database.Models
{
    public partial class Mute
    {
        public Mute(ulong userId, ulong guildId, double muteLength)
        {
            UserId = userId;
            GuildId = guildId;
            MuteLength = muteLength;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public double MuteLength { get; set; }

        public DateTime MutedAt { get; set; } = DateTime.UtcNow;
    }
}