using DEA.Common;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class GamblingService
    {
        private UserRepository _userRepo;
        private ResponseService _responseService;

        public GamblingService(UserRepository userRepo, ResponseService responseService)
        {
            _userRepo = userRepo;
            _responseService = responseService;
        }

        public async Task GambleAsync(DEAContext context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            if (context.Guild.GetTextChannel(context.DbGuild.GambleId) != null && context.Channel.Id != context.DbGuild.GambleId)
                throw new DEAException($"You may only gamble in {context.Guild.GetTextChannel(context.DbGuild.GambleId).Mention}!");
            if (bet < Config.BET_MIN)
                throw new DEAException($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > context.DbUser.Cash)
                throw new DEAException($"You do not have enough money. Balance: {context.DbUser.Cash.ToString("C", Config.CI)}.");

            decimal roll = new Random().Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await _userRepo.EditCashAsync(context, (bet * payoutMultiplier));
                await _responseService.Reply(context, $"You rolled: {roll.ToString("N2")}. Congrats, you won " +
                                                     $"{(bet * payoutMultiplier).ToString("C", Config.CI)}! Balance: {(context.DbUser.Cash + (bet * payoutMultiplier)).ToString("C", Config.CI)}.");
            }
            else
            {
                await _userRepo.EditCashAsync(context, -bet);
                await _responseService.Reply(context, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " +
                                                     $"{bet.ToString("C", Config.CI)}. Balance: {(context.DbUser.Cash - bet).ToString("C", Config.CI)}.");
            }
        }
    }
}
