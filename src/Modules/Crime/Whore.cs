using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Whore")]
        [Cooldown]
        [Summary("Sell your body for some quick cash.")]
        public async Task Whore()
        {
            if (Config.Random.Roll() > Config.WHORE_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.WHORE_FINE);

                await ReplyAsync($"What are the fucking odds that one of your main clients was a cop... " +
                                 $"You are lucky you only got a {Config.WHORE_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyWhored = Config.Random.NextDecimal(Config.MIN_WHORE,Config.MAX_WHORE);
                await _userRepo.EditCashAsync(Context, moneyWhored);

                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.USD()}. Balance: {Context.Cash.USD()}.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Whore", Config.WHORE_COOLDOWN));
        }
    }
}
