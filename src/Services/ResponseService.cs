using DEA.Common;
using DEA.Database.Models;
using Discord;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class ResponseService
    {
        public async Task Reply(DEAContext context, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();

            var builder = new EmbedBuilder()
            {
                Description = $"{await NameAsync(context.User as IGuildUser, context.DbUser)}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await context.Channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public async Task Reply(IMessageChannel channel, IGuildUser user, User dbUser, string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{await NameAsync(user, dbUser)}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public async Task Send(DEAContext context, string description, string title = null, Color color = default(Color))
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

        public async Task Send(IMessageChannel channel, string description, string title = null, Color color = default(Color))
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

        public Task<string> NameAsync(IGuildUser user, User dbUser)
        {
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return Task.FromResult(string.IsNullOrWhiteSpace(user.Nickname) ? $"**{user.Username}**" : $"**{user.Nickname}**");
            else
                return Task.FromResult($"**{dbUser.Name}**");
        }

        public Task<string> TitleNameAsync(IGuildUser user, User dbUser)
        {
            if (string.IsNullOrWhiteSpace(dbUser.Name))
                return Task.FromResult(string.IsNullOrWhiteSpace(user.Nickname) ? user.Username : user.Nickname);
            else
                return Task.FromResult(dbUser.Name);
        }

    }
}