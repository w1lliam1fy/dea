using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repository;
using System.Linq;
using MongoDB.Bson;
using DEA.Resources;
using DEA.Services;

namespace DEA.Modules
{
    [Require(Attributes.Admin)]
    public class Administration : DEAModule
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
            await ResponseMethods.DM(channel, message);
            await Reply("All Role IDs have been DMed to you!");
        }

        [Command("SetPrefix")]
        [Summary("Sets the guild specific prefix.")]
        [Remarks("SetPrefix <Prefix>")]
        public async Task SetPrefix(string prefix)
        {
            if (prefix.Length > 3) await Error("The maximum character length of a prefix is 3.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.Prefix, prefix), Context.Guild.Id);
            await Reply($"You have successfully set the prefix to {prefix}!");
        }

        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        [Remarks("SetMutedRole <@MutedRole>")]
        public async Task SetMutedRole(IRole mutedRole)
        {
            if (mutedRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                await Error($"DEA must be higher in the heigharhy than {mutedRole.Mention}.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.MutedRoleId, mutedRole.Id), Context.Guild.Id);
            await Reply($"You have successfully set the muted role to {mutedRole.Mention}!");
        }

        [Command("AddRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        [Remarks("AddRank <@RankRole> <Cash required>")]
        public async Task AddRank(IRole rankRole, double cashRequired)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (rankRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                await Error($"DEA must be higher in the heigharhy than {rankRole.Mention}.");
            if (guild.RankRoles == null)
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, new BsonDocument()
                {
                    { rankRole.Id.ToString(), cashRequired }
                }), Context.Guild.Id);
            else
            {
                if (guild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                    await Error("This role is already a rank role.");
                if (guild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)cashRequired))
                    await Error("There is already a role set to that amount of cash required.");
                guild.RankRoles.Add(rankRole.Id.ToString(), cashRequired);
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, guild.RankRoles), Context.Guild.Id);
            }
            
            await Reply($"You have successfully added the {rankRole.Mention} rank!");
        }

        [Command("RemoveRank")]
        [Summary("Removes a rank role for the DEA cash system.")]
        [Remarks("RemoveRank <@RankRole>")]
        public async Task RemoveRank(IRole rankRole)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (guild.RankRoles == null) await Error("There are no ranks yet!");
            if (!guild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                await Error("This role is not a rank role.");
            guild.RankRoles.Remove(rankRole.Id.ToString());
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, guild.RankRoles), Context.Guild.Id);
            await Reply($"You have successfully removed the {rankRole.Mention} rank!");
        }

        [Command("SetModLog")]
        [Summary("Sets the moderation log.")]
        [Remarks("SetModLog <#ModLog>")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModLogId, modLogChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the moderator log channel to {modLogChannel.Mention}!");
        }

        [Command("SetDetailedLogs")]
        [Summary("Sets the detailed logs.")]
        [Remarks("SetDetailedLogs <#DetailsLogs>")]
        public async Task SetDetailedLogsChannel(ITextChannel detailedLogsChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.DetailedLogsId, detailedLogsChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the detailed logs channel to {detailedLogsChannel.Mention}!");
        }

        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        [Remarks("SetGambleChannel <#GambleChannel>")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.GambleId, gambleChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the gamble channel to {gambleChannel.Mention}!");
        }

    }
}
