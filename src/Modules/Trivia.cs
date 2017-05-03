using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;
using Discord.Commands;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DEA.Common.Data;

namespace DEA.Modules
{
    public class Trivia : DEAModule
    {
        private readonly GuildRepository _guildRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly GameService _gameService;

        public Trivia(GuildRepository guildRepo, UserRepository userRepo, InteractiveService interactiveService, GameService gameService)
        {
            _guildRepo = guildRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _gameService = gameService;
        }

        [Command("ChangeAutoTriviaSettings")]
        [Alias("EnableAutoTrivia", "DisableAutoTrivia")]
        [Require(Attributes.Admin)]
        [Summary("Enables/disables the auto trivia feature: Sends a trivia question in the default text channel every 2 minutes.")]
        public async Task ChangeAutoTriviaSettings()
        {
            if (Context.DbGuild.AutoTrivia)
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.AutoTrivia = false);
                await ReplyAsync($"You have successfully disabled auto trivia!");
            }
            else
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.AutoTrivia = true);
                await ReplyAsync($"You have successfully enabled auto trivia!");
            }
        }

        [Command("ModifyQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia question.")]
        public async Task ModifyQuestion([Summary("Do you even lift?")] string question, [Summary("Dost thou even hoist?")] [Remainder] string newQuestion)
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

        [Command("ModifyAnswer")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia answer.")]
        public async Task ModifyAnswer([Summary("Do you even lift?")] string question, [Summary("HELL YEA DUDE! FIRE IT UP!")] [Remainder] string answer)
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

        [Command("AddQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Adds a trivia question.")]
        public async Task AddTrivia([Summary("Do you even lift?")] string question, [Summary("HELL YEA DUDE! LET HER RIP!")] [Remainder] string answer)
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

            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendCodeAsync(elements, "Trivia Questions");

            await ReplyAsync("You have been DMed with a list of all the trivia questions!");
        }

        [Command("TriviaAnswers")]
        [Require(Attributes.Moderator)]
        [Alias("Answers", "Answer", "TriviaAnswer")]
        [Summary("Sends you a list of all trivia answers.")]
        public async Task TriviaAnswers([Summary("Do you even lift?")] [Remainder] string question = null)
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

        [Command("Trivia")]
        [Require(Attributes.Moderator)]
        [Summary("Randomly select a trivia question to be asked, and reward whoever answers it correctly.")]
        public Task TriviaCmd()
        {
            return _gameService.TriviaAsync(Context.Channel, Context.DbGuild);
        }
    }
}
