using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public static class MuteRepository 
    {

        public static async Task AddMuteAsync(ulong UserID, ulong GuildID, TimeSpan muteLength)
        {
            await BaseRepository<Mute>.InsertAsync(new Mute()
            {
                UserId = UserID,
                GuildId = GuildID,
                MuteLength = muteLength
            });
        }

        public static async Task<bool> IsMutedAsync(ulong UserID, ulong GuildID)
        {
            return await BaseRepository<Mute>.SearchFor(c => c.UserId == UserID && c.GuildId == GuildID).AnyAsync();
        }

        public static async Task RemoveMuteAsync(ulong UserID, ulong GuildID)
        {
            var muted = await BaseRepository<Mute>.SearchFor(c => c.UserId == UserID && c.GuildId == GuildID).FirstOrDefaultAsync();
            if (muted != null) await BaseRepository<Mute>.DeleteAsync(muted);
        }

    }
}