using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using System;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Cooldowns")]
        [Alias("cd")]
        [Remarks("Sexy John#0007")]
        [Summary("View all your command cooldowns.")]
        public Task Cooldowns([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var cooldowns = _rateLimitService.RateLimits.Where(x => x.UserId == user.Id && x.GuildId == user.GuildId);

            if (cooldowns.Count() == 0)
            {
                ReplyError("All commands are available for use.");
            }

            var description = string.Empty;

            foreach (var cooldown in cooldowns)
            {
                var timespan = cooldown.ExpiresAt.Subtract(DateTime.UtcNow);
                description += $"**{cooldown.CommandId}:** {timespan.ToString("h\\:mm\\:ss")}\n";
            }

            return SendAsync(description, $"All cooldowns for {user}");
        }
    }
}
