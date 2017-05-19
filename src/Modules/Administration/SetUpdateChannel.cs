using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetUpdateChannel")]
        [Summary("Sets the channel where DEA will send messages informing you of its most recent updates and new features.")]
        public async Task SetUpdateChannel([Remainder] ITextChannel channel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.UpdateChannelId = channel.Id);
            await ReplyAsync($"You have successfully set the DEA update channel to {channel.Mention}.");
        }
    }
}
