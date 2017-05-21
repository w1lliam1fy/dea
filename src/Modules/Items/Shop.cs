using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Shop")]
        [Alias("Buy")]
        [Remarks("Gold Crate")]
        [Summary("List of available shop items.")]
        public async Task Shop([Remainder] string item = null)
        {
            item = item?.ToLower();

            if (string.IsNullOrWhiteSpace(item))
            {
                string description = string.Empty;
                foreach (var _item in _items.OrderBy(x => x.Price))
                {
                    if (_item.ItemType == "Crate")
                    {
                        description += $"**Cost:** {_item.Price.USD()} | **Command:** `{Context.Prefix}shop {_item.Name}` | **Description:** {_item.Description}\n";
                    }
                }

                await SendAsync(description, "Available Shop Items");
            }
            else if (_items.Any(x => x.Name.ToLower() == item && x.ItemType == "Crate"))
            {
                var element = _items.First(x => x.Name.ToLower() == item);
                if (element.Price > Context.Cash)
                {
                    ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                }

                await _gameService.ModifyInventoryAsync(Context.DbUser, element.Name);
                await _userRepo.EditCashAsync(Context, -element.Price);

                await ReplyAsync($"You have successfully purchased: {element.Name}!");
            }
            else
            {
                ReplyError("This item either does not exist or is not a crate.");
            }
        }
    }
}
