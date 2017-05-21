using DEA.Common.Extensions;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("ModifyInventory")]
        [Alias("ModifyInv")]
        [Remarks("1 \"Kitchen Knife\" Sexy John#0007")]
        [Summary("Modify a user's inventory.")]
        public async Task ModifyInventory(int quantity, string item, IGuildUser user = null)
        {
            var element = _items.FirstOrDefault(x => x.Name.ToLower() == item.ToLower());

            if (element == null)
            {
                ReplyError("This item does not exist.");
            }

            user = user ?? Context.GUser;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);

            await _gameService.ModifyInventoryAsync(dbUser, element.Name, quantity);
            await ReplyAsync($"You have successfully modified {user.Boldify()}'s inventory.");
        }
    }
}

