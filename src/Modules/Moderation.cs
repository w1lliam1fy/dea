using DEA.Database.Repository;
using Discord;
using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using DEA.Services;
using DEA.Common;

namespace DEA.Modules
{
    [Require(Attributes.Moderator)]
    public class Moderation : DEAModule
    {
        protected override void BeforeExecute()
        {
            InitializeData();
        }

        [Command("Ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Bans a user from the server.")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = "No reason.")
        {
            if (!ModuleMethods.IsHigherMod(Context.User as IGuildUser, userToBan)) Error("You cannot ban another mod with a permission level higher or equal to your own!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);
            await Logger.ModLog(Context, "Ban", new Color(255, 0, 0), reason, userToBan);
            await Send($"{(Context.User as IGuildUser).Nickname} has successfully banned {userToBan.Mention}!");
        }

        [Command("Kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kicks a user from the server.")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = "No reason.")
        {
            if (ModuleMethods.IsMod(userToKick)) Error("You cannot kick another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();
            await Logger.ModLog(Context, "Kick", new Color(255, 114, 14), reason, userToKick);
            await Send($"{(Context.User as IGuildUser).Nickname} has successfully kicked {userToKick.Mention}!");
        }

        [Command("Mute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Permanently mutes a user.")]
        public async Task Mute(IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            var mutedRole = Context.Guild.GetRole(DbGuild.MutedRoleId);
            if (mutedRole == null) Error($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                       $"`{Prefix}SetMutedRole` command to change that.");
            if (ModuleMethods.IsMod(userToMute)) Error("You cannot mute another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await userToMute.AddRoleAsync(mutedRole);
            MuteRepository.AddMute(userToMute.Id, Context.Guild.Id, TimeSpan.FromDays(365));
            await Logger.ModLog(Context, "Mute", new Color(255, 114, 14), reason, userToMute, null);
            await Send($"{(Context.User as IGuildUser).Nickname} has successfully muted {userToMute.Mention}!");
        }

        [Command("CustomMute")]
        [Alias("CMute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user for x amount of hours.")]
        public async Task CustomMute(double hours, IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            if (hours > 168) Error("You may not mute a user for more than a week.");
            if (hours < 1) Error("You may not mute a user for less than 1 hour.");
            string time = (hours == 1) ? "hour" : "hours";
            var mutedRole = Context.Guild.GetRole(DbGuild.MutedRoleId);
            if (mutedRole == null) Error($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                       $"{DbGuild.Prefix}SetMutedRole command to change that.");
            if (ModuleMethods.IsMod(userToMute)) Error("You cannot mute another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await userToMute.AddRoleAsync(mutedRole);
            MuteRepository.AddMute(userToMute.Id, Context.Guild.Id, TimeSpan.FromHours(hours));
            await Logger.ModLog(Context, "Mute", new Color(255, 114, 14), reason, userToMute, $"\n**Length:** {hours} {time}");
            await Send($"{(Context.User as IGuildUser).Nickname} has successfully muted {userToMute.Mention} for {hours} {time}!");
        }

        [Command("Unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Unmutes a muted user.")]
        public async Task Unmute(IGuildUser userToUnmute, [Remainder] string reason = "No reason.")
        {
            if (userToUnmute.RoleIds.All(x => x != DbGuild.MutedRoleId)) Error("You cannot unmute a user who isn't muted.");
            await ModuleMethods.InformSubjectAsync(Context.User, "Unmute", userToUnmute, reason);
            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(DbGuild.MutedRoleId));
            MuteRepository.RemoveMute(userToUnmute.Id, Context.Guild.Id);
            await Logger.ModLog(Context, "Unmute", new Color(12, 255, 129), reason, userToUnmute);
            await Send($"{(Context.User as IGuildUser).Nickname} has successfully unmuted {userToUnmute.Mention}!");
        }

        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Summary("Deletes x amount of messages.")]
        public async Task CleanAsync(int quantity = 25, [Remainder] string reason = "No reason.")
        {
            if (quantity < Config.MIN_CLEAR) Error($"You may not clear less than {Config.MIN_CLEAR} messages.");
            if (quantity > Config.MAX_CLEAR) Error($"You may not clear more than {Config.MAX_CLEAR} messages.");
            if (Context.Channel.Id == DbGuild.ModLogId || Context.Channel.Id == DbGuild.DetailedLogsId)
                Error("For security reasons, you may not use this command in a log channel.");
            var messages = await Context.Channel.GetMessagesAsync(quantity).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            await Logger.ModLog(Context, "Clear", new Color(34, 59, 255), reason, null, $"\n**Quantity:** {quantity}");
            var msg = await Reply($"Messages deleted: **{quantity}**.");
            await Task.Delay(2500);
            await msg.DeleteAsync();
        }

        [Command("Chill")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Summary("Prevents users from talking in a specific channel for x amount of seconds.")]
        public async Task Chill(int seconds = 30, [Remainder] string reason = "No reason.")
        {
            if (seconds < Config.MIN_CHILL) Error($"You may not chill for less than {Config.MIN_CHILL} seconds.");
            if (seconds > Config.MAX_CHILL) Error("You may not chill for more than one hour.");
            var channel = Context.Channel as SocketTextChannel;
            var perms = channel.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value;
            if (perms.SendMessages == PermValue.Deny) Error("This chat is already chilled.");
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, PermValue.Deny));
            await Reply($"Chat just got cooled down. Won't heat up until at least {seconds} seconds have passed.");
            await Task.Delay(seconds * 1000);
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, perms.SendMessages));
            await Logger.ModLog(Context, "Chill", new Color(34, 59, 255), reason, null, $"\n**Length:** {seconds} seconds");
        }

    }
}