using DEA.Common.Extensions.DiscordExtensions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.BotOwners
{
    public partial class BotOwners
    {
        [Command("InformOwners")]
        [Summary("Sends a message to all server owners.")]
        public async Task InformOwners([Remainder] string message)
        {
            await ReplyAsync("The inform owners process has started...");
            foreach (var guild in Context.Client.Guilds)
            {
                try
                {
                    var channel = await guild.Owner.CreateDMChannelAsync();

                    await channel.SendAsync(message);
                }
                catch { }

                await Task.Delay(1000);
            }
            await ReplyAsync("All owners have been informed.");
        }
    }
}