using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
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

            await channel.SendAsync($@"In order to gain money, you must send a message that is at least {Config.MIN_CHAR_LENGTH} characters in length. There is a 30 second cooldown between each message that will give you exactly {Config.CASH_PER_MSG.ToString("C")}.");

            await channel.SendAsync($@"Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the `{p}help` command. You might be wondering what is the point of all these commands. This is where ranks come in. The full list of ranks may be viewed with the `{p}rank` command. Depending on how much money you have, you will get a certain rank, and mainly, gain access to more commands. As your cash stack grows, so do the quantity commands you can use:

**{Config.JUMP_REQUIREMENT.USD()}:** `{p}jump`
**{Config.STEAL_REQUIREMENT.USD()}:** `{p}steal`
**{Config.ROB_REQUIREMENT.USD()}:** `{p}rob <Resources>`
**{Config.BULLY_REQUIREMENT.USD()}:** `{p}bully`
**{Config.FIFTYX2_REQUIREMENT.USD()}:** `{p}50x2 <Bet>`");

            await ReplyAsync($"Information about the DEA Cash System has been DMed to you!");
        }
    }
}
