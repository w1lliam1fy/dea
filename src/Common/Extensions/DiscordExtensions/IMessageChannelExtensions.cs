using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class IMessageChannelExtensions
    {
        public static Task<IUserMessage> ReplyAsync(this IMessageChannel channel, IUser user, string description, string title = null, Color color = default(Color))
        {
            var builder = new EmbedBuilder()
            {
                Description = $"{user}, {description}",
                Color = Config.Color()
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static Task<IUserMessage> SendAsync(this IMessageChannel channel, string description, string title = null, Color color = default(Color))
        {
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.Color()
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static async Task SendCodeAsync(this IMessageChannel channel, IReadOnlyCollection<string> elements, string title = "")
        {
            List<string> messages = new List<string>() { $"```{title}\n\n" };
            int messageCount = 0;

            foreach (var element in elements)
            {
                if (messages[messageCount].Length + element.Length > 1997)
                {
                    messageCount++;
                    messages.Add("```");
                }
                messages[messageCount] += element;
            }

            foreach (var message in messages)
                await channel.SendMessageAsync(message + "```");
        }

    }
}
