using DEA.Common;
using DEA.Common.Data;
using DEA.Common.Utilities;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Models;
using DEA.Database.Repositories;
using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DEA.Services
{
    public class GameService
    {
        private readonly InteractiveService _interactiveService;
        private readonly UserRepository _userRepo;

        private readonly Item[] _items;
        private readonly IEnumerable<Item> _sortedWeapons;
        private readonly IEnumerable<Item> _sortedFish;
        private readonly IEnumerable<Item> _sortedMeat;

        public GameService(InteractiveService interactiveService, UserRepository userRepo, Item[] items)
        {
            _interactiveService = interactiveService;
            _userRepo = userRepo;
            _items = items;
            _sortedWeapons = _items.Where(x => Config.WEAPON_TYPES.Any(y => y == x.ItemType)).OrderByDescending(x => x.Odds);
            _sortedFish = _items.Where(x => x.ItemType == "Fish").OrderByDescending(x => x.Odds);
            _sortedMeat = _items.Where(x => x.ItemType == "Meat").OrderByDescending(x => x.Odds);
        }

        public async Task TriviaAsync(IMessageChannel channel, Guild dbGuild)
        {
            if (dbGuild.Trivia.ElementCount == 0)
            {
                throw new DEAException("There are no trivia questions yet!");
            }

            var random = Config.RAND;
            int roll = random.Next(0, dbGuild.Trivia.ElementCount);

            var element = dbGuild.Trivia.GetElement(roll);
            var answer = element.Value.AsString.ToLower();

            Predicate<IUserMessage> correctResponse = y => y.Content.ToLower() == answer;
            if (!answer.Any(char.IsDigit))
            {
                correctResponse = y => y.Content.SimilarTo(element.Value.AsString, 5, 10, 20);
            }

            await channel.SendAsync("__**TRIVIA:**__ " + element.Name);

            var response = await _interactiveService.WaitForMessage(channel, correctResponse);
            if (response != null)
            {
                var user = response.Author as IGuildUser;
                var winnings = random.Next(Config.TRIVIA_PAYOUT_MIN * 100, Config.TRIVIA_PAYOUT_MAX * 100) / 100m;
                await _userRepo.EditCashAsync(user, dbGuild, await _userRepo.GetUserAsync(user), winnings);
                await channel.SendAsync($"{user.Boldify()}, Congrats! You just won {winnings.USD()} for correctly answering \"{element.Value.AsString}\".");
            }
            else
            {
                await channel.SendAsync($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                                        $"you are on your own. The right answer is: \"{element.Value.AsString}\".");
            }
        }

        public async Task GambleAsync(DEAContext context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            var gambleChannel = context.Guild.GetTextChannel(context.DbGuild.GambleChannelId);

            if (gambleChannel != null && context.Channel.Id != context.DbGuild.GambleChannelId)
            {
                throw new DEAException($"You may only gamble in {gambleChannel.Mention}.");
            }
            else if (bet < Config.BET_MIN)
            {
                throw new DEAException($"Lowest bet is {Config.BET_MIN}$.");
            }
            else if (bet > context.DbUser.Cash)
            {
                throw new DEAException($"You do not have enough money. Balance: {context.DbUser.Cash.USD()}.");
            }

            decimal roll = Config.RAND.Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await _userRepo.EditCashAsync(context, bet * payoutMultiplier);
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Congrats, you won " +
                                                               $"{(bet * payoutMultiplier).USD()}! Balance: {context.DbUser.Cash.USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(context, -bet);
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " +
                                                               $"{bet.USD()}. Balance: {context.DbUser.Cash.USD()}.");
            }
        }

        public async Task OpenCrateAsync(DEAContext context, int odds)
        {
            int cumulative = 0;
            int sum = _sortedWeapons.Sum(x => x.Odds);
            int roll = Config.RAND.Next(1, sum);
            if (odds >= Config.RAND.Next(1, 101))
            {
                foreach (var item in _sortedWeapons)
                {
                    cumulative += item.Odds;
                    if (roll < cumulative)
                    {
                        await ModifyInventoryAsync(context.DbUser, item.Name);
                        await context.Channel.ReplyAsync(context.User, $"Congrats! You won: {item.Name}.");
                        break;
                    }
                }
            }
            else
            {
                await ModifyInventoryAsync(context.DbUser, "Bullet");
                await context.Channel.ReplyAsync(context.User, $"Congrats! You won: Bullet.");
            }
        }

        public async Task GetFoodAsync(DEAContext context, string foodType)
        {
            int cumulative = 0;
            int sum = foodType == "Meat" ? _sortedMeat.Sum(x => x.Odds) : _sortedFish.Sum(x => x.Odds);
            int roll = Config.RAND.Next(1, sum);

            foreach (var item in foodType == "Meat" ? _sortedMeat : _sortedFish)
            {
                cumulative += item.Odds;
                if (roll < cumulative)
                {
                    await ModifyInventoryAsync(context.DbUser, item.Name);
                    await context.Channel.ReplyAsync(context.User, $"Hot fucking pockets you just killed a nigga. You also managed to get: {item.Name}.");
                    break;
                }
            }
        }

        public async Task ModifyInventoryAsync(User DbUser, string item, int amountToAdd = 1)
        {
            if (DbUser.Inventory.Contains(item))
            {
                await _userRepo.ModifyAsync(DbUser, x => x.Inventory[item] = amountToAdd + x.Inventory[item].AsInt32);
            }
            else
            {
                await _userRepo.ModifyAsync(DbUser, x => x.Inventory.Add(item, amountToAdd));
            }

            if (DbUser.Inventory[item] <= 0)
            {
                await _userRepo.ModifyAsync(DbUser, x => x.Inventory.Remove(item));
            }
        }

        public IEnumerable<Item> InventoryData(User dbUser)
        {
            return _items.Where(x => dbUser.Inventory.Names.Any(y => y == x.Name));
        }
    }
}

