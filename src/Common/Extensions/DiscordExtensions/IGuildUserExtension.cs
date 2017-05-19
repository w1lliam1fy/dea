using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class IGuildUserExtension
    {
        public static async Task<IUserMessage> DMAsync(this IGuildUser user, string description, string title = null, Color color = default(Color))
        {
            try
            {
                var channel = await user.CreateDMChannelAsync();

                return await channel.SendAsync(description, title, color);
            }
            catch { }

            return null;
        }
    }
}
