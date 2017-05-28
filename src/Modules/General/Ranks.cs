using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using DEA.Common.Extensions;
namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Ranks")]
        [Summary("View all ranks.")]
        public async Task Ranks()
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                ReplyError("There are no ranks yet!");
            }

            var description = string.Empty;
            foreach (var rank in Context.DbGuild.RankRoles.OrderBy(x => x.Value.AsDouble))
            {
                var role = Context.Guild.GetRole(ulong.Parse(rank.Name));
                if (role == null)
                {
                    await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Remove(rank.Name));
                    continue;
                }
                description += $"{((decimal)rank.Value.AsDouble).USD()}: {role.Mention}\n";
            }

            await SendAsync(description, "Ranks");
        }
    }
}
