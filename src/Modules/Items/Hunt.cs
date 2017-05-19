using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Data;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Hunt")]
        [Cooldown(15, TimeScale.Minutes)]
        [Summary("Go hunting for some food.")]
        public async Task Hunt()
        {
            var invData = _gameService.InventoryData(Context.DbUser).OrderByDescending(x => x.Damage);

            if (!invData.Any(x => Config.WEAPON_TYPES.Any(y => y == x.ItemType)))
            {
                ReplyError("You must have a weapon to go hunting.");
            }

            var strongestWeapon = invData.First();

            if (strongestWeapon.Accuracy >= Config.RAND.Next(1, 101))
            {
                await _gameService.GetFoodAsync(Context, "Meat");
            }
            else
            {
                await ReplyAsync("Nigga you just about had that deer but then he did that hoof kick thing and fucked up your buddy Chuck, so then you had to go bust a nut all over him and the GODDAMN deer got away.");
            }
            _commandTimeouts.Add(new CommandTimeout(Context.User.Id, Context.Guild.Id, "Hunt"));
        }
    }
}
