using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Data;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Eat")]
        [Summary("Eat a chosen food in your inventory to gain health.")]
        public async Task Eat([Remainder]string item)
        {
            item = item.ToLower();

            if (!_items.Any(x => x.Name.ToLower() == item && Config.FOOD_TYPES.Any(y => y == x.Name)))
            {
                ReplyError("That is not an item that is edible.");
            }
            else if (!Context.DbUser.Inventory.Any(x => _items.Any(y => y.Name == x.Name)))
            {
                ReplyError("You do not have that item in your inventory.");
            }
            else
            {
                var element = _items.First(x => x.Name.ToLower() == item);
                await _userRepo.ModifyAsync(Context.DbUser, x =>
                {
                    x.Health += element.Health;
                    if (x.Health > 100)
                    {
                        x.Health = 100;
                    }
                });
                await _gameService.ModifyInventoryAsync(Context.DbUser, element.Name, -1);
                await ReplyAsync($"Successfully ate {element.Name} gaining you {element.Health} health. Health: {Context.DbUser.Health}.");
            }
        }
    }
}
