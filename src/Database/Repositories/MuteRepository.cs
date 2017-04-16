using DEA.Database.Models;
using Discord;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class MuteRepository 
    {

        public static async Task AddMuteAsync(IGuildUser user, TimeSpan muteLength)
        {
            await DEABot.Mutes.InsertOneAsync(new Mute()
            {
                UserId = user.Id,
                GuildId = user.GuildId,
                MuteLength = muteLength.TotalMilliseconds
            }, null, default(CancellationToken));
        }

        public static async Task<bool> IsMutedAsync(ulong userId, ulong guildId)
        {
            return await (await DEABot.Mutes.FindAsync(y => y.UserId == userId && y.GuildId == guildId)).AnyAsync();
        }

        public static async Task RemoveMuteAsync(ulong userId, ulong guildId)
        {
            await DEABot.Mutes.DeleteOneAsync(y => y.UserId == userId && y.GuildId == guildId);
        }

    }
}