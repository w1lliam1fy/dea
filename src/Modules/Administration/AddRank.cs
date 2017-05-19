using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("AddRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        public async Task AddRank(double cashRequired, [Remainder] IRole rankRole)
        {
            if (rankRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                ReplyError($"DEA must be higher in the heigharhy than {rankRole.Mention}.");
            }

            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(rankRole.Id.ToString(), cashRequired));
            }
            else
            {
                if (Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                {
                    ReplyError("This role is already a rank role.");
                }
                if (cashRequired == 500)
                {
                    cashRequired = Context.DbGuild.RankRoles.OrderByDescending(x => x.Value).First().Value.AsDouble * 2;
                }
                if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)cashRequired))
                {
                    ReplyError("There is already a role set to that amount of cash required.");
                }

                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(rankRole.Id.ToString(), cashRequired));
            }

            await ReplyAsync($"You have successfully added the {rankRole.Mention} rank.");
        }
    }
}
