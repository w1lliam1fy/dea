using DEA.Database.Repository;
using Discord;
using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using DEA.Services;
using DEA.Resources;

namespace DEA.Modules
{
    [Require(Attributes.Moderator)]
    public class Moderation : DEAModule
    {
        [Command("Ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Bans a user from the server.")]
        [Remarks("Ban <@User> [Reason]")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = "No reason.")
        {
            if (ModuleMethods.IsMod(Context, userToBan)) throw new Exception("You cannot ban another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);
            await Logger.ModLog(Context, "Ban", new Color(255, 0, 0), reason, userToBan);
            await Send($"{Context.User.Mention} has successfully banned {userToBan.Mention}!");
        }

        [Command("Kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kicks a user from the server.")]
        [Remarks("Kick <@User> [Reason]")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = "No reason.")
        {
            if (ModuleMethods.IsMod(Context, userToKick)) throw new Exception("You cannot kick another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();
            await Logger.ModLog(Context, "Kick", new Color(255, 114, 14), reason, userToKick);
            await Send($"{Context.User.Mention} has successfully kicked {userToKick.Mention}!");
        }

        [Command("Mute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user.")]
        [Remarks("Mute <@User> [Reason]")]
        public async Task Mute(IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            var mutedRole = Context.Guild.GetRole(guild.MutedRoleId);
            if (mutedRole == null) throw new Exception($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                       $"`{guild.Prefix}SetMutedRole` command to change that.");
            if (ModuleMethods.IsMod(Context, userToMute)) throw new Exception("You cannot mute another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await userToMute.AddRoleAsync(mutedRole);
            MuteRepository.AddMute(userToMute.Id, Context.Guild.Id, Config.DEFAULT_MUTE_TIME);
            await Logger.ModLog(Context, "Mute", new Color(255, 114, 14), reason, userToMute, $"\n**Length:** {Config.DEFAULT_MUTE_TIME.TotalHours} hours");
            await Send($"{Context.User.Mention} has successfully muted {userToMute.Mention}!");
        }

        [Command("CustomMute")]
        [Alias("CMute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user for x amount of hours.")]
        [Remarks("CustomMute <Hours> <@User> [Reason]")]
        public async Task CustomMute(double hours, IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            if (hours > 168) throw new Exception("You may not mute a user for more than a week.");
            if (hours < 1) throw new Exception("You may not mute a user for less than 1 hour.");
            string time = "hours";
            if (hours == 1) time = "hour";
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            var mutedRole = Context.Guild.GetRole(guild.MutedRoleId);
            if (mutedRole == null) throw new Exception($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                       $"{guild.Prefix}SetMutedRole command to change that.");
            if (ModuleMethods.IsMod(Context, userToMute)) throw new Exception("You cannot mute another mod!");
            await ModuleMethods.InformSubjectAsync(Context.User, "Mute", userToMute, reason);
            await userToMute.AddRoleAsync(mutedRole);
            MuteRepository.AddMute(userToMute.Id, Context.Guild.Id, TimeSpan.FromHours(hours));
            await Logger.ModLog(Context, "Mute", new Color(255, 114, 14), reason, userToMute, $"\n**Length:** {hours} {time}");
            await Send($"{Context.User.Mention} has successfully muted {userToMute.Mention} for {hours} {time}!");
        }

        [Command("Unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Unmutes a muted user.")]
        [Remarks("Unmute <@User> [Reason]")]
        public async Task Unmute(IGuildUser userToUnmute, [Remainder] string reason = "No reason.")
        {
            var mutedRoleId = GuildRepository.FetchGuild(Context.Guild.Id).MutedRoleId;
            if (userToUnmute.RoleIds.All(x => x != mutedRoleId)) throw new Exception("You cannot unmute a user who isn't muted.");
            await ModuleMethods.InformSubjectAsync(Context.User, "Unmute", userToUnmute, reason);
            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(mutedRoleId));
            MuteRepository.RemoveMute(userToUnmute.Id, Context.Guild.Id);
            await Logger.ModLog(Context, "Unmute", new Color(12, 255, 129), reason, userToUnmute);
            await Send($"{Context.User.Mention} has successfully unmuted {userToUnmute.Mention}!");
        }

        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Summary("Deletes x amount of messages.")]
        [Remarks("Clear [Quantity of messages]")]
        public async Task CleanAsync(int count = 25, [Remainder] string reason = "No reason.")
        {
            if (count < Config.MIN_CLEAR) throw new Exception($"You may not clear less than {Config.MIN_CLEAR} messages.");
            if (count > Config.MAX_CLEAR) throw new Exception($"You may not clear more than {Config.MAX_CLEAR} messages.");
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (Context.Channel.Id == guild.ModLogId || Context.Channel.Id == guild.DetailedLogsId)
                throw new Exception("For security reasons, you may not use this command in a log channel.");
            var messages = await Context.Channel.GetMessagesAsync(count).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            await Logger.ModLog(Context, "Clear", new Color(34, 59, 255), reason, null, $"\n**Quantity:** {count}");
        }

        [Command("Chill")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Summary("Prevents users from talking in a specific channel for x amount of seconds.")]
        [Remarks("Chill [Number of seconds]")]
        public async Task Chill(int seconds = 30, [Remainder] string reason = "No reason.")
        {
            if (seconds < Config.MIN_CHILL) throw new Exception($"You may not chill for less than {Config.MIN_CHILL} seconds.");
            if (seconds > Config.MAX_CHILL) throw new Exception("You may not chill for more than one hour.");
            var channel = Context.Channel as SocketTextChannel;
            var perms = channel.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value;
            if (perms.SendMessages == PermValue.Deny) throw new Exception("This chat is already chilled.");
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, PermValue.Deny));
            await Reply($"Chat just got cooled down. Won't heat up until at least {seconds} seconds have passed.");
            await Task.Delay(seconds * 1000);
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, perms.SendMessages));
            await Logger.ModLog(Context, "Chill", new Color(34, 59, 255), reason, null, $"\n**Length:** {seconds} seconds");
        }

    }
}