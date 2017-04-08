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
                Description = $"{context.User.Mention}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await context.Channel.SendMessageAsync("", embed: builder);
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

            await context.Channel.SendMessageAsync("", embed: builder);
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

            await channel.SendMessageAsync("", embed: builder);
        }
    }
}
