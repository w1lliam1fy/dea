using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("ModifyAnswer")]
        [Require(Attributes.Moderator)]
        [Remarks("ModifyAnswer \"Is DEA the best discord bot?\" HELL YEA DUDE!")]
        [Summary("Modify a trivia answer.")]
        public async Task ModifyAnswer(string question, [Remainder] string answer)
        {
            if (!Context.DbGuild.Trivia.Contains(question))
            {
                ReplyError($"That question does not exist.");
            }
            else if (!Config.ALPHANUMERICAL.IsMatch(answer))
            {
                ReplyError("Trivia answers may only contain alphanumerical characters.");
            }

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Trivia[question] = answer);

            await ReplyAsync($"You have successfully modified the \"{question}\" trivia question.");
        }
    }
}
