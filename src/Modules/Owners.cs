using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DEA.Database.Repository;
using System.Collections.Generic;
using DEA.Database.Models;
using MongoDB.Driver;

namespace DEA.Modules
{
    [Require(Attributes.ServerOwner)]
    public class Owners : ModuleBase<SocketCommandContext>
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
            await ReplyAsync($"Successfully reset all of {user.Mention} cooldowns.");
        }

        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        [Remarks("Add <@User> <Cash>")]
        public async Task Give(IGuildUser userMentioned, double money)
        {
            if (money < 0) throw new Exception("You may not add negative money to a user's balance.");
            await UserRepository.EditCashAsync(Context, userMentioned.Id, money);
            await ReplyAsync($"Successfully added {money.ToString("C", Config.CI)} to {userMentioned.Mention}'s balance.");
        }

        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        [Remarks("Remove <@User> <Cash>")]
        public async Task Remove(IGuildUser userMentioned, double money)
        {
            if (money < 0) throw new Exception("You may not remove a negative amount of money from a user's balance.");
            await UserRepository.EditCashAsync(Context, userMentioned.Id, -money);
            await ReplyAsync($"Successfully removed {money.ToString("C", Config.CI)} from {userMentioned.Mention}'s balance.");
        }

        [Command("Reset", RunMode = RunMode.Async)]
        [Summary("Resets all user data for the entire server or a specific role.")]
        [Remarks("Reset [@Role]")]
        public async Task Remove(IRole role = null)
        {
            if (role == null)
            {
                DEABot.Users.DeleteMany(x => x.GuildId == Context.Guild.Id);
                DEABot.Gangs.DeleteMany(y => y.GuildId == Context.Guild.Id);
                await ReplyAsync("You have successfully reset all data in your server!");
            }
            else
            {
                foreach (var user in Context.Guild.Users.Where(x => x.Roles.Any(y => y.Id == role.Id)))
                    DEABot.Users.DeleteOne(y => y.UserId == user.Id && y.GuildId == user.Guild.Id);
                await ReplyAsync($"You have successfully reset all users with the {role.Mention} role!");
            }
        }

    }
}
