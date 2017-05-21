using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Remarks("SetMutedRole Muted Role")]
        [Summary("Sets the muted role.")]
        public async Task SetMutedRole([Remainder] IRole mutedRole)
        {
            if (mutedRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                ReplyError($"DEA must be higher in the heigharhy than {mutedRole.Mention}.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.MutedRoleId = mutedRole.Id);

            await ReplyAsync($"You have successfully set the muted role to {mutedRole.Mention}.");
        }
    }
}
