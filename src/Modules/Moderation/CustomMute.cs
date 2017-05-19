using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("CustomMute")]
        [Alias("CMute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user for x amount of hours.")]
        public async Task CustomMute([Summary("2")] double hours, IGuildUser userToMute, [Remainder] string reason = null)
        {
            if (hours > 168)
            {
                ReplyError("You may not mute a user for more than a week.");
            }
            else if (hours < 1)
            {
                ReplyError("You may not mute a user for less than 1 hour.");
            }

            string time = (hours == 1) ? "hour" : "hours";
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                ReplyError($"You may not mute users if the muted role is not valid.\nPlease use the " +
                           $"{Context.DbGuild.Prefix}SetMutedRole command to change that.");
            }
            else if (_moderationService.GetPermLevel(Context, userToMute) > 0)
            {
                ReplyError("You cannot mute another mod.");
            }

            await userToMute.AddRoleAsync(mutedRole);
            await _muteRepo.InsertMuteAsync(userToMute, TimeSpan.FromHours(hours));

            await SendAsync($"{Context.User.Boldify()} has successfully muted {userToMute.Boldify()} for {hours} {time}.");

            await _moderationService.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await _moderationService.ModLogAsync(Context, "Mute", new Color(255, 114, 14), reason, userToMute, $"\n**Length:** {hours} {time}");
        }
    }
}
