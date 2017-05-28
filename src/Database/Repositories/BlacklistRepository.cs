using DEA.Database.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public sealed class BlacklistRepository : BaseRepository<Blacklist>
    {
        public BlacklistRepository(IMongoCollection<Blacklist> blacklists) : base(blacklists) { }

        public Task AddGuildAsync(ulong userId, ulong newGuildId)
        {
            return PushAsync(c => c.UserId == userId, "GuildIds", (decimal)newGuildId);
        }
    }
}
