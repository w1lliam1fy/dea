using DEA.SQLite.Models;
using LiteDB;
using System;

namespace DEA.SQLite.Repository
{
    public static class MuteRepository
    {

        public static void AddMute(ulong userId, ulong guildId, TimeSpan muteLength)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var mutes = db.GetCollection<Mute>("Mutes");
                mutes.Insert(new Mute()
                {
                    UserId = userId,
                    GuildId = guildId,
                    MuteLength = muteLength.TotalMilliseconds
                });
            }
        }

        public static bool IsMuted(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var mutes = db.GetCollection<Mute>("Mutes");
                return mutes.Exists(x => x.UserId == userId && x.GuildId == guildId);
            }
        }

        public static void RemoveMute(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var mutes = db.GetCollection<Mute>("Mutes");
                var mute = mutes.FindOne(x => x.UserId == userId && x.GuildId == guildId);
                if (mute != null) mutes.Delete(mute.Id);
            }
        }

    }
}
