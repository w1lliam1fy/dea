using DEA.Common.Extensions;
using DEA.Database.Models;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Leaderboards")]
        [Alias("lb", "rankings", "highscores")]
        [Summary("View the richest Drug Traffickers.")]
        public async Task Leaderboards()
        {
            var users = (await _userRepo.AllAsync(y => y.GuildId == Context.Guild.Id)).OrderByDescending(x => x.Cash);
            string description = string.Empty;
            int position = 1;

            if (users.Count() == 0)
            {
                ReplyError("There is nobody on the leaderboards yet.");
            }

            var guildInterface = Context.Guild as IGuild;

            foreach (User dbUser in users)
            {
                var user = await guildInterface.GetUserAsync(dbUser.UserId);
                if (user == null)
                {
                    continue;
                }

                description += $"{position}. {user.Boldify()}: {dbUser.Cash.USD()}\n";
                if (position >= Config.LeaderboardCap)
                {
                    break;
                }

                position++;
            }

            await SendAsync(description, "The Richest Traffickers");
        }
    }
}
