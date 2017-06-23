using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using DEA.Common.Extensions.DiscordExtensions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("TriviaQuestions")]
        [Alias("Questions")]
        [Summary("Sends you a list of all trivia questions.")]
        public async Task TriviaQuestions()
        {
            if (Context.DbGuild.Trivia.ElementCount == 0)
            {
                ReplyError("There are no trivia questions yet!");
            }

            List<string> elements = new List<string>();
            var triviaElements = Context.DbGuild.Trivia.Elements.ToList();

            for (int i = 0; i < triviaElements.Count; i++)
            {
                elements.Add($"{i + 1}. {triviaElements[i].Name}\n");
            }

            var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendCodeAsync(elements, "Trivia Questions");

            await ReplyAsync("You have been DMed with a list of all the trivia questions!");
        }
    }
}
