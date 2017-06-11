using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("ResetUser")]
        [Remarks("Sexy John#0007")]
        [Summary("Resets all data for a specific user.")]
        public async Task ResetUser([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);

            await _userRepo.DeleteAsync(dbUser);
            await _RankHandler.HandleAsync(user, Context.DbGuild, await _userRepo.GetUserAsync(user));

            await SendAsync($"You have successfully reset {user.Boldify()}'s data.");
        }
    }
}
