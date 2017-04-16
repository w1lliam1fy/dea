using DEA.Common;
using DEA.Database.Models;
using DEA.Services.Handlers;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class UserRepository
    {

        public static async Task<User> FetchUserAsync(DEAContext context)
        {
            var dbUser = await (await DEABot.Users.FindAsync(x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = context.Guild.Id,
                    UserId = context.User.Id
                };
                await DEABot.Users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public static async Task<User> FetchUserAsync(IGuildUser user)
        {
            var dbUser = await (await DEABot.Users.FindAsync(x => x.UserId == user.Id && x.GuildId == user.GuildId)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = user.GuildId,
                    UserId = user.Id
                };
                await DEABot.Users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public static async Task<User> FetchUserAsync(ulong userId, ulong guildId)
        {
            var dbUser = await (await DEABot.Users.FindAsync(x => x.UserId == userId && x.GuildId == guildId)).SingleOrDefaultAsync();
            if (dbUser == default(User))
            {
                var createdUser = new User()
                {
                    GuildId = guildId,
                    UserId = userId
                };
                await DEABot.Users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public static async Task ModifyAsync(DEAContext context, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await DEABot.Users.UpdateOneAsync(y => y.UserId == context.User.Id && y.GuildId == context.Guild.Id, builder.Set(field, value));
        }

        public static async Task ModifyAsync(IGuildUser user, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await DEABot.Users.UpdateOneAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId, builder.Set(field, value));
        }

        public static async Task ModifyAsync(ulong userId, ulong guildId, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            await DEABot.Users.UpdateOneAsync(y => y.UserId == userId && y.GuildId == guildId, builder.Set(field, value));
        }

        public static async Task EditCashAsync(DEAContext context, decimal change)
        {
            await ModifyAsync(context, x => x.Cash, Math.Round(context.Cash + change, 2));
            await RankHandler.HandleAsync(context.Guild, context.User.Id);
        }

        public static async Task EditCashAsync(IGuildUser user, decimal change)
        {
            var cash = (await FetchUserAsync(user)).Cash;
            await ModifyAsync(user, x => x.Cash, Math.Round(cash + change, 2));
            await RankHandler.HandleAsync(user.Guild, user.Id);
        }

    }
}