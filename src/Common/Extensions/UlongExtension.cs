using DEA.Common;
using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
{
    public static class UlongExtension
    {
        public static async Task<IUserMessage> DMAsync(this ulong userId, DEAContext context, string description, string title = null, Color color = default(Color))
        {
            var user = context.Guild.GetUser(userId);

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
