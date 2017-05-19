using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : DEAModule
    {
        [Command("53x2")]
        [Summary("Roll 52.50 or higher on a 100.00 sided die, win your bet.")]
        public Task X2(decimal bet)
        {
            return _gameService.GambleAsync(Context, bet, 52.5m, 1.0m);
        }
    }
}
