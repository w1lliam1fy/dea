using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;
using DEA.Common.Data;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Item")]
        [Summary("Get all the information on any item.")]
        public async Task Item([Remainder] [Summary("Bullet")] string item)
        {
            item = item?.ToLower();
            if (_items.Any(x => x.Name.ToLower() == item))
            {
                var element = _items.First(x => x.Name.ToLower() == item);
                var message = $"**Description:** {element.Description}\n";

                message += element.Price == 0 ? string.Empty : $"**Price:** {element.Price.USD()}\n";
                message += element.Damage == 0 ? string.Empty : $"**Damage:** {element.Damage}\n";
                message += element.Accuracy == 0 ? string.Empty : $"**Accuracy:** {element.Accuracy}\n";
                message += element.Health == 0 ? string.Empty : $"**Health:** {element.Health}\n";
                message += element.Odds == 0 || !Config.WEAPON_TYPES.Any(x => x == element.ItemType) ? string.Empty : $"**Crate Odds:** {(element.Odds / (decimal)_itemWeaponOdds).ToString("P")}\n";

                await SendAsync(message, element.Name);
            }
            else
            {
                ReplyError("This item does not exist.");
            }
        }
    }
}
