using DEA.Common;
using DEA.Common.Extensions;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class GamblingService
    {
        private UserRepository _userRepo;

        public GamblingService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task GambleAsync(DEAContext context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            if (context.Guild.GetTextChannel(context.DbGuild.GambleId) != null && context.Channel.Id != context.DbGuild.GambleId)
                throw new DEAException($"You may only gamble in {context.Guild.GetTextChannel(context.DbGuild.GambleId).Mention}!");
            if (bet < Config.BET_MIN)
                throw new DEAException($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > context.DbUser.Cash)
                throw new DEAException($"You do not have enough money. Balance: {context.DbUser.Cash.USD()}.");

            decimal roll = new Random().Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await _userRepo.EditCashAsync(context, (bet * payoutMultiplier));
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Congrats, you won " +
                                                 $"{(bet * payoutMultiplier).USD()}! Balance: {(context.DbUser.Cash + (bet * payoutMultiplier)).USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(context, -bet);
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " +
                                                 $"{bet.USD()}. Balance: {(context.DbUser.Cash - bet).USD()}.");
            }
        }
    }
}
