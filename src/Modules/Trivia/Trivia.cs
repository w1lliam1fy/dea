using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("Trivia")]
        [Require(Attributes.Moderator)]
        [Summary("Randomly select a trivia question to be asked, and reward whoever answers it correctly.")]
        public Task TriviaCmd()
        {
            return _gameService.TriviaAsync(Context.Channel, Context.DbGuild);
        }
    }
}
