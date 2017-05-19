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
        [Summary("Resets all user data for the entire server or a specific role.")]
        public async Task Remove([Remainder] IRole role = null)
        {
            if (role == null)
            {
                await _userRepo.Collection.DeleteManyAsync(x => x.GuildId == Context.Guild.Id);
                await _gangRepo.Collection.DeleteManyAsync(y => y.GuildId == Context.Guild.Id);

                await ReplyAsync("Successfully reset all data in your server!");
            }
            else
            {
                foreach (var user in (await (Context.Guild as IGuild).GetUsersAsync()).Where(x => x.RoleIds.Any(y => y == role.Id)))
                {
                    _userRepo.Collection.DeleteOne(y => y.UserId == user.Id && y.GuildId == user.Guild.Id);
                }

                await ReplyAsync($"Successfully reset all users with the {role.Mention} role!");
            }
        }
    }
}
