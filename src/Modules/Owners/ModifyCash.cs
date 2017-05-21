using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("ModifyCash")]
        [Remarks("ModifyCash 500 Sexy John#0007")]
        [Summary("Modify a user's balance.")]
        public async Task ModifyCash(decimal money, [Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, dbUser, money);

            await ReplyAsync($"You have successfully modified {user.Boldify()}'s balance to: {dbUser.Cash.USD()}.");
        }
    }
}
