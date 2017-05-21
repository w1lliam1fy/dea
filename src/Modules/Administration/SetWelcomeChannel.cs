using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetWelcomeChannel")]
        [Remarks("SetWelcomeChannel CleanAssChannel")]
        [Summary("Set the channel where DEA will send a welcome message to all new users that join.")]
        public async Task SetWelcomeChannel([Remainder] ITextChannel channel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.WelcomeChannelId = channel.Id);
            await ReplyAsync($"You have successfully set the welcome channel to {channel.Mention}.");
        }
    }
}
