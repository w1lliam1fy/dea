using Discord.Commands;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using DEA.Common.Extensions;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("GangLb")]
        [Alias("gangs")]
        [Summary("Shows the wealthiest gangs.")]
        public async Task Ganglb()
        {
            var gangs = await _gangRepo.AllAsync();

            if (gangs.Count == 0)
            {
                ReplyError("There aren't any gangs yet.");
            }

            var sortedGangs = gangs.OrderByDescending(x => x.Wealth).ToList();
            string description = string.Empty;

            for (int i = 0; i < sortedGangs.Count(); i++)
            {
                if (i + 1 > Config.GANGSLB_CAP)
                {
                    break;
                }

                description += $"{i + 1}. {sortedGangs[i].Name.Boldify()}: {sortedGangs[i].Wealth.USD()}\n";
            }

            await SendAsync(description, "The Wealthiest Gangs");
        }
    }
}