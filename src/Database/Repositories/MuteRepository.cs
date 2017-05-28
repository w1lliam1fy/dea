using DEA.Database.Models;
using Discord;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public sealed class MuteRepository : BaseRepository<Mute>
    {
        public MuteRepository(IMongoCollection<Mute> mutes) : base(mutes) { }

        public Task InsertMuteAsync(IGuildUser user, TimeSpan muteLength)
        {
            return InsertAsync(new Mute(user.Id, user.GuildId, muteLength.TotalMilliseconds));
        }

        public Task<bool> IsMutedAsync(ulong userId, ulong guildId)
        {
            return AnyAsync(y => y.UserId == userId && y.GuildId == guildId);
        }

        public Task RemoveMuteAsync(ulong userId, ulong guildId)
        {
            return DeleteAsync(y => y.UserId == userId && y.GuildId == guildId);
        }
    }
}