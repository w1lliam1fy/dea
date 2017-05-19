using Discord.Commands;
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
            var invData = _gameService.InventoryData(Context.DbUser).OrderByDescending(x => x.Damage);

            if (!invData.Any(x => Config.WEAPON_TYPES.Any(y => y == x.ItemType)))
            {
                ReplyError("You must have a weapon to go fishing.");
            }
            
            var strongestWeapon = invData.First();

            if (strongestWeapon.Accuracy >= Config.RAND.Next(1, 101))
            {
                await _gameService.GetFoodAsync(Context, "Fish");
            }
            else
            {
                await ReplyAsync("You had the fucking fish in your pocket on the way to the supermarket to get some spices, and the nigga flipping fish jumped into the sink and pulled some goddamn Finding Nemo shit and bounced like fish do.");
            }
        }
    }
}
