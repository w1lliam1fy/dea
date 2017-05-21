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
        [Command("AddTo")]
        [Remarks("AddTo \"Poor People\" 100.50")]
        [Summary("Add cash to every users balance in a specific role.")]
        public async Task AddTo(IRole role, decimal money)
        {
            if (money < 0)
            {
                ReplyError("You may not add negative money to these users's balances.");
            }

            await ReplyAsync("The addition of cash has commenced...");
            foreach (var user in (await (Context.Guild as IGuild).GetUsersAsync()).Where(x => x.RoleIds.Any(y => y == role.Id)))
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, await _userRepo.GetUserAsync(user), money);
            }

            await SendAsync($"Successfully added {money.USD()} to the balance of every user in the {role.Mention} role.");
        }
    }
}
