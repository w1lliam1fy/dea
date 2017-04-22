using DEA.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class GuildRepository
    {
        private readonly IMongoCollection<Guild> _guilds;

        public GuildRepository(IMongoCollection<Guild> guilds)
        {
            _guilds = guilds;
        }

        public async Task<Guild> FetchGuildAsync(ulong guildId)
        {
            var dbGuild = await (await _guilds.FindAsync(x => x.Id == guildId)).SingleOrDefaultAsync();
            if (dbGuild == default(Guild))
            {
                var createdGuild = new Guild(guildId);
                await _guilds.InsertOneAsync(createdGuild, null, default(CancellationToken));
                return createdGuild;
            }
            return dbGuild;
        }

        public Task ModifyAsync(ulong guildId, Expression<Func<Guild, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Guild>.Update;
            return _guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

        public Task ModifyAsync(ulong guildId, Expression<Func<Guild, ulong>> field, ulong value)
        {
            var builder = Builders<Guild>.Update;
            return _guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

        public Task ModifyAsync(ulong guildId, Expression<Func<Guild, BsonDocument>> field, BsonDocument value)
        {
            var builder = Builders<Guild>.Update;
            return _guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

    }
}