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
        [Command("Hunt")]
        [RequireCooldown]
        [Summary("Go hunting for some food.")]
        public async Task Hunt()
        {
            if (!Context.DbUser.Inventory.Any(x => _items.Any(y => Config.WEAPON_TYPES.Any(z => z == y.ItemType) && y.Name == x.Name)))
            {
                ReplyError("You must have a weapon to go hunting.");
            }

            await _userRepo.ModifyAsync(Context.DbUser, x => x.Hunt = DateTime.UtcNow);

            var sorted = Context.DbUser.Inventory.OrderByDescending(x => _items.First(y => y.Name == x.Name).Damage);
            var strongestWeapon = _items.First(x => x.Name == sorted.First().Name);

            if (strongestWeapon.Accuracy < Config.RAND.Next(1, 101))
            {
                await ReplyAsync("Nigga you just about had that deer but then he did that hoof kick thing and fucked up your buddy Chuck, so then you had to go bust a nut all over him and the GODDAMN deer got away.");
            }
            else
            {
                await _gameService.GetFoodAsync(Context, "Meat");
            }
        }
    }
}
