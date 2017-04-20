using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
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
    }
}
