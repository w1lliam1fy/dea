using DEA.SQLite.Models;
using LiteDB;
using System;

namespace DEA.SQLite.Repository
{
    public static class GuildRepository
    {

        public static Guild FetchGuild(ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<Guild>("Guilds");
                var ExistingGuild = users.FindById(guildId);
                if (ExistingGuild == null)
                {
                    var CreatedGuild = new Guild()
                    {
                        Id = guildId
                    };
                    users.Insert(CreatedGuild);
                    return CreatedGuild;
                }
                else
                    return ExistingGuild;
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

    }
}
