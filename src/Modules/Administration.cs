using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Database.Repository;
using System.Linq;
using MongoDB.Bson;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    [Require(Attributes.Admin)]
    public class Administration : DEAModule
    {
        protected override void BeforeExecute()
        {
            InitializeData();
        }

        [Command("RoleIDs")]
        [Summary("Gets the ID of all roles in the guild.")]
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
        public async Task SetPrefix(string prefix)
        {
            if (prefix.Length > 3) Error("The maximum character length of a prefix is 3.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.Prefix, prefix), Context.Guild.Id);
            await Reply($"You have successfully set the prefix to {prefix}!");
        }

        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        public async Task SetMutedRole(IRole mutedRole)
        {
            if (mutedRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                Error($"DEA must be higher in the heigharhy than {mutedRole.Mention}.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.MutedRoleId, mutedRole.Id), Context.Guild.Id);
            await Reply($"You have successfully set the muted role to {mutedRole.Mention}!");
        }

        [Command("AddRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        public async Task AddRank(IRole rankRole, double cashRequired = 500)
        {
            if (rankRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                Error($"DEA must be higher in the heigharhy than {rankRole.Mention}.");
            if (DbGuild.RankRoles.ElementCount == 0)
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, new BsonDocument()
                {
                    { rankRole.Id.ToString(), cashRequired }
                }), Context.Guild.Id);
            else
            {
                if (DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                    Error("This role is already a rank role.");
                if (cashRequired == 500) cashRequired = DbGuild.RankRoles.OrderByDescending(x => x.Value).First().Value.AsDouble * 2;
                if (DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)cashRequired))
                    Error("There is already a role set to that amount of cash required.");
                DbGuild.RankRoles.Add(rankRole.Id.ToString(), cashRequired);
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, DbGuild.RankRoles), Context.Guild.Id);
            }
            
            await Reply($"You have successfully added the {rankRole.Mention} rank!");
        }

        [Command("RemoveRank")]
        [Summary("Removes a rank role for the DEA cash system.")]
        public async Task RemoveRank(IRole rankRole)
        {
            if (DbGuild.RankRoles.ElementCount == 0) Error("There are no ranks yet!");
            if (!DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                Error("This role is not a rank role.");
            DbGuild.RankRoles.Remove(rankRole.Id.ToString());
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, DbGuild.RankRoles), Context.Guild.Id);
            await Reply($"You have successfully removed the {rankRole.Mention} rank!");
        }

        [Command("ModifyRank")]
        [Summary("Modfies a rank role for the DEA cash system.")]
        public async Task ModifyRank(IRole rankRole, double newCashRequired)
        {
            if (DbGuild.RankRoles.ElementCount == 0) Error("There are no ranks yet!");
            if (!DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                Error("This role is not a rank role.");
            if (DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)newCashRequired))
                Error("There is already a role set to that amount of cash required.");
            DbGuild.RankRoles[DbGuild.RankRoles.IndexOfName(rankRole.Id.ToString())] = newCashRequired;
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, DbGuild.RankRoles), Context.Guild.Id);
            await Reply($"You have successfully set the cash required for the {rankRole.Mention} rank to {newCashRequired.ToString("C", Config.CI)}.");
        }

        [Command("SetModLog")]
        [Summary("Sets the moderation log.")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModLogId, modLogChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the moderator log channel to {modLogChannel.Mention}!");
        }

        [Command("SetDetailedLogs")]
        [Summary("Sets the detailed logs.")]
        public async Task SetDetailedLogsChannel(ITextChannel detailedLogsChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.DetailedLogsId, detailedLogsChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the detailed logs channel to {detailedLogsChannel.Mention}!");
        }

        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.GambleId, gambleChannel.Id), Context.Guild.Id);
            await Reply($"You have successfully set the gamble channel to {gambleChannel.Mention}!");
        }

    }
}
