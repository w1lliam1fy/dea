using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Cleanup")]
        [Summary("Deletes DEA's most recent messages to prevent chat flood.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Cleanup()
        {
            if (Context.Channel.Id == Context.DbGuild.ModLogChannelId)
            {
                ReplyError("For security reasons, you may not use this command in the mod log channel.");
            }

            var messages = (await Context.Channel.GetMessagesAsync(10).Flatten()).Where(x => x.Author.Id == Context.Guild.CurrentUser.Id);
            await Context.Channel.DeleteMessagesAsync(messages);
        }
    }
}
