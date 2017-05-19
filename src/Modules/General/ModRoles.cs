using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("ModRoles")]
        [Alias("ModeratorRoles", "ModRole", "PermLevels", "PermissionLevels")]
        [Summary("View all the moderator roles.")]
        public async Task ModRoles()
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0)
            {
                ReplyError("There are no moderator roles yet!");
            }

            var description = "**Moderation Roles:**\n";
            foreach (var modRole in Context.DbGuild.ModRoles.OrderBy(x => x.Value))
            {
                var role = Context.Guild.GetRole(ulong.Parse(modRole.Name));
                if (role == null)
                {
                    continue;
                }
                description += $"{role.Mention}: {modRole.Value}\n";
            }

            await SendAsync(description + "\n**Permission Levels:**\n1: Moderator\n2: Administrator\n3: Owner");
        }
    }
}
