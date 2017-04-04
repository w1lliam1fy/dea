using Discord.Commands;
using System.Threading.Tasks;
using DEA.Services;

namespace DEA.Modules
{
    public class Gambling : ModuleBase<SocketCommandContext>
    {

        [Command("21+")]
        [Summary("Roll 20.83 or higher on a 100.00 sided die, win 0.2X your bet.")]
        [Remarks("21+ <Bet>")]
        public async Task XHalf(double bet)
        {
            await ModuleMethods.Gamble(Context, bet, 20.83, 0.2);
        }

        [Command("50x2")]
        [Require(Attributes.FiftyX2)]
        [Summary("Roll 50.00 or higher on a 100.00 sided die, win your bet.")]
        [Remarks("50x2 <Bet>")]
        public async Task X2BetterOdds(double bet)
        {
            await ModuleMethods.Gamble(Context, bet, 50.0, 1.0);
        }

        [Command("53x2")]
        [Summary("Roll 52.50 or higher on a 100.00 sided die, win your bet.")]
        [Remarks("53x2 <Bet>")]
        public async Task X2(double bet)
        {
            await ModuleMethods.Gamble(Context, bet, 52.5, 1.0);
        }

        [Command("75+")]
        [Summary("Roll 75.00 or higher on a 100.00 sided die, win 2.8X your bet.")]
        [Remarks("75+ <Bet>")]
        public async Task X3dot8(double bet)
        {
            await ModuleMethods.Gamble(Context, bet, 75.0, 2.8);
        }

        [Command("100x9499")]
        [Remarks("100x9499 <Bet>")]
        [Summary("Roll 100.00 on a 100.00 sided die, win 9499X your bet.")]
        public async Task X90(double bet) {
            await ModuleMethods.Gamble(Context, bet, 100.0, 9499.0);
        }

    }
}
