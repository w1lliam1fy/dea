using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord.WebSocket;
using System.Linq;

namespace DEA.Modules
{
    public class Crime : ModuleBase<SocketCommandContext>
    {

        [Command("Whore")]
        [RequireCooldown]
        [Summary("Sell your body for some quick cash.")]
        [Remarks("Whore")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Whore()
        {
            using (var db = new DbContext())
            {
                
                
                var user = await UserRepository.FetchUserAsync(Context.User.Id);
                int roll = new Random().Next(1, 101);
                if (roll > Config.WHORE_ODDS && !Config.SPONSOR_IDS.Any(x => x == Context.User.Id))
                {
                    await UserRepository.EditCashAsync(Context, -Config.WHORE_FINE);
                    await ReplyAsync($"{Context.User.Mention}, what are the fucking odds that one of your main clients was a cop... " +
                                     $"You are lucky you only got a {Config.WHORE_FINE.ToString("C2")} fine. Balance: {user.Cash.ToString("C2")}");
                }
                else
                {
                    double moneyWhored = (double)(new Random().Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100;
                    await UserRepository.EditCashAsync(Context, moneyWhored);
                    await ReplyAsync($"{Context.User.Mention}, you whip it out and manage to rake in {moneyWhored.ToString("C2")}. Balance: {user.Cash.ToString("C2")}");
                }
                await UserRepository.ModifyAsync(x => { x.LastWhore = DateTime.Now.ToString(); return Task.CompletedTask; }, Context.User.Id);
            }
        }

        [Command("Jump")]
        [RequireRank(1)]
        [RequireCooldown]
        [Summary("Jump some random nigga in the hood.")]
        [Remarks("Jump")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Jump()
        {
            using (var db = new DbContext())
            {
                
                
                var user = await UserRepository.FetchUserAsync(Context.User.Id);
                int roll = new Random().Next(1, 101);
                if (roll > Config.JUMP_ODDS && !Config.SPONSOR_IDS.Any(x => x == Context.User.Id))
                {
                    await UserRepository.EditCashAsync(Context, -Config.JUMP_FINE);
                    await ReplyAsync($"{Context.User.Mention}, turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                                     $"Court's final ruling was a {Config.JUMP_FINE.ToString("C2")} fine. Balance: {user.Cash.ToString("C2")}");
                }
                else
                {
                    double moneyJumped = (double)(new Random().Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100;
                    await UserRepository.EditCashAsync(Context, moneyJumped);
                    await ReplyAsync($"{Context.User.Mention}, you jump some random nigga on the streets and manage to get {moneyJumped.ToString("C2")}. Balance: {user.Cash.ToString("C2")}");
                }
                await UserRepository.ModifyAsync(x => { x.LastJump = DateTime.Now.ToString(); return Task.CompletedTask; }, Context.User.Id);
            }
        }

        [Command("Steal")]
        [RequireRank(2)]
        [RequireCooldown]
        [Summary("Snipe some goodies from your local stores.")]
        [Remarks("Steal")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Steal()
        {
            using (var db = new DbContext())
            {
                
                var user = await UserRepository.FetchUserAsync(Context.User.Id);
                int roll = new Random().Next(1, 101);
                if (roll > Config.STEAL_ODDS && !Config.SPONSOR_IDS.Any(x => x == Context.User.Id))
                {
                    await UserRepository.EditCashAsync(Context, -Config.STEAL_FINE);
                    await ReplyAsync($"{Context.User.Mention}, you were on your way out with the cash, but then some hot chick asked you if you " +
                                     $"wanted to bust a nut. Turns out she was cop, and raped you before turning you in. Since she passed on some " +
                                     $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                                     $"{Config.STEAL_FINE.ToString("C2")} fine. Balance: {user.Cash.ToString("C2")}");
                }
                else
                {
                    double moneySteal = (double)(new Random().Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100;
                    await UserRepository.EditCashAsync(Context, moneySteal);
                    string randomStore = Config.STORES[new Random().Next(1, Config.STORES.Length) - 1];
                    await ReplyAsync($"{Context.User.Mention}, you walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                                     $"with {moneySteal.ToString("C2")}. Balance: {user.Cash.ToString("C2")}");
                }
                await UserRepository.ModifyAsync(x => { x.LastSteal = DateTime.Now.ToString(); return Task.CompletedTask; }, Context.User.Id);
            }
        }

        [Command("Bully")]
        [RequireRank(3)]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [Remarks("Bully <@User> <Nickname>")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(SocketGuildUser userToBully, [Remainder] string nickname)
        {
            if (nickname.Length > 32) throw new Exception("The length of a nickname may not be longer than 32 characters.");
            using (var db = new DbContext())
            {
                
                var role3 = Context.Guild.GetRole((await GuildRepository.FetchGuildAsync(Context.Guild.Id)).RankIds[2]);
                if (role3.Position <= userToBully.Roles.OrderByDescending(x => x.Position).First().Position)
                    throw new Exception($"You cannot bully someone with role higher or equal to: {role3.Mention}");
                await userToBully.ModifyAsync(x => x.Nickname = nickname);
                await ReplyAsync($"{userToBully.Mention} just got ***BULLIED*** by {Context.User.Mention} with his new nickname: \"{nickname}\".");
            }
        }

        [Command("Rob")]
        [RequireRank(4)]
        [RequireCooldown]
        [Summary("Lead a large scale operation on a local bank.")]
        [Remarks("Rob <Amount of cash to spend on resources>")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Rob(double resources)
        {
            using (var db = new DbContext())
            {
                
                var user = await UserRepository.FetchUserAsync(Context.User.Id);
                if (user.Cash < resources) throw new Exception($"You do not have enough money. Balance: {user.Cash.ToString("C2")}");
                if (resources < Config.MIN_RESOURCES) throw new Exception($"The minimum amount of money to spend on resources for rob is {Config.MIN_RESOURCES.ToString("C2")}.");
                if (resources > Config.MAX_RESOURCES) throw new Exception($"The maximum amount of money to spend on resources for rob is {Config.MAX_RESOURCES.ToString("C2")}.");
                Random rand = new Random();
                double succesRate = rand.Next(Config.MIN_ROB_ODDS * 100, Config.MAX_ROB_ODDS * 100) / 10000f;
                double moneyStolen = resources / (succesRate / 1.50f);
                await UserRepository.ModifyAsync(x => { x.LastRob = DateTime.Now.ToString(); return Task.CompletedTask; }, Context.User.Id);
                string randomBank = Config.BANKS[rand.Next(1, Config.BANKS.Length) - 1];
                if (rand.Next(10000) / 10000f  >= succesRate)
                {
                    await UserRepository.EditCashAsync(Context, moneyStolen);
                    await ReplyAsync($"{Context.User.Mention}, with a {succesRate.ToString("P")} chance of success, you successfully stole " +
                    $"{moneyStolen.ToString("C2")} from the {randomBank}. Balance: {(await UserRepository.FetchUserAsync(Context.User.Id)).Cash.ToString("C2")}$.");
                }
                else
                {
                    await UserRepository.EditCashAsync(Context, -resources);
                    await ReplyAsync($"{Context.User.Mention}, with a {succesRate.ToString("P")} chance of success, you failed to steal " +
                    $"{moneyStolen.ToString("C2")} from the {randomBank}, losing all resources in the process. Balance: {(await UserRepository.FetchUserAsync(Context.User.Id)).Cash.ToString("C2")}.");
                }
            }
        }
    }
}
