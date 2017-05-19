using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("DisableWelcomeMessage")]
        [Alias("DisableWelcome")]
        [Summary("Disables the welcome message from being sent in direct messages and in the welcome channel.")]
        public async Task DisableWelcomeMessage()
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = string.Empty);
            await ReplyAsync("You have successfully disabled the welcome message. If ever change your mind, " +
                             "simply setting the welcome message again will enable this feature.");
        }
    }
}
