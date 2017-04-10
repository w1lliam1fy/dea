using DEA.Database.Models;
using DEA.Services.Handlers;
using Discord.Commands;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class UserRepository
    {

        public static User FetchUser(SocketCommandContext context)
        {
            Expression<Func<User, bool>> expression = x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id;
            if (!DEABot.Users.Find(expression).Limit(1).Any())
            {
                var createdUser = new User()
                {
                    GuildId = context.Guild.Id,
                    UserId = context.User.Id
                };
                DEABot.Users.InsertOne(createdUser);
                return createdUser;
            }
            return DEABot.Users.Find(expression).Limit(1).First();
        }

        public static User FetchUser(ulong userId, ulong guildId)
        {
            Expression<Func<User, bool>> expression = x => x.UserId == userId && x.GuildId == guildId;
            if (!DEABot.Users.Find(expression).Limit(1).Any())
            {
                var createdUser = new User()
                {
                    GuildId = guildId,
                    UserId = userId
                };
                DEABot.Users.InsertOne(createdUser);
                return createdUser;
            }
            return DEABot.Users.Find(expression).Limit(1).First();
        }

        public static void Modify(UpdateDefinition<User> update, SocketCommandContext context)
        {
            FetchUser(context);
            DEABot.Users.UpdateOne(y => y.UserId == context.User.Id && y.GuildId == context.Guild.Id, update);
        }

        public static void Modify(UpdateDefinition<User> update, ulong userId, ulong guildId)
        {
            FetchUser(userId, guildId);
            DEABot.Users.UpdateOne(y => y.UserId == userId && y.GuildId == guildId, update);
        }

        public static async Task EditCashAsync(SocketCommandContext context, decimal change)
        {
            var cash = FetchUser(context).Cash;
            DEABot.Users.UpdateOne(y => y.UserId == context.User.Id && y.GuildId == context.Guild.Id, 
                DEABot.UserUpdateBuilder.Set(x => x.Cash, Math.Round(change + cash, 2)));
            await RankHandler.Handle(context.Guild, context.User.Id);
        }

        public static async Task EditCashAsync(SocketCommandContext context, ulong userId, decimal change)
        {
            var cash = FetchUser(userId, context.Guild.Id).Cash;
            DEABot.Users.UpdateOne(y => y.UserId == userId && y.GuildId == context.Guild.Id,
                DEABot.UserUpdateBuilder.Set(x => x.Cash, Math.Round(change + cash, 2)));
            await RankHandler.Handle(context.Guild, userId);
        }

    }
}