using DEA.Common;
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
        private readonly RankHandler _rankHandler;

        public UserRepository(IMongoCollection<User> users, RankHandler rankHandler) : base (users)
        {
            _rankHandler = rankHandler;
        }

        public async Task<User> FetchUserAsync(IGuildUser user)
        {
            var dbUser = await FetchAsync(x => x.UserId == user.Id && x.GuildId == user.GuildId);

            if (dbUser == default(User))
            {
                var createdUser = new User(user.Id, user.GuildId);
                await InsertAsync(createdUser);
                return createdUser;
            }
            return dbUser;
        }

        public async Task<User> FetchUserAsync(ulong userId, ulong guildId)
        {
            var dbUser = await FetchAsync(x => x.UserId == userId && x.GuildId == guildId);

            if (dbUser == default(User))
            {
                var createdUser = new User(userId, guildId);
                await InsertAsync(createdUser);
                return createdUser;
            }
            return dbUser;
        }

        public Task ModifyUserAsync(IGuildUser user, Action<User> function)
        {
            return ModifyAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId, function);
        }

        public Task ModifyUserAsync(ulong userId, ulong guildId, Action<User> function)
        {
            return ModifyAsync(y => y.UserId == userId && y.GuildId == guildId, function);
        }

        public async Task EditCashAsync(DEAContext context, decimal change)
        {
            var newCash = Math.Round(context.Cash + change, 2);
            context.Cash = newCash;
            context.DbUser.Cash = newCash; 
            await UpdateAsync(context.DbUser);
            await _rankHandler.HandleAsync(context.Guild, context.User as IGuildUser, context.DbGuild, context.DbUser);
        }

        public async Task EditCashAsync(IGuildUser user, Guild dbGuild, User dbUser, decimal change)
        {
            dbUser.Cash = Math.Round(dbUser.Cash + change, 2);
            await UpdateAsync(dbUser);
            await _rankHandler.HandleAsync(user.Guild, user, dbGuild, dbUser);
        }

    }
}