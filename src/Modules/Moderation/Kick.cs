using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Remarks("\"Sexy John#0007\" Being too sexy")]
        [Summary("Kicks a user.")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = null)
        {
            if (_moderationService.GetPermLevel(Context.DbGuild, userToKick) > 0)
            {
                ReplyError("You cannot kick another mod!");
            }

            await _moderationService.TryInformSubjectAsync(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();

            await SendAsync($"{Context.User.Boldify()} has successfully kicked {userToKick.Boldify()}.");

            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Kick", Config.KickColor, reason, Context.User, userToKick);
        }
    }
}
