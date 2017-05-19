using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Data;
using DEA.Common.Preconditions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Fish")]
        [Cooldown(1, 15, Scale.Minutes)]
        [Summary("Go fishing for some food.")]
        public async Task Fish()
        {
            if (!Context.DbUser.Inventory.Any(x => _items.Any(y => Config.WEAPON_TYPES.Any(z => z == y.ItemType) && y.Name == x.Name)))
            {
                ReplyError("You must have a weapon to go fishing.");
            }

            var sorted = Context.DbUser.Inventory.OrderByDescending(x => _items.First(y => y.Name == x.Name).Damage);
            var strongestWeapon = _items.First(x => x.Name == sorted.First().Name);

            if (strongestWeapon.Accuracy < Config.RAND.Next(1, 101))
            {
                await ReplyAsync("You had the fucking fish in your pocket on the way to the supermarket to get some spices, and the nigga flipping fish jumped into the sink and pulled some goddamn Finding Nemo shit and bounced like fish do.");
            }
            else
            {
                await _gameService.GetFoodAsync(Context, "Fish");
            }

        }
    }
}
