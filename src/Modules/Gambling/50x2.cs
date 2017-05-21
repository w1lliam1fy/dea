using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;
using DEA.Common;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : DEAModule
    {
        [Command("50x2")]
        [Require(Attributes.FiftyX2)]
        [Remarks("50")]
        [Summary("Roll 50.01 or higher on a 100.00 sided die, win your bet.")]
        public Task X2BetterOdds(decimal bet)
        {
            return _gameService.GambleAsync(Context, bet, 50.01m, 1.0m);
        }
    }
}
