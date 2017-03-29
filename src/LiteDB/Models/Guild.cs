using DEA.SQLite.Models.Submodels;
using LiteDB;
using System.Collections.Generic;

namespace DEA.SQLite.Models
{
    public class Guild
    {
        [BsonId]
        public ulong Id { get; set; }

        public string Prefix { get; set; } = "$";

        public Roles Roles { get; set; }

        public List<Mute> Mutes { get; set; }

        public uint CaseNumber { get; set; } = 1;

        public Channels Channels { get; set; }

        public Options Options { get; set; }
    }
}
