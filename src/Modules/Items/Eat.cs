using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Items;
using DEA.Common.Preconditions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Eat")]
        [Remarks("Bear Grylls Meat")]
        [Summary("Eat a chosen food in your inventory to gain health.")]
        public async Task Eat([Own] [Remainder] Food food)
        {
            await _userRepo.ModifyAsync(Context.DbUser, x =>
            {
                x.Health += food.Health;
                if (x.Health > 100)
                {
                    x.Health = 100;
                }
            });

            await _gameService.ModifyInventoryAsync(Context.DbUser, food.Name, -1);
            await ReplyAsync($"You ate: \"{food.Name}\" increasing your health by {food.Health}. Health: {Context.DbUser.Health}.");
        }
    }
}
