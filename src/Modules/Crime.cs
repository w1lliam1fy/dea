using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repositories;
using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Services;
using DEA.Common.Extensions;

namespace DEA.Modules
{
    [RequireCooldown]
    public class Crime : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly GangRepository _gangRepo;
        private readonly ModerationService _moderationService;

        public Crime(UserRepository userRepo, GangRepository gangRepo, ModerationService moderationService)
        {
            _userRepo = userRepo;
            _gangRepo = gangRepo;
            _moderationService = moderationService;
        }

        [Command("Whore")]
        [Summary("Sell your body for some quick cash.")]
        public async Task Whore()
        {
            await _userRepo.ModifyAsync(Context, x => x.Whore, DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.WHORE_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.WHORE_FINE);

                await ReplyAsync($"What are the fucking odds that one of your main clients was a cop... " +
                            $"You are lucky you only got a {Config.WHORE_FINE.USD()} fine. Balance: {(Context.Cash - Config.WHORE_FINE).USD()}.");
            }
            else
            {
                decimal moneyWhored = (new Random().Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyWhored);

                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.USD()}. Balance: {(Context.Cash + moneyWhored).USD()}.");
            }
        }

        [Command("Jump")]
        [Require(Attributes.Jump)]
        [Summary("Jump some random nigga in the hood.")]
        public async Task Jump()
        {
            await _userRepo.ModifyAsync(Context, x => x.Jump, DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.JUMP_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.JUMP_FINE);

                await ReplyAsync($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                            $"Court's final ruling was a {Config.JUMP_FINE.USD()} fine. Balance: {(Context.Cash - Config.JUMP_FINE).USD()}.");
            }
            else
            {
                decimal moneyJumped = (new Random().Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyJumped);

                await ReplyAsync($"You jump some random nigga on the streets and manage to get {moneyJumped.USD()}. Balance: {(Context.Cash + moneyJumped).USD()}.");
            }
        }

        [Command("Steal")]
        [Require(Attributes.Steal)]
        [Summary("Snipe some goodies from your local stores.")]
        public async Task Steal()
        {
            await _userRepo.ModifyAsync(Context, x => x.Steal, DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.STEAL_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.STEAL_FINE);
                await ReplyAsync($"You were on your way out with the cash, but then some hot chick asked you if you " +
                            $"wanted to bust a nut. Turns out she was cop, and raped you before turning you in. Since she passed on some " +
                            $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                            $"{Config.STEAL_FINE.USD()} fine. Balance: {(Context.Cash - Config.STEAL_FINE).USD()}.");
            }
            else
            {
                decimal moneyStolen = (new Random().Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyStolen);

                string randomStore = Config.STORES[new Random().Next(1, Config.STORES.Length) - 1];
                await ReplyAsync($"You walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                            $"with {moneyStolen.USD()}. Balance: {(Context.Cash + moneyStolen).USD()}.");
            }
        }

        [Command("Bully")]
        [Require(Attributes.Bully)]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(IGuildUser userToBully, [Remainder] string nickname)
        {
            if (nickname.Length > 32)
                await ErrorAsync("The length of a nickname may not be longer than 32 characters.");
            if (await _moderationService.IsModAsync(Context, userToBully))
                await ErrorAsync("You may not bully a moderator.");
            if ((await _userRepo.FetchUserAsync(userToBully)).Cash > Context.Cash)
                await ErrorAsync("You may not bully a user with more money than you.");

            await userToBully.ModifyAsync(x => x.Nickname = nickname);
            await SendAsync($"{userToBully} just got ***BULLIED*** by {Context.User} with his new nickname: \"{nickname}\".");
        }

        [Command("Rob")]
        [Require(Attributes.Rob)]
        [Summary("Lead a large scale operation on a local bank.")]
        public async Task Rob(decimal resources, [Remainder] IGuildUser user)
        {
            if (await _gangRepo.InGangAsync(user))
                await ErrorAsync("You can't rob this nigga! He in a ***gang***. If you try to rob him, his crew would fuck you up till your dick " +
                                 "poppin out of your left cheek, ***nigga!*** Why don't you try and `$raid` his gang instead?");

            if (resources < Config.MIN_RESOURCES)
                await ErrorAsync($"The minimum amount of money to spend on resources for a robbery is {Config.MIN_RESOURCES.USD()}.");
            if (Context.Cash < resources)
                await ErrorAsync($"You don't have enough money. Balance: {Context.Cash.USD()}.");

            var raidedDbUser = await _userRepo.FetchUserAsync(user);
            if (Math.Round(resources, 2) > Math.Round(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE / 2, 2))
                await ErrorAsync($"You are overkilling it. You only need {(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE / 2).USD()} " +
                      $"to rob {Config.MAX_ROB_PERCENTAGE.ToString("P")} of their cash, that is {(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE).USD()}.");

            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);
            if (Config.ROB_SUCCESS_ODDS > roll)
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, raidedDbUser, -stolen);
                await _userRepo.EditCashAsync(Context, stolen);
                await _userRepo.ModifyAsync(Context, x => x.Rob, DateTime.UtcNow);

                await user.Id.DMAsync(Context.Client, $"{Context.User} just robbed you and managed to walk away with {stolen.USD()}.");

                await ReplyAsync($"With a {Config.ROB_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.USD()}. " +
                            $"Balance: {(Context.Cash + stolen).USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(Context, -resources);
                await _userRepo.ModifyAsync(Context, x => x.Rob, DateTime.UtcNow);

                await user.Id.DMAsync(Context.Client, $"{Context.User} tried to rob your sweet cash, but the nigga slipped on a banana peel and got arrested :joy: :joy: :joy:.");

                await ReplyAsync($"With a {Config.ROB_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.USD()} " +
                            $"and lost all resources in the process. Balance: {(Context.Cash - resources).USD()}.");
            }
        }

    }
}
