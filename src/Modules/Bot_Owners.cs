using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules
{
    [Name("Bot Owners")]
    [Require(Attributes.BotOwner)]
    public class Bot_Owners : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public Bot_Owners(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        [Command("SetGame")]
        [Summary("Sets the game of DEA.")]
        public async Task SetGame([Summary("boss froth")] [Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            await ReplyAsync($"Successfully set the game to {game}.");
        }

        [Command("SendGlobalUpdate")]
        [Summary("Sends a global update message into all DEA Update channels.")]
        public async Task SendGlobalUpdate([Remainder] string updateMessage)
        {
            await ReplyAsync("The global update message process has started...");
            foreach (var guild in Context.Client.Guilds)
            {
                var dbGuild = await _guildRepo.FetchGuildAsync(guild.Id);
                if (dbGuild.UpdateChannelId > 0)
                {
                    var channel = guild.GetChannel(dbGuild.UpdateChannelId);

                    if (channel != null)
                    {
                        try
                        {
                            await (channel as ITextChannel).SendAsync(updateMessage);
                        }
                        catch { }
                    }
                }
            }
            await ReplyAsync("All global update messages have been sent.");
        }

        [Command("InformOwners")]
        [Summary("Informs the owner of every Server DEA is in with a custom message.")]
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
            }
            await ReplyAsync("All owners have been informed.");
        }
    }
}
