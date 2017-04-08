using Discord.Commands;
using System.Threading.Tasks;
using DEA.Services;

namespace DEA.Modules
{
    public class Gambling : ModuleBase<SocketCommandContext>
    {

        [Command("21+")]
        [Summary("Roll 20.84 or higher on a 100.00 sided die, win 0.2X your bet.")]
        [Remarks("21+ <Bet>")]
        public async Task XHalf(decimal bet)
        {
            await ModuleMethods.Gamble(Context, bet, 20.84m, 0.2m);
        }

        [Command("50x2")]
        [Require(Attributes.FiftyX2)]
        [Summary("Roll 50.00 or higher on a 100.00 sided die, win your bet.")]
        [Remarks("50x2 <Bet>")]
        public async Task X2BetterOdds(decimal bet)
        {
            await ModuleMethods.Gamble(Context, bet, 50.0m, 1.0m);
        }

        [Command("53x2")]
        [Summary("Roll 52.50 or higher on a 100.00 sided die, win your bet.")]
        [Remarks("53x2 <Bet>")]
        public async Task X2(decimal bet)
        {
            await ModuleMethods.Gamble(Context, bet, 52.5m, 1.0m);
        }

        [Command("75+")]
        [Summary("Roll 75.01 or higher on a 100.00 sided die, win 2.8X your bet.")]
        [Remarks("75+ <Bet>")]
        public async Task X3dot8(decimal bet)
        {
            await ModuleMethods.Gamble(Context, bet, 75.01m, 2.8m);
        }

        [Command("100x9500")]
        [Remarks("100x9500 <Bet>")]
        [Summary("Roll 100.00 on a 100.00 sided die, win 9500X your bet.")]
        public async Task X90(decimal bet) {
            await ModuleMethods.Gamble(Context, bet, 100.0m, 9500.0m);
        }

    }
}
