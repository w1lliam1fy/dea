using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : Module
    {
        [Command("100x9500")]
        [Remarks("50")]
        [Summary("Roll 100.00 on a 100.00 sided die, win 9500X your bet.")]
        public Task X90(decimal bet)
        {
            return _gameService.GambleAsync(Context, bet, 100.0m, 9500.0m);
        }
    }
}
