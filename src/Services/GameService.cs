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
using DEA.Services.Static;

namespace DEA.Services
{
    public sealed class GameService
    {
        private readonly InteractiveService _interactiveService;
        private readonly UserRepository _userRepo;

        public GameService(InteractiveService interactiveService, UserRepository userRepo)
        {
            _interactiveService = interactiveService;
            _userRepo = userRepo;
        }

        public async Task TriviaAsync(IMessageChannel channel, Guild dbGuild)
        {
            if (dbGuild.Trivia.ElementCount == 0)
            {
                throw new FriendlyException("There are no trivia questions yet!");
            }

            int roll = CryptoRandom.Next(dbGuild.Trivia.ElementCount);

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
                var winnings = CryptoRandom.NextDecimal(Config.MinTriviaPayout, Config.MaxTriviaPayout);
                await _userRepo.EditCashAsync(user, dbGuild, await _userRepo.GetUserAsync(user), winnings);
                await channel.SendAsync($"{user.Boldify()}, Congrats! You just won {winnings.USD()} for correctly answering \"{element.Value.AsString}\".");
            }
            else
            {
                await channel.SendAsync($"NOBODY got the right answer for the trivia question! Alright, I'll sauce it to you guys, but next time " +
                                        $"you are on your own. The right answer is: \"{element.Value.AsString}\".");
            }
        }

        public async Task GambleAsync(Context context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            var gambleChannel = context.Guild.GetTextChannel(context.DbGuild.GambleChannelId);

            if (gambleChannel != null && context.Channel.Id != context.DbGuild.GambleChannelId)
            {
                throw new FriendlyException($"You may only gamble in {gambleChannel.Mention}.");
            }
            else if (bet < Config.MinBet)
            {
                throw new FriendlyException($"The lowest bet is {Config.MinBet}$.");
            }
            else if (bet > context.Cash)
            {
                throw new FriendlyException($"You do not have enough money. Balance: {context.Cash.USD()}.");
            }

            decimal roll = CryptoRandom.NextDecimal(0, 100);
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
            if (quantity > Config.MaxCrateOpen)
            {
                throw new FriendlyException($"You may not be open more than {Config.MaxCrateOpen.ToString("N0")} crates.");
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
            int roll = CryptoRandom.Next(1, Data.CrateItemOdds);

            if (crate.ItemOdds >= CryptoRandom.Roll())
            {
                foreach (var item in Data.CrateItems)
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
                return Data.Items.First(x => x.Name == "Bullet");
            }
            return null;
        }

        public async Task<Food> AcquireFoodAsync(Type type, int weaponAccuracy, User dbUser = null)
        {
            if (type != typeof(Meat) && type != typeof(Fish))
            {
                throw new Exception("Invalid food type.");
            }

            if (CryptoRandom.Roll() <= weaponAccuracy)
            {
                int cumulative = 0;
                int sum = type == typeof(Meat) ? Data.MeatAcquireOdds : Data.FishAcquireOdds;
                int roll = CryptoRandom.Next(1, sum + 1);

                foreach (var item in type == typeof(Meat) ? (Food[])Data.Meat : Data.Fish)
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
            return Data.Items.Where(x => dbUser.Inventory.Names.Any(y => y == x.Name));
        }
    }
}

