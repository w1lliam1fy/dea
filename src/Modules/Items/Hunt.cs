using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Common.Items;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Hunt")]
        [Cooldown]
        [Remarks("Intervention")]
        [Summary("Go hunting for some food.")]
        public async Task Hunt([Own] [Remainder] Weapon weapon)
        {
            var result = await _gameService.AcquireFoodAsync(typeof(Meat), weapon.Accuracy, Context.DbUser);

            if (result != null)
            {
                await ReplyAsync($"Clean kill. Boss froth. Smooth beans. You got: {result.Name}");
            }
            else
            {
                await ReplyAsync("Nigga you just about had that deer but then he did that hoof kick thing and fucked up your buddy Chuck, so then you had to go bust a nut all over him and the GODDAMN deer got away.");
            }
            _cooldownService.TryAdd(new CommandCooldown(Context.User.Id, Context.Guild.Id, "Hunt", Config.HuntCooldown));
        }
    }
}
