using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Database.Repositories;
using System.Linq;
using MongoDB.Bson;
using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules
{
    [Require(Attributes.Admin)]
    public class Administration : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public Administration(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        [Command("RoleIDs")]
        [Summary("Gets the ID of all roles in the guild.")]
        public async Task RoleIDs()
        {
            string message = null;
            foreach (var role in Context.Guild.Roles)
            {
                message += $"{role.Name}: {role.Id}\n";
            }

            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendAsync(message);

            await ReplyAsync("All Role IDs have been DMed to you.");
        }

        [Command("SetPrefix")]
        [Summary("Sets the guild specific prefix.")]
        public async Task SetPrefix([Summary("!")] string prefix)
        {
            if (prefix.Length > 3)
            {
                ReplyError("The maximum character length of a prefix is 3.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Prefix = prefix);

            await ReplyAsync($"You have successfully set the prefix to {prefix}.");
        }

        [Command("SetWelcomeChannel")]
        [Summary("Set the channel where DEA will send a welcome message to all new users that join.")]
        public async Task SetWelcomeChannel([Remainder] ITextChannel channel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeChannelId = channel.Id);
            await ReplyAsync($"You have successfully set the welcome channel to {channel.Mention}.");
        }

        [Command("SetWelcomeMessage")]
        [Alias("SetWelcome")]
        [Summary("Set the channel where DEA will send a welcome message to all new users that join.")]
        public async Task SetWelcomeMessage([Summary("WELCOME FELLOW USER!")] [Remainder] string message)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = message);
            await ReplyAsync($"You have successfully set the welcome message.");
        }

        [Command("DisableWelcomeMessage")]
        [Alias("DisableWelcome")]
        [Summary("Disables the welcome message from being sent in direct messages and in the welcome channel.")]
        public async Task DisableWelcomeMessage()
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = string.Empty);
            await ReplyAsync("You have successfully disabled the welcome message. If ever change your mind, " +
                             "simply setting the welcome message again will enable this feature.");
        }

        [Command("SetUpdateChannel")]
        [Summary("Sets the channel where DEA will send messages informing you of its most recent updates and new features.")]
        public async Task SetUpdateChannel([Remainder] ITextChannel channel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.UpdateChannelId = channel.Id);
            await ReplyAsync($"You have successfully set the DEA update channel to {channel.Mention}.");
        }

        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        public async Task SetMutedRole([Remainder] IRole mutedRole)
        {
            if (mutedRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                ReplyError($"DEA must be higher in the heigharhy than {mutedRole.Mention}.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.MutedRoleId = mutedRole.Id);

            await ReplyAsync($"You have successfully set the muted role to {mutedRole.Mention}.");
        }

        [Command("AddRank")]
        [Summary("Adds a rank role for the DEA cash system.")]
        public async Task AddRank(IRole rankRole, double cashRequired = 500)
        {
            if (rankRole.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                ReplyError($"DEA must be higher in the heigharhy than {rankRole.Mention}.");
            }

            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(rankRole.Id.ToString(), cashRequired));
            }
            else
            {
                if (Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
                {
                    ReplyError("This role is already a rank role.");
                }
                if (cashRequired == 500)
                {
                    cashRequired = Context.DbGuild.RankRoles.OrderByDescending(x => x.Value).First().Value.AsDouble * 2;
                }
                if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)cashRequired))
                {
                    ReplyError("There is already a role set to that amount of cash required.");
                }

                Context.DbGuild.RankRoles.Add(rankRole.Id.ToString(), cashRequired);

                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(rankRole.Id.ToString(), cashRequired));
            }
            
            await ReplyAsync($"You have successfully added the {rankRole.Mention} rank.");
        }

        [Command("RemoveRank")]
        [Summary("Removes a rank role for the DEA cash system.")]
        public async Task RemoveRank([Remainder] IRole rankRole)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                ReplyError("There are no ranks yet.");
            }
            else if (!Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
            {
                ReplyError("This role is not a rank role.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles.Remove(rankRole.Id.ToString()));

            await ReplyAsync($"You have successfully removed the {rankRole.Mention} rank.");
        }

        [Command("ModifyRank")]
        [Summary("Modfies a rank role for the DEA cash system.")]
        public async Task ModifyRank(IRole rankRole, double newCashRequired)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                ReplyError("There are no ranks yet.");
            }
            else if (!Context.DbGuild.RankRoles.Any(x => x.Name == rankRole.Id.ToString()))
            {
                ReplyError("This role is not a rank role.");
            }
            else if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)newCashRequired))
            {
                ReplyError("There is already a role set to that amount of cash required.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.RankRoles[rankRole.Id.ToString()] = newCashRequired);

            await ReplyAsync($"You have successfully set the cash required for the {rankRole.Mention} rank to {((decimal)newCashRequired).USD()}.");
        }

        [Command("SetModLog")]
        [Summary("Sets the moderation log.")]
        public async Task SetModLogChannel([Remainder] ITextChannel modLogChannel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModLogChannelId = modLogChannel.Id);
            await ReplyAsync($"You have successfully set the moderator log channel to {modLogChannel.Mention}.");
        }

        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        public async Task SetGambleChannel([Remainder] ITextChannel gambleChannel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.GambleChannelId = gambleChannel.Id);
            await ReplyAsync($"You have successfully set the gamble channel to {gambleChannel.Mention}.");
        }

    }
}
