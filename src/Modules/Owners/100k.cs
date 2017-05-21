using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("100k")]
        [Remarks("100k Sexy John#0007")]
        [Summary("Sets the user's balance to $100,000.00.")]
        public async Task HundredK([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            await _userRepo.ModifyAsync(dbUser, x => x.Cash = 100000);
            await _RankHandler.HandleAsync(Context.Guild, user, Context.DbGuild, await _userRepo.GetUserAsync(user));

            await SendAsync($"Successfully set {user.Boldify()}'s balance to $100,000.00.");
        }
    }
}
