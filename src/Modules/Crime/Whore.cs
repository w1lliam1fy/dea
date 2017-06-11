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
        [Command("Whore")]
        [Cooldown]
        [Summary("Sell your body for some quick cash.")]
        public async Task Whore()
        {
            if (CryptoRandom.Roll() > Config.WhoreOdds)
            {
                await _userRepo.EditCashAsync(Context, -Config.WhoreFine);

                await ReplyAsync($"What are the fucking odds that one of your main clients was a cop... " +
                                 $"You are lucky you only got a {Config.WhoreFine.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyWhored = CryptoRandom.NextDecimal(Config.MinWhore, Config.MaxWhore);
                await _userRepo.EditCashAsync(Context, moneyWhored);

                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.USD()}. Balance: {Context.Cash.USD()}.");
            }
            _cooldownService.TryAdd(new CommandCooldown(Context.User.Id, Context.Guild.Id, "Whore", Config.WhoreCooldown));
        }
    }
}
