using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.BotOwners
{
    public partial class BotOwners
    {
        [Command("SendGlobalUpdate")]
        [Remarks("SetGlobalUpdate DEA can now doxx people *instantly!*")]
        [Summary("Sends a global update message into all DEA Update channels.")]
        public async Task SendGlobalUpdate([Remainder] string updateMessage)
        {
            await ReplyAsync("The global update message process has started...");
            var dbGuilds = await _guildRepo.AllAsync();

            foreach (var guild in Context.Client.Guilds)
            {
                if (dbGuilds.Exists(x => x.GuildId == guild.Id))
                {
                    var dbGuild = dbGuilds.Find(x => x.GuildId == guild.Id);
                    try
                    {
                        var channel = guild.GetChannel(dbGuild.UpdateChannelId);
                        await (channel as ITextChannel).SendAsync(updateMessage);
                    }
                    catch
                    {
                        //Ignored.
                    }
                }
            }
            await ReplyAsync("All global update messages have been sent.");
        }
    }
}
