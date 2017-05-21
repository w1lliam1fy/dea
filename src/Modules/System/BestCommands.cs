using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using DEA.Common.Data;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("BestCommands")]
        [Alias("Best")]
        [Summary("The most popular DEA commands.")]
        public async Task BestCommands()
        {
            var best = _statistics.CommandUsage.OrderByDescending(x => x.Value);

            var message = string.Empty;
            var position = 0;

            foreach (var element in best)
            {
                message += $"{position}. **{element.Key}:** {element.Value} uses\n";

                if (position >= Config.COMMAND_LB_CAP)
                {
                    break;
                }

                position++;
            }

            await SendAsync(message, "The Best Commands");
        }
    }
}
