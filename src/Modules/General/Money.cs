using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Money")]
        [Alias("Cash", "Balance", "Bal")]
        [Remarks("Sexy John#0007")]
        [Summary("View the wealth of anyone.")]
        public async Task Money([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);

            await SendAsync($"{user.Boldify()}'s balance: {dbUser.Cash.USD()}.");
        }
    }
}
