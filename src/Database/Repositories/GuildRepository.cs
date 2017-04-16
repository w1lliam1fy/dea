using DEA.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class GuildRepository
    {

        public static async Task<Guild> FetchGuildAsync(ulong guildId)
        {
            var dbGuild = await (await DEABot.Guilds.FindAsync(x => x.Id == guildId)).SingleOrDefaultAsync();
            if (dbGuild == default(Guild))
            {
                var createdGuild = new Guild()
                {
                    Id = guildId
                };
                await DEABot.Guilds.InsertOneAsync(createdGuild, null, default(CancellationToken));
                return createdGuild;
            }
            return dbGuild;
        }

        public static async Task ModifyAsync(ulong guildId, Expression<Func<Guild, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Guild>.Update;
            await DEABot.Guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

        public static async Task ModifyAsync(ulong guildId, Expression<Func<Guild, ulong>> field, ulong value)
        {
            var builder = Builders<Guild>.Update;
            await DEABot.Guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

        public static async Task ModifyAsync(ulong guildId, Expression<Func<Guild, BsonDocument>> field, BsonDocument value)
        {
            var builder = Builders<Guild>.Update;
            await DEABot.Guilds.UpdateOneAsync(y => y.Id == guildId, builder.Set(field, value));
        }

    }
}