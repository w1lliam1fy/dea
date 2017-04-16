using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    
    public partial class Guild
    {
        [BsonId]
        public ulong Id { get; set; }

        //Roles
        public BsonDocument ModRoles { get; set; } = new BsonDocument();

        public BsonDocument RankRoles { get; set; } = new BsonDocument();

        public ulong MutedRoleId { get; set; }

        public ulong NsfwRoleId { get; set; }

        //Channels

        public ulong DetailedLogsId { get; set; }

        public ulong NsfwId { get; set; }

        public ulong GambleId { get; set; }

        public ulong ModLogId { get; set; }

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