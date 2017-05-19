using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("Remove")]
        [Summary("Remove cash from a user's balance.")]
        public async Task Remove(decimal money, [Remainder] IGuildUser user)
        {
            if (money < 0)
            {
                ReplyError("You may not remove a negative amount of money from a user's balance.");
            }

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, dbUser, -money);

            await SendAsync($"Successfully removed {money.USD()} from {user.Boldify()}'s balance.");
        }
    }
}
