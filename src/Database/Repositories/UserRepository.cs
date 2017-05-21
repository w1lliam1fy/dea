using DEA.Common;
using DEA.Common.Utilities;
using DEA.Database.Models;
using DEA.Services.Handlers;
using Discord;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly RankHandler _RankHandler;
        private readonly Item[] _items;

        public UserRepository(IMongoCollection<User> users, RankHandler rankHandler, Item[] items) : base (users)
        {
            _RankHandler = rankHandler;
            _items = items;
        }

        public Task<User> GetUserAsync(IGuildUser user)
        {
            return GetUserAsync(user.Id, user.GuildId);
        }

        public async Task<User> GetUserAsync(ulong userId, ulong guildId)
        {
            var dbUser = await GetAsync(x => x.UserId == userId && x.GuildId == guildId);

            if (dbUser == null)
            {
                var createdUser = new User(userId, guildId);
                await InsertAsync(createdUser);
                return createdUser;
            }
            return dbUser;
        }

        public async Task ModifyUserAsync(IGuildUser user, Action<User> function)
        {
            await ModifyAsync(await GetUserAsync(user.Id, user.GuildId), function);
        }

        public async Task EditCashAsync(DEAContext context, decimal change)
        {
            decimal newCash = Math.Round(context.Cash + change, 2);
            context.Cash = newCash;
            context.DbUser.Cash = newCash; 
            await UpdateAsync(context.DbUser);
            await _RankHandler.HandleAsync(context.Guild, context.GUser, context.DbGuild, context.DbUser);
        }

        public async Task EditCashAsync(IGuildUser user, Guild dbGuild, User dbUser, decimal change)
        {
            dbUser.Cash = Math.Round(dbUser.Cash + change, 2);
            await UpdateAsync(dbUser);
            await _RankHandler.HandleAsync(user.Guild, user, dbGuild, dbUser);
        }

    }
}