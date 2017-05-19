using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Summary("Deletes x amount of messages.")]
        public async Task CleanAsync(int quantity = 25, [Remainder] string reason = null)
        {
            if (quantity < Config.MIN_CLEAR)
            {
                ReplyError($"You may not clear less than {Config.MIN_CLEAR} messages.");
            }
            else if (quantity > Config.MAX_CLEAR)
            {
                ReplyError($"You may not clear more than {Config.MAX_CLEAR} messages.");
            }
            else if (Context.Channel.Id == Context.DbGuild.ModLogChannelId)
            {
                ReplyError("For security reasons, you may not use this command in the mod log channel.");
            }

            var messages = await Context.Channel.GetMessagesAsync(quantity).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);

            var msg = await ReplyAsync($"Messages deleted: **{quantity}**.");

            await _moderationService.ModLogAsync(Context, "Clear", new Color(34, 59, 255), reason, null, $"\n**Quantity:** {quantity}");

            await Task.Delay(2500);
            await msg.DeleteAsync();
        }
    }
}
