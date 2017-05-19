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
        [Command("Whore")]
        [RequireCooldown]
        [Summary("Sell your body for some quick cash.")]
        public async Task Whore()
        {
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Whore = DateTime.UtcNow);

            int roll = Config.RAND.Next(1, 101);
            if (roll > Config.WHORE_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.WHORE_FINE);

                await ReplyAsync($"What are the fucking odds that one of your main clients was a cop... " +
                                 $"You are lucky you only got a {Config.WHORE_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyWhored = (Config.RAND.Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyWhored);

                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.USD()}. Balance: {Context.Cash.USD()}.");
            }
        }
    }
}
