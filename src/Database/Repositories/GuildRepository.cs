using DEA.Database.Models;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace DEA.Database.Repository
{
    public static class GuildRepository
    {

        public static Guild FetchGuild(ulong guildId)
        {
            Expression<Func<Guild, bool>> expression = y => y.Id == guildId;
            if (!DEABot.Guilds.Find(expression).Limit(1).Any())
            {
                var createdGuild = new Guild()
                {
                    Id = guildId
                };
                DEABot.Guilds.InsertOne(createdGuild);
                return createdGuild;
            }
            return DEABot.Guilds.Find(expression).Limit(1).First();
        }

        public static void Modify(UpdateDefinition<Guild> update, ulong guildId)
        {
            FetchGuild(guildId);
            DEABot.Guilds.UpdateOne(y => y.Id == guildId, update);
        }

    }
}