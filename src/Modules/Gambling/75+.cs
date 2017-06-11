using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : Module
    {
        [Command("75+")]
        [Remarks("50")]
        [Summary("Roll 75.01 or higher on a 100.00 sided die, win 2.8X your bet.")]
        public Task X3dot8(decimal bet)
        {
            return _gameService.GambleAsync(Context, bet, 75.01m, 2.8m);
        }
    }
}
