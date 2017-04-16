using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repository;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    [RequireCooldown]
    public class Crime : DEAModule
    {

        [Command("Whore")]
        [Summary("Sell your body for some quick cash.")]
        public async Task Whore()
        {
            await UserRepository.ModifyAsync(Context, x => x.Whore, DateTime.UtcNow);
            int roll = new Random().Next(1, 101);
            if (roll > Config.WHORE_ODDS)
            {
                await UserRepository.EditCashAsync(Context, -Config.WHORE_FINE);
                await Reply($"What are the fucking odds that one of your main clients was a cop... " +
                            $"You are lucky you only got a {Config.WHORE_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.WHORE_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyWhored = (new Random().Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100m;
                await UserRepository.EditCashAsync(Context, moneyWhored);
                await Reply($"You whip it out and manage to rake in {moneyWhored.ToString("C", Config.CI)}. Balance: {(Context.Cash + moneyWhored).ToString("C", Config.CI)}.");
            }
        }

        [Command("Jump")]
        [Require(Attributes.Jump)]
        [Summary("Jump some random nigga in the hood.")]
        public async Task Jump()
        {
            await UserRepository.ModifyAsync(Context, x => x.Jump, DateTime.UtcNow);
            int roll = new Random().Next(1, 101);
            if (roll > Config.JUMP_ODDS)
            {
                await UserRepository.EditCashAsync(Context, -Config.JUMP_FINE);
                await Reply($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                            $"Court's final ruling was a {Config.JUMP_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.JUMP_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyJumped = (new Random().Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100m;
                await UserRepository.EditCashAsync(Context, moneyJumped);
                await Reply($"You jump some random nigga on the streets and manage to get {moneyJumped.ToString("C", Config.CI)}. Balance: {(Context.Cash + moneyJumped).ToString("C", Config.CI)}.");
            }
        }

        [Command("Steal")]
        [Require(Attributes.Steal)]
        [Summary("Snipe some goodies from your local stores.")]
        public async Task Steal()
        {
            await UserRepository.ModifyAsync(Context, x => x.Steal, DateTime.UtcNow);
            int roll = new Random().Next(1, 101);
            if (roll > Config.STEAL_ODDS)
            {
                await UserRepository.EditCashAsync(Context, -Config.STEAL_FINE);
                await Reply($"You were on your way out with the cash, but then some hot chick asked you if you " +
                            $"wanted to bust a nut. Turns out she was cop, and raped you before turning you in. Since she passed on some " +
                            $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                            $"{Config.STEAL_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.STEAL_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyStolen = (new Random().Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100m;
                await UserRepository.EditCashAsync(Context, moneyStolen);
                string randomStore = Config.STORES[new Random().Next(1, Config.STORES.Length) - 1];
                await Reply($"You walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                            $"with {moneyStolen.ToString("C", Config.CI)}. Balance: {(Context.Cash + moneyStolen).ToString("C", Config.CI)}.");
            }
        }

        [Command("Bully")]
        [Require(Attributes.Bully)]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(IGuildUser userToBully, [Remainder] string nickname)
        {
            if (nickname.Length > 32) await ErrorAsync("The length of a nickname may not be longer than 32 characters.");
            if (await IsModAsync(Context, userToBully))
                await ErrorAsync("You may not bully a moderator.");
            if ((await UserRepository.FetchUserAsync(userToBully)).Cash > Context.Cash)
                await ErrorAsync("You may not bully a user with more money than you.");
            await userToBully.ModifyAsync(x => x.Nickname = nickname);
            await Send($"{ResponseMethods.Name(userToBully, await UserRepository.FetchUserAsync(userToBully))} just got ***BULLIED*** by {await NameAsync()} with his new nickname: \"{nickname}\".");
        }

        [Command("Rob")]
        [Require(Attributes.Rob)]
        [Summary("Lead a large scale operation on a local bank.")]
        public async Task Rob(decimal resources)
        {
            if (Context.Cash < resources) await ErrorAsync($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}");
            if (resources < Config.MIN_RESOURCES) await ErrorAsync($"The minimum amount of money to spend on resources for a robbery is {Config.MIN_RESOURCES.ToString("C", Config.CI)}.");
            if (resources > Config.MAX_RESOURCES) await ErrorAsync($"The maximum amount of money to spend on resources for a robbery is {Config.MAX_RESOURCES.ToString("C", Config.CI)}.");
            await UserRepository.ModifyAsync(Context, x => x.Rob, DateTime.UtcNow);
            Random rand = new Random();
            decimal succesRate = rand.Next(Config.MIN_ROB_ODDS * 100, Config.MAX_ROB_ODDS * 100) / 10000m;
            decimal moneyStolen = resources / (succesRate / 1.50m); 
            string randomBank = Config.BANKS[rand.Next(1, Config.BANKS.Length) - 1];
            if (rand.Next(10000) / 10000m >= succesRate)
            {
                await UserRepository.EditCashAsync(Context, moneyStolen);
                await Reply($"With a {succesRate.ToString("P")} chance of success, you successfully stole " +
                            $"{moneyStolen.ToString("C", Config.CI)} from the {randomBank}. Balance: {(Context.Cash + moneyStolen).ToString("C", Config.CI)}$.");
            }
            else
            {
                await UserRepository.EditCashAsync(Context, -resources);
                await Reply($"With a {succesRate.ToString("P")} chance of success, you failed to steal " +
                            $"{moneyStolen.ToString("C", Config.CI)} from the {randomBank}, losing all resources in the process. Balance: {(Context.Cash - resources).ToString("C", Config.CI)}.");
            }
        }

    }
}
