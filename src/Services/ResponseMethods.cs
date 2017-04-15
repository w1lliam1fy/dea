using DEA.Database.Models;
using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class ResponseMethods
    {
        public static async Task Reply(SocketCommandContext context, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();

            var builder = new EmbedBuilder()
            {
                Description = $"{Name(context.User as IGuildUser)}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await context.Channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static string Name(IGuildUser user)
        {
            var dbUser = UserRepository.FetchUser(user.Id, user.GuildId);
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return (string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**";
            else
                return $"**{dbUser.Name}**";
        }

        public static string Name(IGuildUser user, User dbUser)
        {
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return (string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**";
            else
                return $"**{dbUser.Name}**";
        }

        public static string TitleName(IGuildUser user)
        {
            var dbUser = UserRepository.FetchUser(user.Id, user.GuildId);
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return (string.IsNullOrWhiteSpace(user.Nickname)) ? user.Username : user.Nickname;
            else
                return dbUser.Name;
        }

        public static async Task Send(SocketCommandContext context, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await context.Channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static async Task Send(ITextChannel channel, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static async Task DM(IDMChannel channel, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await channel.SendMessageAsync(string.Empty, embed: builder);
        }

    }
}
