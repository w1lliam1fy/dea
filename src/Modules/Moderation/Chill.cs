using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using Discord.WebSocket;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Chill")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Remarks("60 people raiding")]
        [Summary("Prevents users from talking in a specific channel for x amount of seconds.")]
        public async Task Chill(int seconds = 30, [Remainder] string reason = null)
        {
            if (seconds < Config.MIN_CHILL.TotalSeconds)
            {
                ReplyError($"You may not chill for less than {Config.MIN_CHILL.TotalSeconds} seconds.");
            }
            else if (seconds > Config.MAX_CHILL.TotalSeconds)
            {
                ReplyError($"You may not chill for more than {Config.MAX_CHILL.TotalSeconds} seconds.");
            }

            var channel = Context.Channel as SocketTextChannel;
            var nullablePermOverwrites = channel.GetPermissionOverwrite(Context.Guild.EveryoneRole);

            var perms = nullablePermOverwrites ?? new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);

            if (perms.SendMessages == PermValue.Deny)
            {
                ReplyError("This chat is already chilled.");
            }

            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, PermValue.Deny));

            await ReplyAsync($"Chat just got cooled down. Won't heat up until at least {seconds} seconds have passed.");

            await Task.Delay(seconds * 1000);
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, perms.SendMessages));

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Chill", new Color(34, 59, 255), reason, Context.User, null, "Length", $"{seconds} seconds");
        }
    }
}
