using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.BotOwners
{
    public partial class BotOwners
    {
        [Command("LeaveGuild")]
        [Summary("Leaves any guild by guild ID.")]
        public async Task LeaveGuild(ulong guildId)
        {
            var guild = await (Context.Client as IDiscordClient).GetGuildAsync(guildId);
            if (guild != null)
            {
                await guild.LeaveAsync();
                await ReplyAsync("DEA has successfully left this guild.");
            }
            else
            {
                await ReplyAsync("DEA is not in this guild.");
            }
        }
    }
}