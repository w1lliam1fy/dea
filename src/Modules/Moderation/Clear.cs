using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Remarks("20 Cleaning up spam")]
        [Summary("Deletes x amount of messages.")]
        public async Task CleanAsync(int quantity = 25, [Remainder] string reason = null)
        {
            if (quantity < Config.MinClear)
            {
                ReplyError($"You may not clear less than {Config.MinClear} messages.");
            }
            else if (quantity > Config.MaxClear)
            {
                ReplyError($"You may not clear more than {Config.MaxClear} messages.");
            }
            else if (Context.Channel.Id == Context.DbGuild.ModLogChannelId)
            {
                ReplyError("For security reasons, you may not use this command in the mod log channel.");
            }

            var messages = await Context.Channel.GetMessagesAsync(quantity).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);

            var msg = await ReplyAsync($"Messages deleted: **{quantity}**.");

            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Clear", Config.ClearColor, reason, Context.User, null, "Quantity", $"{quantity}");

            await Task.Delay(2500);
            await msg.DeleteAsync();
        }
    }
}
