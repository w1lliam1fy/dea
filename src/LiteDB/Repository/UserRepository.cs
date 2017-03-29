using DEA.SQLite.Models;
using Discord.Commands;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public static class UserRepository
    {

        public static User FetchUser(SocketCommandContext context)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<User>("Users");
                var ExistingUser = users.FindOne(x => x.UserId == context.User.Id && x.GuildId == context.Guild.Id);
                if (ExistingUser == null)
                {
                    var CreatedUser = new User()
                    {
                        UserId = context.User.Id,
                        GuildId = context.Guild.Id
                    };
                    users.Insert(CreatedUser);
                    return CreatedUser;
                }
                else
                    return ExistingUser;
            }
        }

        public static User FetchUser(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<User>("Users");
                var ExistingUser = users.FindOne(x => x.UserId == userId && x.GuildId == guildId);
                if (ExistingUser == null)
                {
                    var CreatedUser = new User()
                    {
                        UserId = userId,
                        GuildId = guildId
                    };
                    users.Insert(CreatedUser);
                    return CreatedUser;
                }
                else
                    return ExistingUser;
            }
        }

        public static void Modify(Action<User> function, SocketCommandContext context)
        {
            var user = FetchUser(context.User.Id, context.Guild.Id);
            function(user);
            UpdateUser(user);
        }

        public static void Modify(Action<User> function, ulong userId, ulong guildId)
        {
            var user = FetchUser(userId, guildId);
            function(user);
            UpdateUser(user);
        }

        public static async Task EditCashAsync(SocketCommandContext context, double change)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var user = FetchUser(context.User.Id, context.Guild.Id);
                Modify(x => x.Cash += change, context);
            }
            await RankHandler.Handle(context.Guild, context.User.Id);
        }

        public static async Task EditCashAsync(SocketCommandContext context, ulong userId, double change)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var user = FetchUser(userId, context.Guild.Id);
                Modify(x => x.Cash += change, context);
            }
            await RankHandler.Handle(context.Guild, userId);
        }

        public static IEnumerable<User> FetchAll(ulong guildId)
        {
            using(var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<User>("Users");
                return users.Find(x => x.GuildId == guildId);
            }
        }

        private static void UpdateUser(User user)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<User>("Users");
                users.Update(user);
            }
        }

    }
}

