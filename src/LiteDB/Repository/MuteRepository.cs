using DEA.SQLite.Models;
using DEA.SQLite.Models.Submodels;
using LiteDB;
using System;
using System.Linq;

namespace DEA.SQLite.Repository
{
    public static class MuteRepository
    {

        public static void AddMute(ulong userId, ulong guildId, TimeSpan muteLength)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var guild = GuildRepository.FetchGuild(guildId);
                var guilds = db.GetCollection<Guild>("Guilds");
                guild.Mutes.Add(new Mute()
                {
                    UserId = userId,
                    MuteLength = muteLength
                });
            }
        }

        public static bool IsMuted(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var guild = GuildRepository.FetchGuild(guildId);
                return guild.Mutes.Any(x => x.UserId == userId);
            }
        }

        public static void RemoveMute(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var guild = GuildRepository.FetchGuild(guildId);
                var mute = guild.Mutes.Find(x => x.UserId == userId);
                if (mute != null) guild.Mutes.Remove(mute);
            }
        }

    }
}
