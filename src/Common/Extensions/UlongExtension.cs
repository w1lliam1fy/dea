using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
{
    public static class UlongExtension
    {
        public static async Task<bool> TryDMAsync(this ulong userId, IDiscordClient client, string description, string title = null, Color color = default(Color))
        {
            try
            {
                var user = await client.GetUserAsync(userId);
                var channel = await user.CreateDMChannelAsync();

                await channel.SendAsync(description, title, color);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
