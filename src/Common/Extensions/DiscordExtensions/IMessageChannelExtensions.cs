using DEA.Services.Static;
using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class IMessageChannelExtensions
    {
        public static Task<IUserMessage> ReplyAsync(this IMessageChannel channel, IUser user, string description, string title = null, Color color = default(Color))
        {
            return channel.SendAsync($"{user.Boldify()}, {description}", title, color);
        }

        public static Task<IUserMessage> SendAsync(this IMessageChannel channel, string description, string title = null, Color color = default(Color))
        {
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.Colors[CryptoRandom.Next(Config.Colors.Length)]
            };
            if (title != null)
            {
                builder.Title = title;
            }

            if (color.RawValue != default(Color).RawValue)
            {
                builder.Color = color;
            }

            return channel.SendMessageAsync(string.Empty, embed: builder);
        }

        public static Task<IUserMessage> SendErrorAsync(this IMessageChannel channel, string message, string title = null)
        {
            return channel.SendAsync(message, title, Config.ErrorColor);
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
            {
                await channel.SendMessageAsync(message + "```");
            }
        }

    }
}
