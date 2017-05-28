using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Items;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Shop")]
        [Alias("Buy")]
        [Remarks("Gold Crate")]
        [Summary("List of available shop items.")]
        public async Task Shop([Remainder] Crate crate = null)
        {
            if (crate == null)
            {
                string description = string.Empty;
                foreach (var item in _crates)
                {
                    description += $"**Cost:** {item.Price.USD()} | **Command:** `{Context.Prefix}shop {item.Name}` | **Description:** {item.Description}\n";
                }

                await SendAsync(description, "Available Shop Items");
            }
            else
            {
                if (crate.Price > Context.Cash)
                {
                    ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                }

                await _gameService.ModifyInventoryAsync(Context.DbUser, crate.Name);
                await _userRepo.EditCashAsync(Context, -crate.Price);

                await ReplyAsync($"You have successfully purchased: {crate.Name}!");
            }
        }
    }
}
