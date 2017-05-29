using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using System.Collections.Generic;
using DEA.Common.Items;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("OpenAll")]
        [Cooldown]
        [Remarks("Gold Crate")]
        [Summary("Open a crate!")]
        public async Task OpenAll([Own] [Remainder] Crate crate)
        {
            var quantity = Context.DbUser.Inventory[crate.Name].AsInt32;

            IReadOnlyDictionary<string,int> items = await _gameService.MassOpenCratesAsync(crate, quantity, Context.DbUser);

            var reply = string.Empty;
            foreach (var item in items)
            {
                reply += $"**{item.Key}:** {item.Value}\n";
            }

            await SendAsync(reply, $"Items {Context.User} has won");

            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "OpenAll", Config.OPEN_CRATE_COOLDOWN));
        }
    }
}
