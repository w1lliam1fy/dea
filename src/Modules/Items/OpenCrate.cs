using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Common.Items;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("OpenCrate")]
        [Alias("Open")]
        [Cooldown]
        [Remarks("Gold Crate")]
        [Summary("Open a crate!")]
        public async Task OpenCrate([Own] [Remainder] Crate crate)
        {
            var item = await _gameService.OpenCrateAsync(crate, Context.DbUser);
            await ReplyAsync($"Congrats! You just won: {item.Name}");
            _cooldownService.TryAdd(new CommandCooldown(Context.User.Id, Context.Guild.Id, "OpenCrate", Config.OpenCrateCooldown));
        }
    }
}
