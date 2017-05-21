using DEA.Common.Extensions;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("ModifyRank")]
        [Remarks("ModifyRank \"Spicy Role\" 2500")]
        [Summary("Modfies a rank role for the DEA cash system.")]
        public async Task ModifyRank(IRole rankRole, double newCashRequired)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                ReplyError("There are no ranks yet.");
            }
            else if (!Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
            {
                ReplyError("This role is not a rank role.");
            }
            else if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)newCashRequired))
            {
                ReplyError("There is already a role set to that amount of cash required.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles[rankRole.Id.ToString()] = newCashRequired);

            await ReplyAsync($"You have successfully set the cash required for the {rankRole.Mention} rank to {((decimal)newCashRequired).USD()}.");
        }
    }
}
