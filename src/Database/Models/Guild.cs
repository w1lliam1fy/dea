using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    
    public partial class Guild
    {
        public Guild(ulong id)
        {
            Id = id;
        }

        [BsonId]
        public ulong Id { get; set; }

        //Roles
        public BsonDocument ModRoles { get; set; } = new BsonDocument();

        public BsonDocument RankRoles { get; set; } = new BsonDocument();

        public ulong MutedRoleId { get; set; }

        //Channels

        public ulong NsfwChannelId { get; set; }

        public ulong GambleChannelId { get; set; }

        public ulong ModLogChannelId { get; set; }

        //Options

        public bool Nsfw { get; set; } = false;

        public bool AutoTrivia { get; set; } = false;

        public string Prefix { get; set; } = Config.DEFAULT_PREFIX;

        public decimal GlobalChattingMultiplier { get; set; } = 1;

        public decimal TempMultiplierIncreaseRate { get; set; } = 0.1m;

        //Misc

        public int CaseNumber { get; set; } = 1;

        public BsonDocument Trivia { get; set; } = new BsonDocument();

    }
}