using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("WorstCommands")]
        [Alias("Worst")]
        [Summary("The least popular DEA commands.")]
        public async Task WorstCommands()
        {
            var worst = _statistics.CommandUsage.OrderBy(x => x.Value);

            var message = string.Empty;
            var position = 1;

            foreach (var element in worst)
            {
                message += $"{position}. **{element.Key}:** {element.Value} uses\n";

                if (position >= Config.COMMAND_LB_CAP)
                {
                    break;
                }

                position++;
            }

            await SendAsync(message, "The Worst Commands");
        }
    }
}