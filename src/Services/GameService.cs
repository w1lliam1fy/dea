using DEA.Common;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Models;
using DEA.Database.Repository;
using Discord;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class GameService
    {
        private readonly InteractiveService _interactiveService;
        private readonly UserRepository _userRepo;

        public GameService(InteractiveService interactiveService, UserRepository userRepo)
        {
            _interactiveService = interactiveService;
            _userRepo = userRepo;
        }

        public async Task Trivia(IMessageChannel channel, Guild dbGuild)
        {
            if (dbGuild.Trivia.ElementCount == 0)
                throw new DEAException("There are no trivia questions yet!");

            var random = new Random();
            int roll = random.Next(0, dbGuild.Trivia.ElementCount);

            var element = dbGuild.Trivia.GetElement(roll);
            var answer = element.Value.AsString.ToLower();

            Expression<Func<IUserMessage, bool>> correctResponse = y => y.Content.ToLower() == answer;
            if (!answer.Any(char.IsDigit))
            {
                if (answer.Length >= 5 && answer.Length < 10)
                    correctResponse = y => LevenshteinDistance.Compute(y.Content, element.Value.AsString) <= 1;
                else if (answer.Length < 20)
                    correctResponse = y => LevenshteinDistance.Compute(y.Content, element.Value.AsString) <= 2;
                else
                    correctResponse = y => LevenshteinDistance.Compute(y.Content, element.Value.AsString) <= 3;
            }

            await channel.SendAsync("__**TRIVIA:**__ " + element.Name);

            var response = await _interactiveService.WaitForMessage(channel, correctResponse);
            if (response != null)
            {
                var user = response.Author as IGuildUser;
                var winnings = random.Next(Config.TRIVIA_PAYOUT_MIN * 100, Config.TRIVIA_PAYOUT_MAX * 100) / 100m;
                await _userRepo.EditCashAsync(user, dbGuild, await _userRepo.FetchUserAsync(user), winnings);
                await channel.SendAsync($"{user}, Congrats! You just won {winnings.USD()} for correctly answering \"{element.Value.AsString}\".");
            }
            else
            {
                await channel.SendAsync($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                           $"you are on your own. The right answer is: \"{element.Value.AsString}\".");
            }
        }
    }
}
