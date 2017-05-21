using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("AddQuestion")]
        [Require(Attributes.Moderator)]
        [Remarks("\"Is DEA the best discord bot?\" Yes")]
        [Summary("Adds a trivia question.")]
        public async Task AddTrivia(string question, [Remainder] string answer)
        {
            if (Context.DbGuild.Trivia.Contains(question))
            {
                ReplyError("That question already exists.");
            }
            else if (!Config.ALPHANUMERICAL.IsMatch(answer))
            {
                ReplyError("Trivia answers may only contain alphanumerical characters.");
            }
            else if (!Config.ANWITHQUESTIONMARK.IsMatch(question))
            {
                ReplyError("Trivia questions may only contain alphanumerical characters excluding the question mark.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Trivia.Add(question, answer));

            await ReplyAsync($"Successfully added the \"{question}\" trivia question.");
        }
    }
}
