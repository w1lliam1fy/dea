using DEA.Common.Extensions;
using DEA.Database.Models;
using DEA.Database.Repository;
using Discord;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class GameService
    {
        private InteractiveService _interactiveService;
        private UserRepository _userRepo;

        public GameService(InteractiveService interactiveService, UserRepository userRepo)
        {
            _interactiveService = interactiveService;
            _userRepo = userRepo;
        }

        public async Task Trivia(IMessageChannel channel, Guild dbGuild)
        {
            int roll = new Random().Next(0, dbGuild.Trivia.ElementCount);
            var element = dbGuild.Trivia.GetElement(roll);
            await channel.SendAsync("__**TRIVIA:**__ " + element.Name);
            var answer = await _interactiveService.WaitForMessage(channel, y => y.Content.ToLower() == element.Value.AsString.ToLower());
            if (answer != null)
            {
                var user = answer.Author as IGuildUser;
                await _userRepo.EditCashAsync(user, dbGuild, await _userRepo.FetchUserAsync(user), Config.TRIVIA_PAYOUT);
                await channel.SendAsync($"{user}, Congrats! You just won {Config.TRIVIA_PAYOUT.USD()} for correctly answering \"{element.Value.AsString}\".");
            }
            else
            {
                await channel.SendAsync($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                           $"you are on your own. The right answer is: \"{element.Value.AsString}\".");
            }
        }
    }
}
