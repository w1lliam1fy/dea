using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using DEA.Common.Preconditions;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("TriviaAnswers")]
        [Require(Attributes.Moderator)]
        [Alias("Answers", "Answer", "TriviaAnswer")]
        [Remarks("Is DEA the best discord bot?")]
        [Summary("Sends you a list of all trivia answers.")]
        public async Task TriviaAnswers([Remainder] string question = null)
        {
            if (Context.DbGuild.Trivia.ElementCount == 0)
            {
                ReplyError("There are no trivia questions yet!");
            }

            var channel = await Context.User.CreateDMChannelAsync();

            if (question == null)
            {
                List<string> elements = new List<string>();
                var triviaElements = Context.DbGuild.Trivia.Elements.ToList();

                for (int i = 0; i < triviaElements.Count; i++)
                {
                    elements.Add($"{i + 1}. {triviaElements[i].Name} | {triviaElements[i].Value}\n");
                }

                await channel.SendCodeAsync(elements, "Trivia Answers");

                await ReplyAsync("You have been DMed with a list of all the trivia answers!");
            }
            else
            {
                if (!Context.DbGuild.Trivia.Contains(question))
                {
                    ReplyError($"That question does not exist.");
                }

                await channel.SendAsync($"The answer to that question is: {Context.DbGuild.Trivia[question]}");
                await ReplyAsync("You have been DMed with the answer to that question!");
            }
        }
    }
}
