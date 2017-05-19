using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("RemoveRank")]
        [Summary("Removes a rank role for the DEA cash system.")]
        public async Task RemoveRank([Remainder] IRole rankRole)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                ReplyError("There are no ranks yet.");
            }
            else if (!Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
            {
                ReplyError("This role is not a rank role.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Remove(rankRole.Id.ToString()));

            await ReplyAsync($"You have successfully removed the {rankRole.Mention} rank.");
        }
    }
}
