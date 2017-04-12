using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
using MongoDB.Bson;
using DEA.Resources;
using DEA.Services.Handlers;

namespace DEA.Modules
{
    [Require(Attributes.ServerOwner)]
    public class Owners : DEAModule
    {
        protected override void BeforeExecute()
        {
            InitializeData();
        }

        [Command("ResetCooldowns")]
        [Summary("Resets all cooldowns for a specific user.")]
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

        [Command("100k")]
        [Summary("Sets the user's balance to $100,000.00.")]
        public async Task HundredK(IGuildUser user= null)
        {
            user = user ?? Context.User as IGuildUser;
            UserRepository.Modify(DEABot.UserUpdateBuilder.Set(x => x.Cash, 100000), user.Id, Context.Guild.Id);
            await RankHandler.Handle(Context.Guild, user.Id);
            await Reply($"Successfully set {user.Mention}'s balance to $100,000.00.");
        }

        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        public async Task Add(IGuildUser user, decimal money)
        {
            if (money < 0) Error("You may not add negative money to a user's balance.");
            await UserRepository.EditCashAsync(Context, user.Id, money);
            await Reply($"Successfully added {money.ToString("C", Config.CI)} to {user.Mention}'s balance.");
        }

        [Command("AddTo")]
        [Summary("Add cash to every users balance in a specific role.")]
        public async Task Add(IRole role, decimal money)
        {
            if (money < 0) Error("You may not add negative money to these users's balances.");
            await Reply("The addition of cash has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await UserRepository.EditCashAsync(Context, user.Id, money);
            await Reply($"Successfully added {money.ToString("C", Config.CI)} to the balance of every user in the {role.Mention} role.");
        }

        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        public async Task Remove(IGuildUser user, decimal money)
        {
            if (money < 0) Error("You may not remove a negative amount of money from a user's balance.");
            await UserRepository.EditCashAsync(Context, user.Id, -money);
            await Reply($"Successfully removed {money.ToString("C", Config.CI)} from {user.Mention}'s balance.");
        }

        [Command("RemoveFrom")]
        [Summary("Remove cash to every users balance in a specific role.")]
        public async Task Remove(IRole role, decimal money)
        {
            if (money < 0) Error("You may not remove negative money from these users's balances.");
            await Reply("The cash removal has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await UserRepository.EditCashAsync(Context, user.Id, -money);
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
            if (DbGuild.ModRoles == null)
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, new BsonDocument()
                {
                    { modRole.Id.ToString(), permissionLevel }
                }), Context.Guild.Id);
            else
            {
                if (DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString())) Error("You have already set this mod role.");
                DbGuild.ModRoles.Add(modRole.Id.ToString(), permissionLevel);
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, DbGuild.ModRoles), Context.Guild.Id);
            }
            await Reply($"You have successfully added {modRole.Mention} as a moderation role with a permission level of {permissionLevel}.");
        }

        [Command("RemoveModRole")]
        [Summary("Removes a moderator role.")]
        public async Task RemoveModRole(IRole modRole)
        {
            if (DbGuild.ModRoles == null) Error("There are no moderator roles yet!");
            if (!DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                Error("This role is not a moderator role!");
            DbGuild.ModRoles.Remove(modRole.Id.ToString());
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.ModRoles, DbGuild.ModRoles), Context.Guild.Id);
            await Reply($"You have successfully removed the {modRole.Mention} moderation role.");
        }
        
        [Command("SetGlobalMultiplier")]
        [Summary("Sets the global chatting multiplier.")]
        public async Task SetGlobalMultiplier(decimal globalMultiplier){
            if (globalMultiplier < 0) Error("The global multiplier may not be negative.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.GlobalChattingMultiplier, globalMultiplier), Context.Guild.Id);
            await Reply($"You have successfully set the global chatting multiplier to {globalMultiplier.ToString("N2")}.");
        }
        
        [Command("SetRate")]
        [Summary("Sets the global temporary multiplier increase rate.")]
        public async Task SetMultiplierIncrease(decimal intrestRate){
            if (intrestRate < 0) Error("The temporary multiplier increase rate may not be negative.");
            GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.TempMultiplierIncreaseRate, intrestRate), Context.Guild.Id);
            await Reply($"You have successfully set the global temporary multiplier increase rate to {intrestRate.ToString("N2")}.");
        }

    }
}
