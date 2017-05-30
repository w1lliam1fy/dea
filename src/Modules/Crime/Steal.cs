using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Services.Static;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Steal")]
        [Require(Attributes.Steal)]
        [Cooldown]
        [Summary("Snipe some goodies from your local stores.")]
        public async Task Steal()
        {
            int roll = CryptoRandom.Next(100);
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
                decimal moneyStolen = CryptoRandom.NextDecimal(Config.MIN_STEAL, Config.MAX_STEAL);
                await _userRepo.EditCashAsync(Context, moneyStolen);

                string randomStore = Config.STORES[CryptoRandom.Next(Config.STORES.Length) - 1];
                await ReplyAsync($"You walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                                 $"with {moneyStolen.USD()}. Balance: {Context.Cash.USD()}.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Steal", Config.STEAL_COOLDOWN));
        }
    }
}
