using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetWelcomeMessage")]
        [Alias("SetWelcome")]
        [Remarks("SetWelcomeMessage CleanAssChannel")]
        [Summary("Sets the welcome message that DEA will send in either the Welcome Channel or the users DM's.")]
        public async Task SetWelcomeMessage([Remainder] string message)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = message);
            await ReplyAsync($"You have successfully set the welcome message.");
        }
    }
}
