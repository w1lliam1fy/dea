using DEA.Database.Models;
using System;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class GuildRepository
    {

        public static async Task<Guild> FetchGuildAsync(ulong guildId)
        {
            using (var db = new DEAContext())
            {
                Guild ExistingGuild = await db.Guilds.FindAsync((decimal)guildId);
                if (ExistingGuild == null)
                {
                    var CreatedGuild = new Guild()
                    {
                        Id = guildId
                    };
                    await BaseRepository<Guild>.InsertAsync(CreatedGuild);
                    return CreatedGuild;
                }
                return ExistingGuild;
            }
            
        }

        public static async Task ModifyAsync(Func<Guild, Task> function, ulong guildId)
        {
            var guild = await FetchGuildAsync(guildId);
            await function(guild);
            await BaseRepository<Guild>.UpdateAsync(guild);
        }

    }
}