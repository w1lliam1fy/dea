using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    public partial class Poll
    {
        public Poll(ulong creatorId, ulong guildId, string name, string[] choices)
        {
            Name = name;
            CreatorId = creatorId;
            GuildId = guildId;
            Choices = choices;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public ulong CreatorId { get; set; }

        public ulong GuildId { get; set; }

        public string[] Choices { get; set; } = new string[] { };

        public BsonDocument Votes { get; set; } = new BsonDocument();

        public double Length { get; set; } = Config.DEFAULT_POLL_LENGTH;

        public bool ElderOnly { get; set; } = false;

        public bool ModOnly { get; set; } = false;
        
    }
}
