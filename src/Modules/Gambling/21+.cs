using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : Module
    {
        [Command("21+")]
        [Remarks("50")]
        [Summary("Roll 20.84 or higher on a 100.00 sided die, win 0.2X your bet.")]
        public Task XHalf(decimal bet)
        {
            return _gameService.GambleAsync(Context, bet, 20.84m, 0.2m);
        }
    }
}
