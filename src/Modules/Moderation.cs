using DEA.Database.Repositories;
using Discord;
using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    [Require(Attributes.Moderator)]
    public class Moderation : DEAModule
    {
        private readonly MuteRepository _muteRepo;
        private readonly ModerationService _moderationService;

        public Moderation(MuteRepository muteRepo, ModerationService moderationService)
        {
            _muteRepo = muteRepo;
            _moderationService = moderationService;
        }

        [Command("Ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Bans a user.")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = null)
        {
            if (!await _moderationService.IsHigherModAsync(Context, Context.GUser, userToBan))
                await ErrorAsync("You cannot ban another mod with a permission level higher or equal to your own!");

            await _moderationService.InformSubjectAsync(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);
            await _moderationService.ModLogAsync(Context, "Ban", Config.ERROR_COLOR, reason, userToBan);

            await SendAsync($"{Context.User} has successfully banned {userToBan}.");
        }

        [Command("Unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Unban a user.")]
        public async Task Unban(IUser user, [Remainder] string reason = null)
        {
            await Context.Guild.RemoveBanAsync(user);
            await _moderationService.InformSubjectAsync(Context.User, "Unban", user, reason);
            await _moderationService.ModLogAsync(Context, "Unban", Config.ERROR_COLOR, reason, user);

            await SendAsync($"{Context.User} has successfully unbanned {user}.");
        }

        [Command("Kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kicks a user.")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = null)
        {
            if (await _moderationService.IsModAsync(Context, userToKick))
                await ErrorAsync("You cannot kick another mod!");

            await _moderationService.InformSubjectAsync(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();
            await _moderationService.ModLogAsync(Context, "Kick", new Color(255, 114, 14), reason, userToKick);

            await SendAsync($"{Context.User} has successfully kicked {userToKick}.");
        }

        [Command("Mute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Permanently mutes a user.")]
        public async Task Mute(IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
                await ErrorAsync($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                 $"`{Context.Prefix}SetMutedRole` command to change that.");
            if (await _moderationService.IsModAsync(Context, userToMute))
                await ErrorAsync("You cannot mute another mod.");

            await userToMute.AddRoleAsync(mutedRole);
            await _muteRepo.AddMuteAsync(userToMute, TimeSpan.FromDays(365));
            await _moderationService.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await _moderationService.ModLogAsync(Context, "Mute", new Color(255, 114, 14), reason, userToMute, null);

            await SendAsync($"{Context.User} has successfully muted {userToMute}.");
        }

        [Command("CustomMute")]
        [Alias("CMute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user for x amount of hours.")]
        public async Task CustomMute(double hours, IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            if (hours > 168)
                await ErrorAsync("You may not mute a user for more than a week.");
            if (hours < 1)
                await ErrorAsync("You may not mute a user for less than 1 hour.");

            string time = (hours == 1) ? "hour" : "hours";
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
                await ErrorAsync($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                 $"{Context.DbGuild.Prefix}SetMutedRole command to change that.");
            if (await _moderationService.IsModAsync(Context, userToMute))
                await ErrorAsync("You cannot mute another mod!");

            await userToMute.AddRoleAsync(mutedRole);
            await _muteRepo.AddMuteAsync(userToMute, TimeSpan.FromHours(hours));
            await _moderationService.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await _moderationService.ModLogAsync(Context, "Mute", new Color(255, 114, 14), reason, userToMute, $"\n**Length:** {hours} {time}");

            await SendAsync($"{Context.User} has successfully muted {userToMute} for {hours} {time}!");
        }

        [Command("Unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Unmutes a muted user.")]
        public async Task Unmute(IGuildUser userToUnmute, [Remainder] string reason = "No reason.")
        {
            if (!userToUnmute.RoleIds.Any(x => x == Context.DbGuild.MutedRoleId))
                await ErrorAsync("You cannot unmute a user who isn't muted.");
            
            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId));
            await _muteRepo.RemoveMuteAsync(userToUnmute.Id, userToUnmute.GuildId);
            await _moderationService.InformSubjectAsync(Context.User, "Unmute", userToUnmute, reason);
            await _moderationService.ModLogAsync(Context, "Unmute", new Color(12, 255, 129), reason, userToUnmute);

            await SendAsync($"{Context.User} has successfully unmuted {userToUnmute}!");
        }

        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Summary("Deletes x amount of messages.")]
        public async Task CleanAsync(int quantity = 25, [Remainder] string reason = "No reason.")
        {
            if (quantity < Config.MIN_CLEAR)
                await ErrorAsync($"You may not clear less than {Config.MIN_CLEAR} messages.");
            if (quantity > Config.MAX_CLEAR)
                await ErrorAsync($"You may not clear more than {Config.MAX_CLEAR} messages.");
            if (Context.Channel.Id == Context.DbGuild.ModLogId)
                await ErrorAsync("For security reasons, you may not use this command in the mod log channel.");

            var messages = await Context.Channel.GetMessagesAsync(quantity).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            await _moderationService.ModLogAsync(Context, "Clear", new Color(34, 59, 255), reason, null, $"\n**Quantity:** {quantity}");

            var msg = await ReplyAsync($"Messages deleted: **{quantity}**.");

            await Task.Delay(2500);
            await msg.DeleteAsync();
        }

        [Command("Chill")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Summary("Prevents users from talking in a specific channel for x amount of seconds.")]
        public async Task Chill(int seconds = 30, [Remainder] string reason = "No reason.")
        {
            if (seconds < Config.MIN_CHILL.TotalSeconds)
                await ErrorAsync($"You may not chill for less than {Config.MIN_CHILL.TotalSeconds} seconds.");
            if (seconds > Config.MAX_CHILL.TotalSeconds)
                await ErrorAsync($"You may not chill for more than {Config.MAX_CHILL.TotalSeconds} seconds.");

            var channel = Context.Channel as SocketTextChannel;
            var nullablePermOverwrites = channel.GetPermissionOverwrite(Context.Guild.EveryoneRole);

            var perms = nullablePermOverwrites ?? new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);

            if (perms.SendMessages == PermValue.Deny)
                await ErrorAsync("This chat is already chilled.");

            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, PermValue.Deny));

            await ReplyAsync($"Chat just got cooled down. Won't heat up until at least {seconds} seconds have passed.");

            await Task.Delay(seconds * 1000);
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, perms.SendMessages));

            await _moderationService.ModLogAsync(Context, "Chill", new Color(34, 59, 255), reason, null, $"\n**Length:** {seconds} seconds");
        }

    }
}