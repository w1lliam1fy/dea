using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
using MongoDB.Bson;
using DEA.Resources;

namespace DEA.Modules
{
    [Require(Attributes.ServerOwner)]
    public class Owners : DEAModule
    {
        [Command("ResetCooldowns")]
        [Summary("Resets all cooldowns for a specific user.")]
        [Remarks("ResetCooldowns [@User]")]
        public async Task ResetCooldowns(IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            var time = DateTime.UtcNow.AddYears(-1);
            UserRepository.Modify(DEABot.UserUpdateBuilder.Combine(
                DEABot.UserUpdateBuilder.Set(x => x.Message, time),
                DEABot.UserUpdateBuilder.Set(x => x.Whore, time),
                DEABot.UserUpdateBuilder.Set(x => x.Jump, time),
                DEABot.UserUpdateBuilder.Set(x => x.Steal, time),
                DEABot.UserUpdateBuilder.Set(x => x.Rob, time),
                DEABot.UserUpdateBuilder.Set(x => x.Withdraw, time)), user.Id, Context.Guild.Id);
            await Reply($"Successfully reset all of {user.Mention} cooldowns.");
        }

        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        [Remarks("Add <@User> <Cash>")]
        public async Task Give(IGuildUser userMentioned, decimal money)
        {
            if (money < 0) throw new Exception("You may not add negative money to a user's balance.");
            await UserRepository.EditCashAsync(Context, userMentioned.Id, money);
            await Reply($"Successfully added {money.ToString("C", Config.CI)} to {userMentioned.Mention}'s balance.");
        }

        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        [Remarks("Remove <@User> <Cash>")]
        public async Task Remove(IGuildUser userMentioned, decimal money)
        {
            if (money < 0) throw new Exception("You may not remove a negative amount of money from a user's balance.");
            await UserRepository.EditCashAsync(Context, userMentioned.Id, -money);
            await Reply($"Successfully removed {money.ToString("C", Config.CI)} from {userMentioned.Mention}'s balance.");
        }

        [Command("Reset")]
        [Summary("Resets all user data for the entire server or a specific role.")]
        [Remarks("Reset [@Role]")]
        public async Task Remove(IRole role = null)
        {
            if (role == null)
            {
                DEABot.Users.DeleteMany(x => x.GuildId == Context.Guild.Id);
                DEABot.Gangs.DeleteMany(y => y.GuildId == Context.Guild.Id);
                await Reply("Successfully reset all data in your server!");
            }
            else
            {
                foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                    DEABot.Users.DeleteOne(y => y.UserId == user.Id && y.GuildId == user.Guild.Id);
                await Reply($"Successfully reset all users with the {role.Mention} role!");
            }
        }

        [Command("AddModRole")]
        [Summary("Adds a moderator role.")]
        [Remarks("AddModRole <@ModRole>")]
        public async Task AddModRole(IRole modRole, int permissionLevel = 1)
        {
            if (permissionLevel < 1 || permissionLevel > 3) throw new Exception("Permission levels:\nModeration: 1\nAdministration: 2\nServer Owner: 3");
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (guild.ModRoles == null)
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, new BsonDocument()
                {
                    { modRole.Id.ToString(), permissionLevel }
                }), Context.Guild.Id);
            else
            {
                guild.ModRoles.Add(modRole.Id.ToString(), permissionLevel);
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, guild.ModRoles), Context.Guild.Id);
            }
            await Reply($"You have successfully add {modRole.Mention} as a moderation role with a permission level of {permissionLevel}.");
        }

        [Command("RemoveModRole")]
        [Summary("Removes a moderator role.")]
        [Remarks("RemoveModRole <@ModRole>")]
        public async Task RemoveModRole(IRole modRole)
        {
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);
            if (guild.ModRoles == null) throw new Exception("There are no moderator roles yet!");
            if (!guild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                throw new Exception("This role is not a moderator role!");
            guild.ModRoles.Remove(modRole.Id.ToString());
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, guild.ModRoles), Context.Guild.Id);
            await Reply($"You have successfully set the moderator role to {modRole.Mention}!");
        }
        [Command("SetGlobalMultiplier")]
        [Summary("Sets the global chatting multiplier.")]
        [Remarks("SetGlobalMultiplier <amount>")]
        public async Task SetGlobalMultiplier(decimal amount){
            if (amount < 1) throw new Exception("Global chatting multiplier cannot be less than 1");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.GlobalChattingMultiplier, amount), Context.Guild.Id);
            await Reply($"You have successfully set the global chatting multiplier to {amount.ToString("C2", Config.CI)}!");
        }
        [Command("SetMultiplierIncrease")]
        [Summary("Sets the global temporary multiplier increase rate")]
        [Remarks("SetMultiplierIncrease <amount>)"]
        public async Task SetMultiplierIncrease(decimal amount){
            if (amount <= 0) throw new Exception("Temporary multiplier increase rate cannot be less than 0");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.TempMultiplierIncreaseRate, amount), Context.Guild.Id);
            await Reply($"You have successfully set the global temporary multiplier increase rate to {amount.ToString("C2", Config.CI)}!");
        }

    }
}
