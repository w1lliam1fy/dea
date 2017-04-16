using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repository;
using DEA.Services;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.Modules
{
    public class Trivia : DEAModule
    {

        [Command("ModifyQuestion")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia question.")]
        public async Task ModifyQuestion(string question, [Remainder] string newQuestion)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That question does not exist.");
            var answer = Context.DbGuild.Trivia[question];
            Context.DbGuild.Trivia.SetElement(Context.DbGuild.Trivia.IndexOfName(question), new BsonElement(newQuestion, answer));
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await Reply($"You have successfully modified the \"{question}\" trivia question.");
        }

        [Command("ModifyAnswer")]
        [Require(Attributes.Moderator)]
        [Summary("Modify a trivia answer.")]
        public async Task ModifyAnswer(string question, [Remainder] string answer)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That quesiton does not exist.");
            if (!Config.ALPHANUMERICAL.IsMatch(answer)) await ErrorAsync("Trivia answers may only contain alphanumerical characters.");
            Context.DbGuild.Trivia[question] = answer;
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await Reply($"You have successfully modified the \"{question}\" trivia question.");
        }

        [Command("AddTrivia")]
        [Require(Attributes.Moderator)]
        [Summary("Add a trivia question.")]
        public async Task AddTrivia(string question, [Remainder] string answer)
        {
            if (Context.DbGuild.Trivia.Contains(question)) await ErrorAsync("That question already exists.");
            if (!Config.ALPHANUMERICAL.IsMatch(answer)) await ErrorAsync("Trivia answers may only contain alphanumerical characters.");
            Context.DbGuild.Trivia.Add(question, answer);
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"Successfully added the \"{question}\" trivia question.");
        }

        [Command("RemoveTrivia")]
        [Require(Attributes.Moderator)]
        [Summary("Remove a trivia question.")]
        public async Task RemoveTrivia([Remainder] string question)
        {
            if (!Context.DbGuild.Trivia.Contains(question)) await ErrorAsync($"That quesiton does not exist.");
            Context.DbGuild.Trivia.Remove(question);
            await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.Trivia, Context.DbGuild.Trivia);
            await ReplyAsync($"Successfully removed the \"{question}\" trivia question.");
        }

        [Command("TriviaQuestions")]
        [Alias("Questions")]
        [Summary("Sends you a list of all trivia questions.")]
        public async Task TriviaQuestions()
        {
            if (Context.DbGuild.Trivia.ElementCount == 0) await ErrorAsync("There are no trivia questions yet!");
            List<string> messages = new List<string>() { string.Empty };
            int questionCount = 1;
            int messageCount = 0;

            foreach (var question in Context.DbGuild.Trivia.Names)
            {
                if (messages[messageCount].Length > 2000)
                {
                    messageCount++;
                    messages.Add(string.Empty);
                }
                messages[messageCount] += $"{questionCount++}. {question}\n";
            }

            var channel = await Context.User.CreateDMChannelAsync();
            foreach (var message in messages)
                await channel.SendMessageAsync(message);
            await Reply("You have been DMed with a list of all the trivia questions!");
        }

        [Command("Trivia")]
        [Require(Attributes.Moderator)]
        [Summary("Randomly select a trivia question to be asked, and reward whoever answers it correctly.")]
        public async Task TriviaCmd()
        {
            if (Context.DbGuild.Trivia.ElementCount == 0) await ErrorAsync("There are no trivia questions yet!");
            int roll = new Random().Next(0, Context.DbGuild.Trivia.ElementCount);
            var element = Context.DbGuild.Trivia.GetElement(roll);
            await ReplyAsync("__**TRIVIA:**__ " + element.Name);
            var answer = await WaitForMessage(Context.Channel, element.Value.AsString);
            if (answer != null)
            {
                var user = answer.Author as IGuildUser;
                await UserRepository.EditCashAsync(user, Config.TRIVIA_PAYOUT);
                await Send($"{ResponseMethods.Name(user, await UserRepository.FetchUserAsync(user))}, Congrats! You just " +
                           $"won {Config.TRIVIA_PAYOUT.ToString("C", Config.CI)} for correctly answering the trivia question!");
            }
            else
            {
                await Send($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                           $"you are on your own. The right answer is: \"{element.Value.AsString}\"");
            }
        }
        
    }
}
