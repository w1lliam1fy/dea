using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DEA.Database.Models
{
    
    public partial class Guild
    {
        [BsonId]
        public ulong Id { get; set; }

        //Roles
        public BsonDocument ModRoles { get; set; }

        public BsonDocument RankRoles { get; set; }

        public ulong MutedRoleId { get; set; }

        public ulong NsfwRoleId { get; set; }

        //Channels

        public ulong DetailedLogsId { get; set; }

        public ulong NsfwId { get; set; }

        public ulong GambleId { get; set; }

        public ulong ModLogId { get; set; }

        //Options

        public bool Nsfw { get; set; } = false;

        public string Prefix { get; set; } = Config.DEFAULT_PREFIX;

        public double GlobalChattingMultiplier = 1;

        public double TempMultiplierIncreaseRate = 0.1;

        public double JumpRequirement { get; set; } = 500;

        public double StealRequirement { get; set; } = 2500;

        public double BullyRequirement { get; set; } = 5000;

        public double RobRequirement { get; set; } = 10000;

        public double FiftyX2Requirement { get; set; } = 25000;

        //Misc

        public int CaseNumber { get; set; } = 1;

    }
}