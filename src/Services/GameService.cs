using DEA.Common;
using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Models;
using DEA.Database.Repositories;
using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DEA.Common.Items;

namespace DEA.Services
{
    public sealed class GameService
    {
        private readonly InteractiveService _interactiveService;
        private readonly UserRepository _userRepo;
        private readonly Item[] _items;
        private readonly CrateItem[] _crateItems;
        private readonly Fish[] _fish;
        private readonly Meat[] _meat;
        private readonly int _crateOdds;
        private readonly int _fishOdds;
        private readonly int _meatOdds;

        public GameService(InteractiveService interactiveService, UserRepository userRepo, Item[] items, CrateItem[] crateItems, Fish[] fish, Meat[] meat)
        {
            _interactiveService = interactiveService;
            _userRepo = userRepo;
            _items = items;
            _crateItems = crateItems;
            _fish = fish;
            _meat = meat;

            _crateOdds = _crateItems.Sum(x => x.CrateOdds);
            _fishOdds = _fish.Sum(x => x.AcquireOdds);
            _meatOdds = _meat.Sum(x => x.AcquireOdds);
        }

        public async Task TriviaAsync(IMessageChannel channel, Guild dbGuild)
        {
            if (dbGuild.Trivia.ElementCount == 0)
            {
                throw new DEAException("There are no trivia questions yet!");
            }

            int roll = Config.Random.Next(dbGuild.Trivia.ElementCount);

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
                var winnings = (decimal)Config.Random.NextDouble((double)Config.TRIVIA_PAYOUT_MIN, (double)Config.TRIVIA_PAYOUT_MAX);
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
            else if (bet > context.Cash)
            {
                throw new DEAException($"You do not have enough money. Balance: {context.Cash.USD()}.");
            }

            decimal roll = (decimal)Config.Random.NextDouble(0, 100);
            if (roll >= odds)
            {
                await _userRepo.EditCashAsync(context, bet * payoutMultiplier);
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Congrats, you won " +
                                                               $"{(bet * payoutMultiplier).USD()}! Balance: {context.Cash.USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(context, -bet);
                await context.Channel.ReplyAsync(context.User, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " +
                                                               $"{bet.USD()}. Balance: {context.Cash.USD()}.");
            }
        }

        public async Task<IReadOnlyDictionary<string, int>> MassOpenCratesAsync(Crate crate, int quantity, User dbUser = null)
        {
            if (quantity > Config.MAX_CRATE_OPEN)
            {
                throw new DEAException($"You may not be open more than {Config.MAX_CRATE_OPEN.ToString("N0")} crates.");
            }

            var itemsToAdd = new Dictionary<string, int>();

            for (int i = 0; i < quantity; i++)
            {
                var item = await OpenCrateAsync(crate);

                if (itemsToAdd.TryGetValue(item.Name, out int itemCount))
                {
                    itemsToAdd[item.Name]++;
                }
                else
                {
                    itemsToAdd.Add(item.Name, 1);
                }
            }

            if (dbUser != null)
            {
                foreach (var item in itemsToAdd)
                {
                    await ModifyInventoryAsync(dbUser, item.Key, item.Value);
                }

                await ModifyInventoryAsync(dbUser, crate.Name, -quantity);
            }

            return itemsToAdd;
        }

        public async Task<Item> OpenCrateAsync(Crate crate, User dbUser = null)
        {
            int cumulative = 0;
            int roll = Config.Random.Next(1, _crateOdds);

            if (crate.ItemOdds >= Config.Random.Roll())
            {
                foreach (var item in _crateItems)
                {
                    cumulative += item.CrateOdds;

                    if (roll < cumulative)
                    {
                        if (dbUser != null)
                        {
                            await ModifyInventoryAsync(dbUser, crate.Name, -1);
                            await ModifyInventoryAsync(dbUser, item.Name);
                        } 
                        return item;
                    }
                }
            }
            else
            {
                if (dbUser != null)
                {
                    await ModifyInventoryAsync(dbUser, crate.Name, -1);
                    await ModifyInventoryAsync(dbUser, "Bullet");
                }
                return _items.First(x => x.Name == "Bullet");
            }
            return null;
        }

        public async Task<Food> AcquireFoodAsync(Type type, int weaponAccuracy, User dbUser = null)
        {
            if (type != typeof(Meat) && type != typeof(Fish))
            {
                throw new Exception("Invalid food type.");
            }

            if (Config.Random.Roll() <= weaponAccuracy)
            {
                int cumulative = 0;
                int sum = type == typeof(Meat) ? _meatOdds : _fishOdds;
                int roll = Config.Random.Next(1, sum + 1);

                foreach (var item in type == typeof(Meat) ? (Food[])_meat : _fish)
                {
                    cumulative += item.AcquireOdds;
                    if (roll < cumulative)
                    {
                        if (dbUser != null)
                        {
                            await ModifyInventoryAsync(dbUser, item.Name);
                        }
                        return item;
                    }
                }
                return null;
            }
            else
            {
                return null;
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

