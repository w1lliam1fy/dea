using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules
{
    [Name("Bot Owners")]
    [Require(Attributes.BotOwner)]
    public class Bot_Owners : DEAModule
    {
        private ResponseService _responseService;

        public Bot_Owners(ResponseService responseService)
        {
            _responseService = responseService;
        }

        [Command("SetGame")]
        [Summary("Sets the game of DEA.")]
        public async Task SetGame([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            await Reply($"Successfully set the game to {game}.");
        }

        [Command("Global")]
        [Summary("Send a global announcement in the default channel of all servers.")]
        public async Task GlobalAnnouncement([Remainder] string announcement)
        {
            await Reply("Global announcement process has commenced...");
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.Id == Context.Guild.Id) continue;
                var perms = (guild.CurrentUser as IGuildUser).GetPermissions(guild.DefaultChannel);
                if (perms.SendMessages && perms.EmbedLinks) await _responseService.Send(guild.DefaultChannel,"__**Announcement:**__ " + announcement);
            }
            await Reply("All announcements have been delivered!");
        }
    }
}
