using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
using MongoDB.Bson;
using DEA.Services.Handlers;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    [Require(Attributes.ServerOwner)]
    public class Owners : DEAModule
    {

        [Command("ResetUser")]
        [Summary("Resets all data for a specific user.")]
        public async Task ResetUser(IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            await DEABot.Users.DeleteOneAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId);
            await Reply($"Successfully reset {await ResponseMethods.NameAsync(user)}'s data.");
        }

        [Command("100k")]
        [Summary("Sets the user's balance to $100,000.00.")]
        public async Task HundredK(IGuildUser user= null)
        {
            user = user ?? Context.User as IGuildUser;
            await UserRepository.ModifyAsync(user, x => x.Cash, 100000);
            await RankHandler.HandleAsync(Context.Guild, user.Id);
            await Reply($"Successfully set {await ResponseMethods.NameAsync(user)}'s balance to $100,000.00.");
        }

        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        public async Task Add(IGuildUser user, decimal money)
        {
            if (money < 0) Error("You may not add negative money to a user's balance.");
            await UserRepository.EditCashAsync(user, money);
            await Reply($"Successfully added {money.ToString("C", Config.CI)} to {await ResponseMethods.NameAsync(user)}'s balance.");
        }

        [Command("AddTo")]
        [Summary("Add cash to every users balance in a specific role.")]
        public async Task Add(IRole role, decimal money)
        {
            if (money < 0) Error("You may not add negative money to these users's balances.");
            await Reply("The addition of cash has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await UserRepository.EditCashAsync(user, money);
            await Reply($"Successfully added {money.ToString("C", Config.CI)} to the balance of every user in the {role.Mention} role.");
        }

        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        public async Task Remove(IGuildUser user, decimal money)
        {
            if (money < 0) Error("You may not remove a negative amount of money from a user's balance.");
            await UserRepository.EditCashAsync(user, -money);
            await Reply($"Successfully removed {money.ToString("C", Config.CI)} from {await ResponseMethods.NameAsync(user)}'s balance.");
        }

        [Command("RemoveFrom")]
        [Summary("Remove cash to every users balance in a specific role.")]
        public async Task Remove(IRole role, decimal money)
        {
            if (money < 0) Error("You may not remove negative money from these users's balances.");
            await Reply("The cash removal has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await UserRepository.EditCashAsync(user, -money);
            await Reply($"Successfully removed {money.ToString("C", Config.CI)} from the balance of every user in the {role.Mention} role.");
        }

        [Command("Reset")]
        [Summary("Resets all user data for the entire server or a specific role.")]
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
        public async Task AddModRole(IRole modRole, int permissionLevel = 1)
        {
            if (permissionLevel < 1 || permissionLevel > 3) Error("Permission levels:\nModeration: 1\nAdministration: 2\nServer Owner: 3");
            if (Context.DbGuild.ModRoles.ElementCount == 0)
                await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.ModRoles, new BsonDocument()
                {
                    { modRole.Id.ToString(), permissionLevel }
                });
            else
            {
                if (Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString())) Error("You have already set this mod role.");
                Context.DbGuild.ModRoles.Add(modRole.Id.ToString(), permissionLevel);
                await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            }
            await Reply($"You have successfully added {modRole.Mention} as a moderation role with a permission level of {permissionLevel}.");
        }

        [Command("RemoveModRole")]
        [Summary("Removes a moderator role.")]
        public async Task RemoveModRole(IRole modRole)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0) Error("There are no moderator roles yet!");
            if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                Error("This role is not a moderator role!");
            Context.DbGuild.ModRoles.Remove(modRole.Id.ToString());
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            await Reply($"You have successfully removed the {modRole.Mention} moderator role.");
        }

        [Command("ModifyModRole")]
        [Summary("Modfies a moderator role.")]
        public async Task ModifyRank(IRole modRole, int permissionLevel)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0) Error("There are no moderator roles yet!");
            if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                Error("This role is not a moderator role!");
            if (Context.DbGuild.ModRoles.First(x => x.Name == modRole.Id.ToString()).Value == permissionLevel)
                Error($"This mod role already has a permission level of {permissionLevel}");
            Context.DbGuild.ModRoles[Context.DbGuild.ModRoles.IndexOfName(modRole.Id.ToString())] = permissionLevel;
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            await Reply($"You have successfully set the permission level of the {modRole.Mention} moderator role to {permissionLevel}.");
        }

        [Command("SetGlobalMultiplier")]
        [Summary("Sets the global chatting multiplier.")]
        public async Task SetGlobalMultiplier(decimal globalMultiplier){
            if (globalMultiplier < 0) Error("The global multiplier may not be negative.");
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.GlobalChattingMultiplier, globalMultiplier);
            await Reply($"You have successfully set the global chatting multiplier to {globalMultiplier.ToString("N2")}.");
        }
        
        [Command("SetRate")]
        [Summary("Sets the global temporary multiplier increase rate.")]
        public async Task SetMultiplierIncrease(decimal intrestRate){
            if (intrestRate < 0) Error("The temporary multiplier increase rate may not be negative.");
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.TempMultiplierIncreaseRate, intrestRate);
            await Reply($"You have successfully set the global temporary multiplier increase rate to {intrestRate.ToString("N2")}.");
        }

    }
}
