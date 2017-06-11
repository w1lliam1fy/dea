using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("CashInfo")]
        [Summary("Information about the DEA Cash System.")]
        public async Task Info()
        {
            string p = Context.Prefix;

            var channel = await Context.User.CreateDMChannelAsync();

            await channel.SendAsync($@"In order to gain money, you must send a message that is at least {Config.MinCharLength} characters in length. There is a 30 second cooldown between each message that will give you exactly {Config.CashPerMsg.ToString("C")}.");

            await channel.SendAsync($@"Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the `{p}help` command. You might be wondering what is the point of all these commands. This is where ranks come in. The full list of ranks may be viewed with the `{p}rank` command. Depending on how much money you have, you will get a certain rank, and mainly, gain access to more commands. As your cash stack grows, so do the quantity commands you can use:

**{Config.JumpRequirement.USD()}:** `{p}jump`
**{Config.StealRequirement.USD()}:** `{p}steal`
**{Config.RobRequirement.USD()}:** `{p}rob <Resources>`
**{Config.BullyRequirement.USD()}:** `{p}bully`
**{Config.FiftyX2Requirement.USD()}:** `{p}50x2 <Bet>`");

            await ReplyAsync($"Information about the DEA Cash System has been DMed to you!");
        }
    }
}
