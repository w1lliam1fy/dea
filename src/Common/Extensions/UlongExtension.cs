using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
{
    public static class UlongExtension
    {
        public static async Task<bool> TryDMAsync(this ulong userId, IDiscordClient client, string description, string title = null, Color color = default(Color))
        {
            var user = await client.GetUserAsync(userId);

            if (user == null)
            {
                return false;
            }
            else
            {
                return await user.TryDMAsync(description, title, color);
            }
        }
    }
}
