using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("Reset")]
        [Remarks("@everyone")]
        [Summary("Resets all user data for the entire server or a specific role.")]
        public async Task Remove([Remainder] IRole role = null)
        {
            if (role == null)
            {
                await _userRepo.DeleteAsync(x => x.GuildId == Context.Guild.Id);

                await ReplyAsync("Successfully reset all data in your server!");
            }
            else
            {
                foreach (var user in (await (Context.Guild as IGuild).GetUsersAsync()).Where(x => x.RoleIds.Any(y => y == role.Id)))
                {
                    await _userRepo.DeleteAsync(y => y.UserId == user.Id && y.GuildId == user.Guild.Id);
                }

                await ReplyAsync($"Successfully reset all users with the {role.Mention} role!");
            }
        }
    }
}
