using Discord;
using System.Threading.Tasks;

namespace DEA.Common.Extensions.DiscordExtensions
{
    public static class IUserExtension
    {
        public static async Task<bool> TryDMAsync(this IUser user, string description, string title = null, Color color = default(Color))
        {
            try
            {
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
