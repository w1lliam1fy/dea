using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Resources
{
    public class DEAModule : ModuleBase<SocketCommandContext>
    {
        public async Task Reply(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{Context.User.Mention}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await ReplyAsync("", embed: builder);
        }

        public async Task Send(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            await ReplyAsync("", embed: builder);
        }

        public async Task Error(string message)
        {
            var builder = new EmbedBuilder()
            {
                Description = message,
                Color = new Color(255, 0, 0)
            };

            await ReplyAsync("", embed: builder);
            throw new Exception();
        }
    }
}
