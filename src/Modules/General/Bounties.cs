using Discord.Commands;
using Discord;
using DEA.Database.Models;
using DEA.Common.Data;
using DEA.Common.Extensions;
using System.Threading.Tasks;
using System.Linq;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Bounties")]
        [Alias("bl", "bountyleaderboards")]
        [Summary("View the most targeted traffickers.")]
        public async Task Bounties()
        {
            var users = await _userRepo.AllAsync(y => y.GuildId == Context.Guild.Id);
            var sorted = users.OrderByDescending(x => x.Bounty);
            string description = string.Empty;
            int position = 1;

            if (users.Count == 0)
            {
                ReplyError("There is nobody on the bounty leaderboards yet.");
            }

            var guildInterface = Context.Guild as IGuild;

            foreach (User dbUser in sorted)
            {
                var user = await guildInterface.GetUserAsync(dbUser.UserId);
                if (user == null)
                {
                    continue;
                }

                description += $"{position}. {user.Boldify()}: {dbUser.Bounty.USD()}\n";
                if (position >= Config.LEADERBOARD_CAP)
                {
                    break;
                }

                position++;
            }

            await SendAsync(description, "The Most Targeted Traffickers");
        }
    }
}
