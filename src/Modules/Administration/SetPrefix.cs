using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetPrefix")]
        [Remarks("SetPrefix !")]
        [Summary("Sets the guild specific prefix.")]
        public async Task SetPrefix([Remainder] string prefix)
        {
            if (prefix.Length > 3)
            {
                ReplyError("The maximum character length of a prefix is 3.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Prefix = prefix);

            await ReplyAsync($"You have successfully set the prefix to {prefix}.");
        }
    }
}
