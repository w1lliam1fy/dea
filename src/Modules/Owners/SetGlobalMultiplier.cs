using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("SetGlobalMultiplier")]
        [Remarks("SetGlobalMultiplier 1.5")]
        [Summary("Sets the global chatting multiplier.")]
        public async Task SetGlobalMultiplier(decimal globalMultiplier)
        {
            if (globalMultiplier < 0)
            {
                ReplyError("The global multiplier may not be negative.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.GlobalChattingMultiplier = globalMultiplier);

            await ReplyAsync($"You have successfully set the global chatting multiplier to {globalMultiplier.ToString("N2")}.");
        }
    }
}
