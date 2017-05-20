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
        [Summary("Kicks a user.")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = null)
        {
            if (_moderationService.GetPermLevel(Context.DbGuild, userToKick) > 0)
            {
                ReplyError("You cannot kick another mod!");
            }

            await _moderationService.InformSubjectAsync(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();

            await SendAsync($"{Context.User.Boldify()} has successfully kicked {userToKick.Boldify()}.");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Kick", new Color(255, 114, 14), reason, Context.User, userToKick);
        }
    }
}
