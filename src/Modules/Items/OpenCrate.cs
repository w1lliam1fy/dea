using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Preconditions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Open")]
        [Cooldown(1, 2, Scale.Seconds)]
        [Alias("OpenCrate")]
        [Summary("Open a crate!")]
        public async Task Crates([Remainder]string crate)
        {
            crate = crate.ToLower();
            if (!crate.EndsWith("crate"))
            {
                crate += " crate";
            }

            if (!_items.Any(x => x.Name.ToLower() == crate))
            {
                ReplyError("This item does not exist.");
            }
            else if (!Context.DbUser.Inventory.Any(x => x.Name.ToLower() == crate))
            {
                ReplyError($"You do not have any {crate}s in your inventory.");   
            }

            var element = _items.First(x => x.Name.ToLower() == crate);

            await _gameService.ModifyInventoryAsync(Context.DbUser, element.Name, -1);
            await _gameService.OpenCrateAsync(Context, element.Odds);
        }
    }
}
