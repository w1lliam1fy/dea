using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("SetModLog")]
        [Remarks("SetModLog mod_log")]
        [Summary("Sets the moderation log.")]
        public async Task SetModLogChannel([Remainder] ITextChannel modLogChannel)
        {
            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.ModLogChannelId = modLogChannel.Id);
            await ReplyAsync($"You have successfully set the moderator log channel to {modLogChannel.Mention}.");
        }
    }
}
