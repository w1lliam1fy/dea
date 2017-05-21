using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("ModifyModRole")]
        [Remarks("\"Big Daddy Mods\" 2")]
        [Summary("Modfies a moderator role.")]
        public async Task ModifyRank(IRole modRole, int permissionLevel)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0)
            { 
                ReplyError("There are no moderator roles yet!");
            }
            else if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
            {
                ReplyError("This role is not a moderator role!");
            }
            else if (Context.DbGuild.ModRoles.First(x => x.Name == modRole.Id.ToString()).Value == permissionLevel)
            {
                ReplyError($"This mod role already has a permission level of {permissionLevel}");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModRoles[Context.DbGuild.ModRoles.IndexOfName(modRole.Id.ToString())] = permissionLevel);

            await ReplyAsync($"You have successfully set the permission level of the {modRole.Mention} moderator role to {permissionLevel}.");
        }
    }
}
