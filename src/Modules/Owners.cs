using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Repository;

namespace DEA.Modules
{
    public class Owners : ModuleBase<SocketCommandContext>
    {
        [Command("Reset")]
        [RequireBotOwner]
        [Alias("Reset")]
        [Summary("Resets all cooldowns for a specific user.")]
        [Remarks("Reset [@User]")]
        public async Task ResetCooldowns(IGuildUser user = null)
        {
            var time = DateTimeOffset.Now.AddYears(-1);
            UserRepository.Modify(x => {
                x.Cooldowns.Whore = time;
                x.Cooldowns.Jump = time;
                x.Cooldowns.Steal = time;
                x.Cooldowns.Rob = time;
                x.Cooldowns.Message = time;
                x.Cooldowns.Withdraw = time;
            }, Context);
            await ReplyAsync($"Successfully reset all of {user.Mention} cooldowns.");
        }

        [Command("Give")]
        [RequireBotOwner]
        [Summary("Inject cash into a user's balance.")]
        [Remarks("Give <@User> <Amount of cash>")]
        public async Task Give(IGuildUser userMentioned, double money) {
            await UserRepository.EditCashAsync(Context, userMentioned.Id, +money);
            await ReplyAsync($"Successfully given {money.ToString("C2")} to <@{userMentioned.Id}>.");
        }
    }
}
