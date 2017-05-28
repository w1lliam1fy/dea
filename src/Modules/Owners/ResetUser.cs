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

            await _userRepo.DeleteAsync(Context.DbUser);
            await _RankHandler.HandleAsync(user, Context.DbGuild, await _userRepo.GetUserAsync(user));

            await SendAsync($"Successfully reset {user.Boldify()}'s data.");
        }
    }
}
