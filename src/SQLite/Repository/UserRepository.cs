using DEA.SQLite.Models;
using Discord.Commands;
using LiteDB;
using System;

namespace DEA.SQLite.Repository
{
    public static class UserRepository
    {

        public static User FetchUser(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<User>("Users");
                var ExistingUser = users.FindById(userId);
                if (ExistingUser == null)
                {
                    var CreatedUser = new User()
                    {
                        Id = userId
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

        public static async void EditCashAsync(SocketCommandContext context, double change)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var user = FetchUser(context.User.Id, context.Guild.Id);
                Modify(x => x.Cash += change, context);
            }
            await RankHandler.Handle(context.Guild, context.User.Id);
        }

        public static async void EditCashAsync(SocketCommandContext context, ulong userId, double change)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var user = FetchUser(userId, context.Guild.Id);
                Modify(x => x.Cash += change, context);
            }
            await RankHandler.Handle(context.Guild, userId);
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

