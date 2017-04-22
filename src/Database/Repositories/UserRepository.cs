﻿using DEA.Common;
using DEA.Database.Models;
using DEA.Services.Handlers;
using Discord;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly RankHandler _rankHandler;

        public UserRepository(IMongoCollection<User> users, RankHandler rankHandler)
        {
            _users = users;
            _rankHandler = rankHandler;
        }

        public async Task<User> FetchUserAsync(DEAContext context)
        {
            var dbUser = await (await _users.FindAsync(x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id)).SingleOrDefaultAsync();

            if (dbUser == default(User))
            {
                var createdUser = new User(context.User.Id, context.Guild.Id);
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
                var createdUser = new User(user.Id, user.GuildId);
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
                var createdUser = new User(userId, guildId);
                await _users.InsertOneAsync(createdUser, null, default(CancellationToken));
                return createdUser;
            }
            return dbUser;
        }

        public Task ModifyAsync(DEAContext context, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            return _users.UpdateOneAsync(y => y.UserId == context.User.Id && y.GuildId == context.Guild.Id, builder.Set(field, value));
        }

        public Task ModifyAsync(IGuildUser user, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            return _users.UpdateOneAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId, builder.Set(field, value));
        }

        public Task ModifyAsync(ulong userId, ulong guildId, Expression<Func<User, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<User>.Update;
            return _users.UpdateOneAsync(y => y.UserId == userId && y.GuildId == guildId, builder.Set(field, value));
        }

        public async Task EditCashAsync(DEAContext context, decimal change)
        {
            await ModifyAsync(context, x => x.Cash, Math.Round(context.Cash + change, 2));
            await _rankHandler.HandleAsync(context.Guild, context.User as IGuildUser, context.DbGuild, context.DbUser);
        }

        public async Task EditCashAsync(IGuildUser user, Guild dbGuild, User dbUser, decimal change)
        {
            var cash = (await FetchUserAsync(user)).Cash;
            await ModifyAsync(user, x => x.Cash, Math.Round(cash + change, 2));
            await _rankHandler.HandleAsync(user.Guild, user, dbGuild, dbUser);
        }

    }
}