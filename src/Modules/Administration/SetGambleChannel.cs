using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Remarks("SetGambleChannel CleanAssChannel")]
        [Summary("Sets the gambling channel.")]
        public async Task SetGambleChannel([Remainder] ITextChannel gambleChannel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.GambleChannelId = gambleChannel.Id);
            await ReplyAsync($"You have successfully set the gamble channel to {gambleChannel.Mention}.");
        }
    }
}
