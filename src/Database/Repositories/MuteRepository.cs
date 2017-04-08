using DEA.Database.Models;
using MongoDB.Driver;
using System;

namespace DEA.Database.Repository
{
    public static class MuteRepository 
    {

        public static void AddMute(ulong userId, ulong guildId, TimeSpan muteLength)
        {
            DEABot.Mutes.InsertOne(new Mute()
            {
                UserId = userId,
                GuildId = guildId,
                MuteLength = muteLength.TotalMilliseconds
            });
        }

        public static bool IsMuted(ulong userId, ulong guildId)
        {
            return DEABot.Mutes.Find(y => y.UserId == userId && y.GuildId == guildId).Limit(1).Any();
        }

        public static void RemoveMute(ulong userId, ulong guildId)
        {
            DEABot.Mutes.DeleteOne(y => y.UserId == userId && y.GuildId == guildId);
        }

    }
}