using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("RemoveFrom")]
        [Summary("Remove cash to every users balance in a specific role.")]
        public async Task Remove(decimal money, [Remainder] IRole role)
        {
            if (money < 0)
            {
                ReplyError("You may not remove negative money from these users's balances.");
            }

            await ReplyAsync("The cash removal has commenced...");
            foreach (var user in (await (Context.Guild as IGuild).GetUsersAsync()).Where(x => x.RoleIds.Any(y => y == role.Id)))
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, await _userRepo.GetUserAsync(user), -money);
            }

            await SendAsync($"Successfully removed {money.USD()} from the balance of every user in the {role.Mention} role.");
        }
    }
}
