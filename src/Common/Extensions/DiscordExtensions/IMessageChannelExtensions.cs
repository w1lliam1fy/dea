using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class IMessageChannelExtensions
    {
        /// <summary>
        /// Replies to the user in question, starting the message with their username, discriminator and a comma.
        /// </summary>
        /// <param name="user">The user to reply to.</param>
        /// <param name="description">The content of the embed.</param>
        /// <param name="title">The title of the embed.</param>
        /// <param name="color">The color of the embed.</param>
        /// <returns>Task returning the sent message.</returns>
        public static Task<IUserMessage> ReplyAsync(this IMessageChannel channel, IUser user, string description, string title = null, Color color = default(Color))
        {
            var builder = new EmbedBuilder()
            {
                Description = $"**{user}**, {description}",
                Color = Config.Color()
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return channel.SendMessageAsync(string.Empty, embed: builder);
        }

        /// <summary>
        /// Sends a embedded message.
        /// </summary>
        /// <param name="description">The content of the embed.</param>
        /// <param name="title">The title of the embed.</param>
        /// <param name="color">The color of the embed.</param>
        /// <returns>Task returning the sent message.</returns>
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

        /// <summary>
        /// Sends a list of elements which will automatically be split into multiple messages when over 2000 characters, and will be replied in code blocks.
        /// </summary>
        /// <param name="elements">The elements to send.</param>
        /// <param name="title">The title of the message.</param>
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
