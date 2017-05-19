using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("Add")]
        [Summary("Add cash into a user's balance.")]
        public async Task Add(decimal money, [Remainder] IGuildUser user)
        {
            if (money < 0)
            {
                ReplyError("You may not add negative money to a user's balance.");
            }

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, dbUser, money);

            await SendAsync($"Successfully added {money.USD()} to {user.Boldify()}'s balance.");
        }
    }
}
