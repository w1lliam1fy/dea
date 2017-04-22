using DEA.Common;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Preconditions;
using DEA.Database.Repository;
using DEA.Services;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.Modules
{
    public class Trivia : DEAModule
    {
        private GuildRepository _guildRepo;
        private UserRepository _userRepo;
        private InteractiveService _interactiveService;

        public Trivia(GuildRepository guildRepo, UserRepository userRepo, InteractiveService interactiveService)
        {
            _guildRepo = guildRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
        }

        [Command("ChangeAutoTriviaSettings")]
        [Alias("EnableAutoTrivia", "DisableAutoTrivia")]
        [Require(Attributes.Admin)]
        [Summary("Enables/disables the auto trivia feature: Sends a trivia question in the default text channel every 2 minites.")]
        public async Task ChangeAutoTriviaSettings()
        {
            switch (Context.DbGuild.AutoTrivia)
            {
                case true:
                    await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.AutoTrivia, false);
                    await ReplyAsync($"You have successfully disabled auto trivia!");
                    break;
                case false:
                    await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.AutoTrivia, true);
                    await ReplyAsync($"You have successfully enabled auto trivia!");
                    break;
            }
        }

        [Command("ModifyQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia question.")]
        public async Task ModifyQuestion(string question, [Remainder] string newQuestion)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That question does not exist.");
            if (!Config.ANWITHQUESTIONMARK.IsMatch(newQuestion)) await ErrorAsync("Trivia questions may only contain alphanumerical characters excluding the question mark.");
            var answer = Context.DbGuild.Trivia[question];
            Context.DbGuild.Trivia.SetElement(Context.DbGuild.Trivia.IndexOfName(question), new BsonElement(newQuestion, answer));
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"You have successfully modified the \"{question}\" trivia question.");
        }

        [Command("ModifyAnswer")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia answer.")]
        public async Task ModifyAnswer(string question, [Remainder] string answer)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That quesiton does not exist.");
            if (!Config.ALPHANUMERICAL.IsMatch(answer)) await ErrorAsync("Trivia answers may only contain alphanumerical characters.");
            Context.DbGuild.Trivia[question] = answer;
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"You have successfully modified the \"{question}\" trivia question.");
        }

        [Command("AddQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Adds a trivia question.")]
        public async Task AddTrivia(string question, [Remainder] string answer)
        {
            if (Context.DbGuild.Trivia.Contains(question)) await ErrorAsync("That question already exists.");
            if (question.Contains(".")) await ErrorAsync("Trivia questions may not contain periods.");
            if (!Config.ALPHANUMERICAL.IsMatch(answer)) await ErrorAsync("Trivia answers may only contain alphanumerical characters.");
            if (!Config.ANWITHQUESTIONMARK.IsMatch(question)) await ErrorAsync("Trivia questions may only contain alphanumerical characters excluding the question mark.");
            Context.DbGuild.Trivia.Add(question, answer);
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"Successfully added the \"{question}\" trivia question.");
        }

        [Command("RemoveQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Removes a trivia question.")]
        public async Task RemoveTrivia([Remainder] string question)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That quesiton does not exist.");
            Context.DbGuild.Trivia.Remove(question);
            await _guildRepo.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"Successfully removed the \"{question}\" trivia question.");
        }

        [Command("TriviaQuestions")]
        [Alias("Questions")]
        [Summary("Sends you a list of all trivia questions.")]
        public async Task TriviaQuestions()
        {
            if (Context.DbGuild.Trivia.ElementCount == 0)
                await ErrorAsync("There are no trivia questions yet!");

            List<string> elements = new List<string>();
            var triviaElements = Context.DbGuild.Trivia.Elements.ToList();

            for (int i = 1; i < triviaElements.Count -1; i++)
                elements.Add($"{i}. {triviaElements[i].Name}\n");

            var channel = await Context.User.CreateDMChannelAsync();

            await channel.SendCode(elements);

            await ReplyAsync("You have been DMed with a list of all the trivia questions!");
        }

        [Command("TriviaAnswers")]
        [Require(Attributes.Moderator)]
        [Alias("Answers", "Answer", "TriviaAnswer")]
        [Summary("Sends you a list of all trivia answers.")]
        public async Task TriviaAnswers([Remainder] string question = null)
        {
            if (Context.DbGuild.Trivia.ElementCount == 0)
                await ErrorAsync("There are no trivia questions yet!");

            var channel = await Context.User.CreateDMChannelAsync();

            if (question == null)
            {
                List<string> elements = new List<string>();
                var triviaElements = Context.DbGuild.Trivia.Elements.ToList();

                for (int i = 1; i < triviaElements.Count -1; i++)
                    elements.Add($"{i}. {triviaElements[i].Name} | {triviaElements[i].Value}\n");

                var dmChannel = await Context.User.CreateDMChannelAsync();

                await dmChannel.SendCode(elements);

                await ReplyAsync("You have been DMed with a list of all the trivia answers!");
            }
            else
            {
                if (!Context.DbGuild.Trivia.Contains(question))
                    await ErrorAsync($"That quesiton does not exist.");

                await channel.SendAsync($"The answer to that question is: {Context.DbGuild.Trivia[question]}");
                await ReplyAsync("You have been DMed with the answer to that question!");
            }
        }

        [Command("Trivia")]
        [Require(Attributes.Moderator)]
        [Summary("Randomly select a trivia question to be asked, and reward whoever answers it correctly.")]
        public async Task TriviaCmd()
        {
            if (Context.DbGuild.Trivia.ElementCount == 0) await ErrorAsync("There are no trivia questions yet!");
            int roll = new Random().Next(0, Context.DbGuild.Trivia.ElementCount);
            var element = Context.DbGuild.Trivia.GetElement(roll);
            await SendAsync("__**TRIVIA:**__ " + element.Name);
            var answer = await _interactiveService.WaitForMessage(Context.Channel, y => y.Content.ToLower() == element.Value.AsString.ToLower() && y.Author.Id != Context.User.Id);
            if (answer != null)
            {
                var user = answer.Author as IGuildUser;
                await _userRepo.EditCashAsync(user, Context.DbGuild, await _userRepo.FetchUserAsync(user), Config.TRIVIA_PAYOUT);
                await SendAsync($"{user}, Congrats! You just " +
                           $"won {Config.TRIVIA_PAYOUT.USD()} for correctly answering \"{element.Value.AsString}\"");
            }
            else
            {
                await SendAsync($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                           $"you are on your own. The right answer is: \"{element.Value.AsString}\"");
            }
        }
        
    }
}
