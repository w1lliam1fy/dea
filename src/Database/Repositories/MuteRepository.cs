using DEA.Database.Models;
using Discord;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class MuteRepository 
    {
        private readonly IMongoCollection<Mute> _mutes;

        public MuteRepository(IMongoCollection<Mute> mutes)
        {
            _mutes = mutes;
        }

        public Task AddMuteAsync(IGuildUser user, TimeSpan muteLength)
            => _mutes.InsertOneAsync(new Mute(user.Id, user.GuildId, muteLength.TotalMilliseconds), null, default(CancellationToken));

        public async Task<bool> IsMutedAsync(ulong userId, ulong guildId)
            => await (await _mutes.FindAsync(y => y.UserId == userId && y.GuildId == guildId)).AnyAsync();

        public Task RemoveMuteAsync(ulong userId, ulong guildId)
            => _mutes.DeleteOneAsync(y => y.UserId == userId && y.GuildId == guildId);

    }
}