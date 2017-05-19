using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Inventory")]
        [Alias("Inv")]
        [Summary("View the inventory of any user.")]
        public async Task Inventory([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);

            if (dbUser.Inventory.ElementCount == 0)
            {
                if (dbUser.UserId == Context.User.Id)
                {
                    ReplyError("You have nothing in your inventory.");
                }
                else
                {
                    ReplyError("This user has nothing in their inventory.");
                }

            }
            var description = string.Empty;

            foreach (var item in dbUser.Inventory.Elements)
            {
                var s = item.Value.AsInt32 == 1 ? string.Empty : "s";

                description += $"{item.Value} {item.Name}{s}\n";
            }
            await SendAsync(description, $"Inventory of {user.Boldify()}");
        }
    }
}
