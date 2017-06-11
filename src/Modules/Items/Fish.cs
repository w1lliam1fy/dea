using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Common.Items;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Fish")]
        [Cooldown]
        [Remarks("Butterfly Knife")]
        [Summary("Go fishing for some food.")]
        public async Task Fish([Own] [Remainder] Weapon weapon)
        {
            var result = await _gameService.AcquireFoodAsync(typeof(Fish), weapon.Accuracy, Context.DbUser);

            if (result != null)
            {
                await ReplyAsync($"RIP NEMO LMFAO. Finding nemo, more like EATING NEMO ROFL! Good buddy, you got: {result.Name}");
            }
            else
            {
                await ReplyAsync("You had the fucking fish in your pocket on the way to the supermarket to get some spices, and the nigga flipping fish jumped into the sink and pulled some goddamn Finding Nemo shit and bounced.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Fish", Config.FishCooldown));
        }
    }
}
