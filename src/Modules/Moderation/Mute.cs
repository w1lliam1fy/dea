using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Mute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Remarks("\"Sexy John#0007\" Was so sexy others felt bad about themselves")]
        [Summary("Permanently mutes a user.")]
        public async Task Mute(IGuildUser userToMute, [Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                ReplyError($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                 $"`{Context.Prefix}SetMutedRole` command to change that.");
            }
            else if (_moderationService.GetPermLevel(Context.DbGuild, userToMute) > 0)
            {
                ReplyError("You cannot mute another mod.");
            }

            await userToMute.AddRoleAsync(mutedRole);
            await _muteRepo.InsertMuteAsync(userToMute, TimeSpan.FromDays(365));

            await SendAsync($"{Context.User.Boldify()} has successfully muted {userToMute.Boldify()}.");

            await _moderationService.TryInformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Mute", Config.MuteColor, reason, Context.User, userToMute);
        }
    }
}
