using DEA.Database.Models;
using Discord;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public class MuteRepository 
    {
        private IMongoCollection<Mute> _mutes;

        public MuteRepository(IMongoCollection<Mute> mutes)
        {
            _mutes = mutes;
        }

        public async Task AddMuteAsync(IGuildUser user, TimeSpan muteLength)
        {
            await _mutes.InsertOneAsync(new Mute()
            {
                UserId = user.Id,
                GuildId = user.GuildId,
                MuteLength = muteLength.TotalMilliseconds
            }, null, default(CancellationToken));
        }

        public async Task<bool> IsMutedAsync(ulong userId, ulong guildId)
        {
            return await (await _mutes.FindAsync(y => y.UserId == userId && y.GuildId == guildId)).AnyAsync();
        }

        public async Task RemoveMuteAsync(ulong userId, ulong guildId)
        {
            await _mutes.DeleteOneAsync(y => y.UserId == userId && y.GuildId == guildId);
        }

    }
}