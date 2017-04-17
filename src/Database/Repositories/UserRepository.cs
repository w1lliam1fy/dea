using DEA.Common;
using DEA.Database.Models;
using DEA.Services.Handlers;
using Discord;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public class UserRepository
    {
        private IMongoCollection<User> _users;
        private RankingService _rankingService;

        public UserRepository(IMongoCollection<User> users, RankingService rankingService)
        {
            _users = users;
            _rankingService = rankingService;
        }

        public async Task<User> FetchUserAsync(DEAContext context)
        {
            var dbUser = await (await _users.FindAsync(x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = context.Guild.Id,
                    UserId = context.User.Id
                };
                await _users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public async Task<User> FetchUserAsync(IGuildUser user)
        {
            var dbUser = await (await _users.FindAsync(x => x.UserId == user.Id && x.GuildId == user.GuildId)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = user.GuildId,
                    UserId = user.Id
                };
                await _users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public async Task<User> FetchUserAsync(ulong userId, ulong guildId)
        {
            var dbUser = await (await _users.FindAsync(x => x.UserId == userId && x.GuildId == guildId)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = guildId,
                    UserId = userId
                };
                await _users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public async Task ModifyAsync(DEAContext context, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await _users.UpdateOneAsync(y => y.UserId == context.User.Id && y.GuildId == context.Guild.Id, builder.Set(field, value));
        }

        public async Task ModifyAsync(IGuildUser user, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await _users.UpdateOneAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId, builder.Set(field, value));
        }

        public async Task ModifyAsync(ulong userId, ulong guildId, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await _users.UpdateOneAsync(y => y.UserId == userId && y.GuildId == guildId, builder.Set(field, value));
        }

        public async Task EditCashAsync(DEAContext context, decimal change)
        {
            await ModifyAsync(context, x => x.Cash, Math.Round(context.Cash + change, 2));
            await _rankingService.HandleAsync(context.Guild, context.User as IGuildUser, context.DbGuild, context.DbUser);
        }

        public async Task EditCashAsync(IGuildUser user, Guild dbGuild, User dbUser, decimal change)
        {
            var cash = (await FetchUserAsync(user)).Cash;
            await ModifyAsync(user, x => x.Cash, Math.Round(cash + change, 2));
            await _rankingService.HandleAsync(user.Guild, user, dbGuild, dbUser);
        }

    }
}