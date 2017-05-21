using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("RemoveModRole")]
        [Remarks("Admin")]
        [Summary("Removes a moderator role.")]
        public async Task RemoveModRole([Remainder] IRole modRole)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0)
            {
                ReplyError("There are no moderator roles yet!");
            }
            else if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
            {
                ReplyError("This role is not a moderator role!");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModRoles.Remove(modRole.Id.ToString()));

            await ReplyAsync($"You have successfully removed the {modRole.Mention} moderator role.");
        }
    }
}
