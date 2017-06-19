using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Items;
using DEA.Services.Static;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Shop")]
        [Alias("Buy")]
        [Remarks("Gold Crate")]
        [Summary("List of available shop items.")]
        public async Task Shop(Crate crate = null, int quantity = 1)
        {
            if (crate == null)
            {
                string description = string.Empty;
                foreach (var item in Data.Crates)
                {
                    description += $"**Cost:** {item.Price.USD()} | **Command:** `{Context.Prefix}shop {item.Name}` | **Description:** {item.Description}\n";
                }

                await SendAsync(description, "Available Shop Items");
            }
            else
            {
                if (quantity < 1)
                {
                    ReplyError("You may not purchase less than one item.");
                }
                else if (crate.Price * quantity > Context.Cash)
                {
                    ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                }

                await _gameService.ModifyInventoryAsync(Context.DbUser, crate.Name, quantity);
                await _userRepo.EditCashAsync(Context, -crate.Price * quantity);

                await ReplyAsync($"You have successfully purchased: {quantity} {crate.Name}{(quantity == 1 ? string.Empty : "s")}!");
            }
        }
    }
}
