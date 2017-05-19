using DEA.Common.Extensions;
using DEA.Database.Models;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        public async Task GangInfo([Summary("SLAM EM BOYS")] [Remainder] string gangName = null)
        {
            Gang gang;
            if (gangName == null)
            {
                gang = Context.Gang;
            }
            else
            {
                gang = await _gangRepo.GetGangAsync(gangName, Context.Guild.Id);
            }

            if (gang == null && gangName == null)
            {
                ReplyError("You are not in a gang.");
            }

            var members = string.Empty;
            var guildInterface = Context.Guild as IGuild;
            foreach (var member in gang.Members)
            {
                var user = await guildInterface.GetUserAsync(member);
                if (user != null)
                {
                    members += $"{user.Boldify()}, ";
                }
            }

            if (members.Length != 0)
            {
                members = $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n";
            }

            var leader = await guildInterface.GetUserAsync(gang.LeaderId);
            if (leader != null)
            {
                members = $"__**Leader:**__ {leader.Boldify()}\n" + members;
            }

            var description = members + $"__**Wealth:**__ {gang.Wealth.USD()}\n" +
                              $"__**Interest rate:**__ {InterestRate.Calculate(gang.Wealth).ToString("P")}";

            await SendAsync(description, gang.Name);
        }
    }
}