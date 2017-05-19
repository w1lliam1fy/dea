using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("RemoveQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Removes a trivia question.")]
        public async Task RemoveTrivia([Summary("Do you even lift?")] [Remainder] string question)
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
