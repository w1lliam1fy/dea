using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using DEA.Database.Repository;
using MongoDB.Driver;
using MongoDB.Bson;
using DEA.Services.Handlers;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Models;
using DEA.Common.Extensions;

namespace DEA.Modules
{
    [Require(Attributes.ServerOwner)]
    public class Owners : DEAModule
    {
        private readonly GuildRepository _guildRepo;
        private readonly UserRepository _userRepo;
        private readonly RankHandler _rankHandler;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Gang> _gangs;

        public Owners(GuildRepository guildRepo, UserRepository userRepo, RankHandler rankHandler, IMongoCollection<User> users, IMongoCollection<Gang> gangs)
        {
            _guildRepo = guildRepo;
            _userRepo = userRepo;
            _rankHandler = rankHandler;
            _users = users;
            _gangs = gangs;
        }

        [Command("ResetUser")]
        [Summary("Resets all data for a specific user.")]
        public async Task ResetUser([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            await _users.DeleteOneAsync(y => y.UserId == user.Id && y.GuildId == user.GuildId);
            await _rankHandler.HandleAsync(Context.Guild, user, Context.DbGuild, await _userRepo.FetchUserAsync(user));
            await ReplyAsync($"Successfully reset {user}'s data.");
        }

        [Command("100k")]
        [Summary("Sets the user's balance to $100,000.00.")]
        public async Task HundredK([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            await _userRepo.ModifyAsync(user, x => x.Cash, 100000);
            await _rankHandler.HandleAsync(Context.Guild, user, Context.DbGuild, await _userRepo.FetchUserAsync(user));
            await ReplyAsync($"Successfully set {user}'s balance to $100,000.00.");
        }

        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        public async Task Add(decimal money, [Remainder] IGuildUser user)
        {
            if (money < 0) await ErrorAsync("You may not add negative money to a user's balance.");
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.FetchUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, dbUser, money);
            await ReplyAsync($"Successfully added {money.USD()} to {user}'s balance.");
        }

        [Command("AddTo")]
        [Summary("Add cash to every users balance in a specific role.")]
        public async Task AddTo(decimal money, [Remainder] IRole role)
        {
            if (money < 0) await ErrorAsync("You may not add negative money to these users's balances.");
            await ReplyAsync("The addition of cash has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await _userRepo.EditCashAsync(user, Context.DbGuild, await _userRepo.FetchUserAsync(user), money);
            await ReplyAsync($"Successfully added {money.USD()} to the balance of every user in the {role.Mention} role.");
        }

        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        public async Task Remove(decimal money, [Remainder] IGuildUser user)
        {
            if (money < 0) await ErrorAsync("You may not remove a negative amount of money from a user's balance.");
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.FetchUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, dbUser, -money);
            await ReplyAsync($"Successfully removed {money.USD()} from {user}'s balance.");
        }

        [Command("RemoveFrom")]
        [Summary("Remove cash to every users balance in a specific role.")]
        public async Task Remove(decimal money, [Remainder] IRole role)
        {
            if (money < 0) await ErrorAsync("You may not remove negative money from these users's balances.");
            await ReplyAsync("The cash removal has commenced...");
            foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                await _userRepo.EditCashAsync(user, Context.DbGuild, await _userRepo.FetchUserAsync(user), -money);
            await ReplyAsync($"Successfully removed {money.USD()} from the balance of every user in the {role.Mention} role.");
        }

        [Command("Reset")]
        [Summary("Resets all user data for the entire server or a specific role.")]
        public async Task Remove([Remainder] IRole role = null)
        {
            if (role == null)
            {
                _users.DeleteMany(x => x.GuildId == Context.Guild.Id);
                _gangs.DeleteMany(y => y.GuildId == Context.Guild.Id);
                await ReplyAsync("Successfully reset all data in your server!");
            }
            else
            {
                foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                    _users.DeleteOne(y => y.UserId == user.Id && y.GuildId == user.Guild.Id);
                await ReplyAsync($"Successfully reset all users with the {role.Mention} role!");
            }
        }

        [Command("AddModRole")]
        [Summary("Adds a moderator role.")]
        public async Task AddModRole(IRole modRole, int permissionLevel = 1)
        {
            if (permissionLevel < 1 || permissionLevel > 3) await ErrorAsync("Permission levels:\nModeration: 1\nAdministration: 2\nServer Owner: 3");
            if (Context.DbGuild.ModRoles.ElementCount == 0)
                await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.ModRoles, new BsonDocument()
                {
                    { modRole.Id.ToString(), permissionLevel }
                });
            else
            {
                if (Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString())) await ErrorAsync("You have already set this mod role.");
                Context.DbGuild.ModRoles.Add(modRole.Id.ToString(), permissionLevel);
                await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            }
            await ReplyAsync($"You have successfully added {modRole.Mention} as a moderation role with a permission level of {permissionLevel}.");
        }

        [Command("RemoveModRole")]
        [Summary("Removes a moderator role.")]
        public async Task RemoveModRole([Remainder] IRole modRole)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0) await ErrorAsync("There are no moderator roles yet!");
            if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                await ErrorAsync("This role is not a moderator role!");
            Context.DbGuild.ModRoles.Remove(modRole.Id.ToString());
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            await ReplyAsync($"You have successfully removed the {modRole.Mention} moderator role.");
        }

        [Command("ModifyModRole")]
        [Summary("Modfies a moderator role.")]
        public async Task ModifyRank(IRole modRole, int permissionLevel)
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0) await ErrorAsync("There are no moderator roles yet!");
            if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                await ErrorAsync("This role is not a moderator role!");
            if (Context.DbGuild.ModRoles.First(x => x.Name == modRole.Id.ToString()).Value == permissionLevel)
                await ErrorAsync($"This mod role already has a permission level of {permissionLevel}");
            Context.DbGuild.ModRoles[Context.DbGuild.ModRoles.IndexOfName(modRole.Id.ToString())] = permissionLevel;
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
            await ReplyAsync($"You have successfully set the permission level of the {modRole.Mention} moderator role to {permissionLevel}.");
        }

        [Command("SetGlobalMultiplier")]
        [Summary("Sets the global chatting multiplier.")]
        public async Task SetGlobalMultiplier(decimal globalMultiplier){
            if (globalMultiplier < 0) await ErrorAsync("The global multiplier may not be negative.");
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.GlobalChattingMultiplier, globalMultiplier);
            await ReplyAsync($"You have successfully set the global chatting multiplier to {globalMultiplier.ToString("N2")}.");
        }
        
        [Command("SetRate")]
        [Summary("Sets the global temporary multiplier increase rate.")]
        public async Task SetMultiplierIncrease(decimal interestRate){
            if (interestRate < 0) await ErrorAsync("The temporary multiplier increase rate may not be negative.");
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.TempMultiplierIncreaseRate, interestRate);
            await ReplyAsync($"You have successfully set the global temporary multiplier increase rate to {interestRate.ToString("N2")}.");
        }

    }
}
