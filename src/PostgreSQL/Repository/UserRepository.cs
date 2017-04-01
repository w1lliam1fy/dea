using DEA.SQLite.Models;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public static class UserRepository
    {

        public static async Task ModifyAsync(Func<User, Task> function, SocketCommandContext context)
        {
            var user = await FetchUserAsync(context.User.Id, context.Guild.Id);
            await function(user);
            await BaseRepository<User>.UpdateAsync(user);
        }

        public static async Task ModifyAsync(Func<User, Task> function, ulong userId, ulong guildId)
        {
            var user = await FetchUserAsync(userId, guildId);
            await function(user);
            await BaseRepository<User>.UpdateAsync(user);
        }

        public static async Task<User> FetchUserAsync(SocketCommandContext context)
        {
            User ExistingUser = await BaseRepository<User>.SearchFor(c => c.UserId == context.User.Id && c.GuildId == context.Guild.Id).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    UserId = context.User.Id,
                    GuildId = context.Guild.Id
                };
                await BaseRepository<User>.InsertAsync(CreatedUser);
                return CreatedUser;
            }
            return ExistingUser;
        }

        public static async Task<User> FetchUserAsync(ulong userId, ulong guildId)
        {
            User ExistingUser = await BaseRepository<User>.SearchFor(c => c.UserId == userId && c.GuildId == guildId).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    UserId = userId,
                    GuildId = guildId
                };
                await BaseRepository<User>.InsertAsync(CreatedUser);
                return CreatedUser;
            }
            return ExistingUser;
        }

        public static async Task<double> GetCashAsync(SocketCommandContext context)
        {
            var user = await FetchUserAsync(context.User.Id, context.Guild.Id);
            return user.Cash;
        }

        public static async Task EditCashAsync(SocketCommandContext context, double change)
        {
            var user = await FetchUserAsync(context.User.Id, context.Guild.Id);
            user.Cash = Math.Round(user.Cash + change, 2);
            await BaseRepository<User>.UpdateAsync(user);
            await RankHandler.Handle(context.Guild, context.User.Id);
        }

        public static async Task EditCashAsync(SocketCommandContext context, ulong userId, double change)
        {
            var user = await FetchUserAsync(userId, context.Guild.Id);
            user.Cash = Math.Round(user.Cash + change, 2);
            await BaseRepository<User>.UpdateAsync(user);
            await RankHandler.Handle(context.Guild, userId);
        }

        public static async Task<List<User>> AllAsync()
        {
            return await BaseRepository<User>.GetAll().ToListAsync();
        }

        public static async Task<List<User>> AllAsync(ulong guildId)
        {
            return (await BaseRepository<User>.GetAll().ToListAsync()).FindAll(x => x.GuildId == guildId);
        }

    }
}