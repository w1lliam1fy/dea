using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("ModifyHealth")]
        [Remarks("50 Sexy John#0007")]
        [Summary("Modify a user's health.")]
        public async Task ModifyHealth(int modifyHealth, [Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            await _userRepo.ModifyAsync(dbUser, x => x.Health += modifyHealth);

            if (dbUser.Health <= 0)
            {
                await _userRepo.DeleteAsync(x => x.Id == dbUser.Id);
                await ReplyAsync("NIGGA! You just killed him!");
            } 
            else
            {
                await ReplyAsync($"You have successfully modified {user.Boldify()}'s health to: {dbUser.Health}.");
            }
        }
    }
}
