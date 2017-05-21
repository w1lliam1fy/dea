using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Data;
using DEA.Common.Preconditions;
using MongoDB.Bson;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("ModifyQuestion")]
        [Require(Attributes.Moderator)]
        [Remarks("\"Is DEA the best discord bot?\" Is DEA vastly superior to all other discord bots?")]
        [Summary("Modify a trivia question.")]
        public async Task ModifyQuestion(string question, [Remainder] string newQuestion)
        {
            if (!Context.DbGuild.Trivia.Contains(question))
            {
                ReplyError($"That question does not exist.");
            }
            else if (!Config.ANWITHQUESTIONMARK.IsMatch(newQuestion))
            {
                ReplyError("Trivia questions may only contain alphanumerical characters excluding the question mark.");
            }

            var answer = Context.DbGuild.Trivia[question];

            await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Trivia.SetElement(Context.DbGuild.Trivia.IndexOfName(question), new BsonElement(newQuestion, answer)));

            await ReplyAsync($"You have successfully modified the \"{question}\" trivia question.");
        }
    }
}
