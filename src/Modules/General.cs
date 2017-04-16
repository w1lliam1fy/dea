using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Models;
using DEA.Database.Repository;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using DEA.Services.Handlers;
using DEA.Services;
using DEA.Common;

namespace DEA.Modules
{
    public class General : DEAModule
    {

        [Command("Investments")]
        [Summary("Increase your money per message")]
        public async Task Invest([Remainder] string investment = null)
        {
            switch (investment)
            {
                case "line":
                    if (Config.LINE_COST > Context.Cash)
                    {
                        await Reply($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
                        break;
                    }
                    if (Context.DbUser.MessageCooldown == Config.LINE_COOLDOWN.TotalMilliseconds)
                    {
                        await Reply($"You have already purchased this investment.");
                        break;
                    }
                    await UserRepository.EditCashAsync(Context, -Config.LINE_COST);
                    await UserRepository.ModifyAsync(Context, x => x.MessageCooldown, Config.LINE_COOLDOWN.TotalMilliseconds);
                    await Reply("Don't forget to wipe your nose when you are done with that line.");
                    break;
                case "pound":
                case "lb":
                    if (Config.POUND_COST > Context.Cash)
                    {
                        await Reply($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
                        break;
                    }
                    if (Context.DbUser.InvestmentMultiplier >= Config.POUND_MULTIPLIER)
                    {
                        await Reply("You already purchased this investment.");
                        break;
                    }
                    await UserRepository.EditCashAsync(Context, -Config.POUND_COST);
                    await UserRepository.ModifyAsync(Context, x => x.InvestmentMultiplier, Config.POUND_MULTIPLIER);
                    await Reply("***DOUBLE CASH SMACK DAB CENTER NIGGA!***");
                    break;
                case "kg":
                case "kilo":
                case "kilogram":
                    if (Config.KILO_COST > Context.Cash)
                    {
                        await Reply($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
                        break;
                    }
                    if (Context.DbUser.InvestmentMultiplier != Config.POUND_MULTIPLIER)
                    {
                        await Reply("You must purchase the pound of cocaine investment before buying this one.");
                        break;
                    }
                    if (Context.DbUser.InvestmentMultiplier >= Config.KILO_MULTIPLIER)
                    {
                        await Reply("You already purchased this investment.");
                        break;
                    }
                    await UserRepository.EditCashAsync(Context, -Config.KILO_COST);
                    await UserRepository.ModifyAsync(Context, x => x.InvestmentMultiplier, Config.KILO_MULTIPLIER);
                    await Reply("Only the black jews would actually enjoy 4$/msg.");
                    break;
                default:
                    await Send($"\n**Cost: {Config.LINE_COST}$** | Command: `{Context.Prefix}investments line` | Description: " +
                        $"One line of blow. Seems like nothing, yet it's enough to lower the message cooldown from 30 to 25 seconds." +
                        $"\n**Cost: {Config.POUND_COST}$** | Command: `{Context.Prefix}investments pound` | Description: " +
                        $"This one pound of coke will double the amount of cash you get per message\n**Cost: {Config.KILO_COST}$** | Command: " +
                        $"`{Context.Prefix}investments kilo` | Description: A kilo of cocaine is more than enough to " +
                        $"quadruple your cash/message.\n These investments stack with the chatting multiplier. However, they will not stack with themselves.",
                        "Available Investments:");
                    break;
            }
        }

        [Command("Leaderboards")]
        [Alias("lb", "rankings", "highscores")]
        [Summary("View the richest Drug Traffickers.")]
        public async Task Leaderboards()
        {
            var users = DEABot.Users.Find(x => x.GuildId == Context.Guild.Id).ToList();
            var sorted = users.OrderByDescending(x => x.Cash);
            string description = string.Empty;
            int position = 1;

            foreach (User user in sorted)
            {
                if (Context.Guild.GetUser(user.UserId) == null)
                {
                    await DEABot.Users.DeleteOneAsync(x => x.Id == user.Id);
                    continue;
                }

                description += $"{position}. <@{user.UserId}>: {user.Cash.ToString("C", Config.CI)}\n";
                if (position >= Config.LEADERBOARD_CAP) break;
                position++;
            }

            if (description.Length == 0) Error("There is nobody on the leaderboards yet!");

            await Send(description, "The Richest Traffickers");
        }

        [Command("Rates")]
        [Alias("highestrate", "ratehighscore", "highestrates", "ratelb", "rateleaderboards")]
        [Summary("View the richest Drug Traffickers.")]
        public async Task Chatters()
        {
            var users = DEABot.Users.Find(y => y.GuildId == Context.Guild.Id).ToList();
            var sorted = users.OrderByDescending(x => x.TemporaryMultiplier);
            string description = string.Empty;
            int position = 1;

            foreach (User user in sorted)
            {
                if (Context.Guild.GetUser(user.UserId) == null)
                {
                    await DEABot.Users.DeleteOneAsync(x => x.Id == user.Id);
                    continue;
                }

                description += $"{position}. <@{user.UserId}>: {user.TemporaryMultiplier.ToString("N2")}\n";
                if (position >= Config.RATELB_CAP) break;
                position++;
            }

            if (description.Length == 0) Error("There is nobody on the leaderboards yet!");

            var builder = new EmbedBuilder()
            {
                Title = $"The Best Chatters",
                Color = new Color(0x00AE86),
                Description = description
            };

            await Send(description, "The Best Chatters");
        }

        [Command("Donate")]
        [Alias("Sauce")]
        [Summary("Sauce some cash to one of your mates.")]
        public async Task Donate(IGuildUser user, decimal money)
        {
            if (user.Id == Context.User.Id) Error("Hey kids! Look at that retard, he is trying to give money to himself!");
            if (money < Config.DONATE_MIN) Error($"Lowest donation is {Config.DONATE_MIN}$.");
            if (Context.Cash < money) Error($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            await UserRepository.EditCashAsync(Context, -money);
            decimal deaMoney = money * Config.DEA_CUT / 100;
            await UserRepository.EditCashAsync(user, money - deaMoney);
            await UserRepository.EditCashAsync(Context.Guild.CurrentUser, deaMoney);
            await Reply($"Successfully donated {(money - deaMoney).ToString("C", Config.CI)} to {ResponseMethods.Name(user, await UserRepository.FetchUserAsync(user))}.\nDEA has taken a {deaMoney.ToString("C", Config.CI)} cut out of this donation. Balance: {(Context.Cash + money - deaMoney).ToString("C", Config.CI)}.");
        }

        [Command("Rank")]
        [Summary("View the detailed ranking information of any user.")]
        public async Task Rank([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            var dbUser = await UserRepository.FetchUserAsync(user);
            var users = DEABot.Users.Find(y => y.GuildId == Context.Guild.Id).ToList();
            var sorted = users.OrderByDescending(x => x.Cash).ToList();
            IRole rank = RankHandler.FetchRankAsync(Context, dbUser);
            var description = $"Balance: {dbUser.Cash.ToString("C", Config.CI)}\n" +
                              $"Position: #{sorted.FindIndex(x => x.UserId == user.Id) + 1}\n";
            if (rank != null)
                description += $"Rank: {rank.Mention}";
            await Send(description, $"Ranking of {await ResponseMethods.TitleNameAsync(user)}");
        }

        [Command("Rate")]
        [Summary("View the money/message rate of anyone.")]
        public async Task Rate([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            var dbUser = await UserRepository.FetchUserAsync(user);
            await Send($"Cash/msg: {(dbUser.TemporaryMultiplier * dbUser.InvestmentMultiplier).ToString("C", Config.CI)}\n" +
                       $"Chatting multiplier: {dbUser.TemporaryMultiplier.ToString("N2")}\n" +
                       $"Investment multiplier: {dbUser.InvestmentMultiplier.ToString("N2")}\n" +
                       $"Message cooldown: {dbUser.MessageCooldown / 1000} seconds",
                       $"Rate of {await ResponseMethods.TitleNameAsync(user)}");
        }

        [Command("Money")]
        [Alias("Cash", "Balance", "Bal")]
        [Summary("View the wealth of anyone.")]
        public async Task Money([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.User as IGuildUser;
            var dbUser = await UserRepository.FetchUserAsync(user);
            await Send($"{ResponseMethods.Name(user, dbUser)}'s balance: {dbUser.Cash.ToString("C", Config.CI)}.");
        }

        [Command("Ranks")]
        [Summary("View all ranks.")]
        public async Task Ranks()
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0) Error("There are no ranks yet!");
            var description = string.Empty;
            foreach (var rank in Context.DbGuild.RankRoles.OrderBy(x => x.Value))
            {
                var role = Context.Guild.GetRole(Convert.ToUInt64(rank.Name));
                if (role == null)
                {
                    Context.DbGuild.RankRoles.Remove(rank.Name);
                    await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.RankRoles, Context.DbGuild.RankRoles);
                    continue;
                }
                description += $"{rank.Value.AsDouble.ToString("C", Config.CI)}: {role.Mention}\n";
            }
            if (description.Length > 2048) Error("You have too many ranks to be able to use this command.");
            await Send(description, "Ranks");
        }

        [Command("ModRoles")]
        [Alias("ModeratorRoles", "ModRole")]
        [Summary("View all the moderator roles.")]
        public async Task ModRoles()
        {
            if (Context.DbGuild.ModRoles.ElementCount == 0) Error("There are no moderator roles yet!");
            var description = "**Moderation Roles:**\n";
            foreach (var modRole in Context.DbGuild.ModRoles.OrderBy(x => x.Value))
            {
                var role = Context.Guild.GetRole(Convert.ToUInt64(modRole.Name));
                if (role == null)
                {
                    Context.DbGuild.ModRoles.Remove(modRole.Name);
                    await GuildRepository.ModifyAsync(Context.Guild.Id, x => x.ModRoles, Context.DbGuild.ModRoles);
                    continue;
                }
                description += $"{role.Mention}: {modRole.Value}\n";
            }
            if (description.Length > 2000) Error("You have too many mod roles to be able to use this command.");
            await Send(description + "\n**Permission Levels:**\n1: Moderator\n2: Administrator\n3: Owner");
        }

        [Command("Cooldowns")]
        [Summary("Check when you can sauce out more cash.")]
        public async Task Cooldowns()
        {
            var cooldowns = new Dictionary<String, TimeSpan>
            {
                { "Whore", Config.WHORE_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.DbUser.Whore)) },
                { "Jump", Config.JUMP_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.DbUser.Jump)) },
                { "Steal", Config.STEAL_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.DbUser.Steal)) },
                { "Rob", Config.ROB_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.DbUser.Rob)) },
                { "Withdraw", Config.WITHDRAW_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.DbUser.Withdraw)) }
            };
            if (Context.Gang != null)
                cooldowns.Add("Raid", Config.RAID_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.Gang.Raid)));
            var description = string.Empty;
            foreach (var cooldown in cooldowns)
            {
                if (cooldown.Value.TotalMilliseconds > 0)
                    description += $"{cooldown.Key}: {cooldown.Value.Hours}:{cooldown.Value.Minutes.ToString("D2")}:{cooldown.Value.Seconds.ToString("D2")}\n";
            }
            if (description.Length == 0) Error("All your commands are available for use!");

            await Send(description, $"All cooldowns for {TitleName()}");
        }

        [Command("Callme")]
        [Summary("Tell the bot what you want it to call you.")]
        public async Task Callme([Remainder] string name)
        {
            if (name.Length > 32) Error("Your DEA nickname may not be longer than 32 characters.");
            name = name.Replace("\n", string.Empty);
            await UserRepository.ModifyAsync(Context, x => x.Name, name);
            await Send($"How's it going, *{name}*?");
        }

    }
}
