using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Remarks("\"Sexy John#0007\" Being too sexy for this server")]
        [Summary("Bans a user.")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = null)
        {
            if (_moderationService.GetPermLevel(Context.DbGuild, Context.GUser) <= _moderationService.GetPermLevel(Context.DbGuild, userToBan))
            {
                ReplyError("You cannot ban another mod with a permission level higher or equal to your own.");
            }

            await _moderationService.TryInformSubjectAsync(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);

            await SendAsync($"{Context.User.Boldify()} has successfully banned {userToBan.Boldify()}.");

            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Ban", Config.ErrorColor, reason, Context.User, userToBan);
        }
    }
}
