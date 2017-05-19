using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("AddModRole")]
        [Summary("Adds a moderator role.")]
        public async Task AddModRole(IRole modRole, int permissionLevel = 1)
        {
            if (permissionLevel < 1 || permissionLevel > 3)
            {
                ReplyError("Permission levels:\nModeration: 1\nAdministration: 2\nServer Owner: 3");
            }

            if (Context.DbGuild.ModRoles.ElementCount == 0)
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModRoles.Add(modRole.Id.ToString(), permissionLevel));
            }
            else
            {
                if (Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                {
                    ReplyError("You have already set this mod role.");
                }

                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModRoles.Add(modRole.Id.ToString(), permissionLevel));
            }

            await ReplyAsync($"You have successfully added {modRole.Mention} as a moderation role with a permission level of {permissionLevel}.");
        }
    }
}
