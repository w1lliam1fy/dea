using DEA.SQLite.Models;
using LiteDB;
using System;
using System.Collections.Generic;

namespace DEA.SQLite.Repository
{
    public static class GuildRepository
    {

        public static Guild FetchGuild(ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var guilds = db.GetCollection<Guild>("Guilds");
                if (!guilds.Exists(x => x.Id == guildId))
                {
                    var CreatedGuild = new Guild()
                    {
                        Id = guildId
                    };
                    guilds.Insert(CreatedGuild);
                    return CreatedGuild;
                }
                else
                    return guilds.FindOne(x => x.Id == guildId);
            }
        }

        public static void Modify(Action<Guild> function, ulong guildId)
        {
            var guild = FetchGuild(guildId);
            function(guild);
            UpdateGuild(guild);
        }

        private static void UpdateGuild(Guild guild)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var guilds = db.GetCollection<Guild>("Guilds");
                guilds.Update(guild);
            }
        }

        public static IEnumerable<Guild> FetchAll()
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                return db.GetCollection<Guild>("Guilds").FindAll();
            }
        }

    }
}
