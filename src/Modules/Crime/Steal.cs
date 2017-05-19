using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Steal")]
        [RequireCooldown]
        [Require(Attributes.Steal)]
        [Summary("Snipe some goodies from your local stores.")]
        public async Task Steal()
        {
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Steal = DateTime.UtcNow);

            int roll = Config.RAND.Next(1, 101);
            if (roll > Config.STEAL_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.STEAL_FINE);
                await ReplyAsync($"You were on your way out with the cash, but then some hot chick asked you if you " +
                                 $"wanted to bust a nut. Turns out she was a cop, and raped you before turning you in. Since she passed on some " +
                                 $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                                 $"{Config.STEAL_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyStolen = (Config.RAND.Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyStolen);

                string randomStore = Config.STORES[Config.RAND.Next(1, Config.STORES.Length) - 1];
                await ReplyAsync($"You walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                                 $"with {moneyStolen.USD()}. Balance: {Context.Cash.USD()}.");
            }
        }
    }
}
