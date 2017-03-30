using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Repository;
using System.Linq;

namespace DEA.Modules
{
    [Require(Attributes.Admin)]
    public class Administration : ModuleBase<SocketCommandContext>
    {

        [Command("RoleIDs")]
        [Summary("Gets the ID of all roles in the guild.")]
        [Remarks("RoleIDs")]
        public async Task RoleIDs()
        {
            string message = null;
            foreach (var role in Context.Guild.Roles)
                message += $"{role.Name}: {role.Id}\n";
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(message);
            await ReplyAsync($"{Context.User.Mention}, all Role IDs have been DMed to you!");
        }

        [Command("SetPrefix")]
        [Summary("Sets the guild specific prefix.")]
        [Remarks("SetPrefix <Prefix>")]
        public async Task SetPrefix(string prefix)
        {
            if (prefix.Length > 3) throw new Exception("The maximum character length of a prefix is 3.");
            GuildRepository.Modify(x => x.Prefix = prefix, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the prefix to {prefix}!");
        }

        [Command("AddModRole")]
        [Summary("Adds a moderator role.")]
        [Remarks("AddModRole <@ModRole>")]
        public async Task AddModRole(IRole modRole)
        {
            GuildRepository.Modify(x => x.ModRoles.Add(modRole.Id), Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the moderator role to {modRole.Mention}!");
        }

        [Command("RemoveModRole")]
        [Summary("Removes a moderator role.")]
        [Remarks("RemoveModRole <@ModRole>")]
        public async Task RemoveModRole(IRole modRole)
        {
            if (!GuildRepository.FetchGuild(Context.Guild.Id).ModRoles.Any(x => x == modRole.Id))
                throw new Exception("This role is not a moderator role!");
            GuildRepository.Modify(x => x.ModRoles.Remove(modRole.Id), Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the moderator role to {modRole.Mention}!");
        }

        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        [Remarks("SetMutedRole <@MutedRole>")]
        public async Task SetMutedRole(IRole mutedRole)
        {
            if (mutedRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                throw new Exception($"DEA must be higher in the heigharhy than {mutedRole.Mention}.");
            GuildRepository.Modify(x => x.MutedRoleId = mutedRole.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the muted role to {mutedRole.Mention}!");
        }

        [Command("AddRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        [Remarks("AddRank <@RankRole> <Cash required>")]
        public async Task AddRank(IRole rankRole, double cashRequired)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (rankRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                throw new Exception($"DEA must be higher in the heigharhy than {rankRole.Mention}.");
            if (guild.RankRoles.Any(x => Convert.ToUInt64(x.Key) == rankRole.Id))
                throw new Exception("This role is already a rank role.");
            if (guild.RankRoles.Any(x => x.Value == cashRequired))
                throw new Exception("There is already a role set to that amount of cash required.");
            GuildRepository.Modify(x => x.RankRoles.Add(rankRole.ToString(), cashRequired), Context.Guild.Id);
            await ReplyAsync($"You have successfully added the {rankRole.Mention} rank!");
        }

        [Command("RemoveRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        [Remarks("AddRank <@RankRole> <Cash required>")]
        public async Task RemoveRank(IRole rankRole)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (!guild.RankRoles.Any(x => Convert.ToUInt64(x.Key) == rankRole.Id))
                throw new Exception("This role is not a rank role.");
            GuildRepository.Modify(x => x.RankRoles.Remove(rankRole.Id.ToString()), Context.Guild.Id);
            await ReplyAsync($"You have successfully added the {rankRole.Mention} rank!");
        }

        [Command("SetModLog")]
        [Summary("Sets the moderation log.")]
        [Remarks("SetModLog <#ModLog>")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            GuildRepository.Modify(x => x.ModLogId = modLogChannel.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the moderator log channel to {modLogChannel.Mention}!");
        }

        [Command("SetDetailedLogs")]
        [Summary("Sets the detailed logs.")]
        [Remarks("SetDetailedLogs <#DetailsLogs>")]
        public async Task SetDetailedLogsChannel(ITextChannel detailedLogsChannel)
        {
            GuildRepository.Modify(x => x.DetailedLogsId = detailedLogsChannel.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the detailed logs channel to {detailedLogsChannel.Mention}!");
        }

        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        [Remarks("SetGambleChannel <#GambleChannel>")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            GuildRepository.Modify(x => x.GambleId = gambleChannel.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully set the gamble channel to {gambleChannel.Mention}!");
        }

    }
}
