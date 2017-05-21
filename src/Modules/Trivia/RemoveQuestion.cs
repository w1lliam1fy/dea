using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("RemoveQuestion")]
        [Require(Attributes.Moderator)]
        [Remarks("RemoveQuestion What discord bot is better than DEA?")]
        [Summary("Removes a trivia question.")]
        public async Task RemoveTrivia([Remainder] string question)
        {
            if (!Context.DbGuild.Trivia.Contains(question))
            {
                ReplyError($"That question does not exist.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Trivia.Remove(question));

            await ReplyAsync($"Successfully removed the \"{question}\" trivia question.");
        }
    }
}
