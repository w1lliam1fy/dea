using DEA.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class MuteRepository 
    {

        public static async Task AddMuteAsync(ulong userId, ulong guildId, TimeSpan muteLength)
        {
            var guild = await GuildRepository.FetchGuildAsync(guildId);
            guild.Mutes.Add(new Mute()
            {
                UserId = userId,
                Guild = guild,
                MuteLength = muteLength
            });
            await BaseRepository<Guild>.UpdateAsync(guild);
        }

        public static async Task<bool> IsMutedAsync(ulong userId, ulong guildId)
        {
            using (var db = new DEAContext())
            {
                return await db.Mutes.AnyAsync(c => c.UserId == userId && c.Guild.Id == guildId);
            }
            
        }

        public static async Task RemoveMuteAsync(ulong userId, ulong guildId)
        {
            using (var db = new DEAContext())
            {
                var muted = await db.Mutes.FirstAsync(c => c.UserId == userId && c.Guild.Id == guildId);
                if (muted != null) await BaseRepository<Mute>.DeleteAsync(muted);
            }
        }

    }
}