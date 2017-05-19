using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.BotOwners
{
    public partial class BotOwners
    {
        [Command("SetGame")]
        [Summary("Sets the game of DEA.")]
        public async Task SetGame([Summary("boss froth")] [Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            await ReplyAsync($"Successfully set the game to {game}.");
        }
    }
}
