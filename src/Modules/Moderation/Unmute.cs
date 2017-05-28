using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Remarks("\"Sexy John#0007\" We needed the sexiness back")]
        [Summary("Unmutes a muted user.")]
        public async Task Unmute(IGuildUser userToUnmute, [Remainder] string reason = null)
        {
            if (!userToUnmute.RoleIds.Any(x => x == Context.DbGuild.MutedRoleId))
            {
                ReplyError("You cannot unmute a user who isn't muted.");
            }

            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId));
            await _muteRepo.RemoveMuteAsync(userToUnmute.Id, userToUnmute.GuildId);

            await SendAsync($"{Context.User.Boldify()} has successfully unmuted {userToUnmute.Boldify()}.");

            await _moderationService.TryInformSubjectAsync(Context.User, "Unmute", userToUnmute, reason);
            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Unmute", new Color(12, 255, 129), reason, Context.User, userToUnmute);
        }
    }
}
