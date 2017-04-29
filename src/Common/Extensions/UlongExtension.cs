using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
{
    public static class UlongExtension
    {
        /// <summary>
        /// Tries to direct messages a user by ID. Ignores all exceptions.
        /// </summary>
        /// <param name="client">Discord Client object to fetch the user by ID.</param>
        /// <param name="description">The content of the embed.</param>
        /// <param name="title">The title of the embed.</param>
        /// <param name="color">The color of the embed.</param>
        /// <returns>Task returning the sent message.</returns>
        public static async Task<IUserMessage> DMAsync(this ulong userId, IDiscordClient client, string description, string title = null, Color color = default(Color))
        {
            var user = await client.GetUserAsync(userId);

            if (user != null)
            {
                try
                {
                    var channel = await user.CreateDMChannelAsync();

                    var builder = new EmbedBuilder()
                    {
                        Description = description,
                        Color = Config.Color()
                    };
                    if (title != null) builder.Title = title;
                    if (color.RawValue != default(Color).RawValue) builder.Color = color;

                    return await channel.SendMessageAsync(string.Empty, embed: builder);
                }
                catch { }
            }
            return null;
        }
    }
}
