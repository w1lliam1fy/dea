using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repository;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    [RequireCooldown]
    public class Crime : DEAModule
    {
        private UserRepository _userRepo;
        private GangRepository _gangRepo;

        public Crime(UserRepository userRepo, GangRepository gangRepo)
        {
            _userRepo = userRepo;
            _gangRepo = gangRepo;
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
                await Reply($"What are the fucking odds that one of your main clients was a cop... " +
                            $"You are lucky you only got a {Config.WHORE_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.WHORE_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyWhored = (new Random().Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyWhored);
                await Reply($"You whip it out and manage to rake in {moneyWhored.ToString("C", Config.CI)}. Balance: {(Context.Cash + moneyWhored).ToString("C", Config.CI)}.");
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
                await Reply($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                            $"Court's final ruling was a {Config.JUMP_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.JUMP_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyJumped = (new Random().Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyJumped);
                await Reply($"You jump some random nigga on the streets and manage to get {moneyJumped.ToString("C", Config.CI)}. Balance: {(Context.Cash + moneyJumped).ToString("C", Config.CI)}.");
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
                await Reply($"You were on your way out with the cash, but then some hot chick asked you if you " +
                            $"wanted to bust a nut. Turns out she was cop, and raped you before turning you in. Since she passed on some " +
                            $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                            $"{Config.STEAL_FINE.ToString("C", Config.CI)} fine. Balance: {(Context.Cash - Config.STEAL_FINE).ToString("C", Config.CI)}.");
            }
            else
            {
                decimal moneyStolen = (new Random().Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyStolen);
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
            if ((await _userRepo.FetchUserAsync(userToBully)).Cash > Context.Cash)
                await ErrorAsync("You may not bully a user with more money than you.");
            await userToBully.ModifyAsync(x => x.Nickname = nickname);
            await Send($"{await NameAsync(userToBully, await _userRepo.FetchUserAsync(userToBully))} just got ***BULLIED*** by {await NameAsync()} with his new nickname: \"{nickname}\".");
        }

        [Command("Rob")]
        [Require(Attributes.Rob)]
        [Summary("Lead a large scale operation on a local bank.")]
        public async Task Rob(decimal resources, [Remainder] IGuildUser user)
        {
            if (await _gangRepo.InGangAsync(user))
                await ErrorAsync("You can't rob this nigga! He in a ***gang***. If you try to rob him, his crew would fuck you up till your dick " +
                                 "poppin out of your left cheek, ***nigga!*** Why don't you try and `$raid` his gang instead?");

            if (resources < Config.MIN_RESOURCES) await ErrorAsync($"The minimum amount of money to spend on resources for a robbery is {Config.MIN_RESOURCES.ToString("C", Config.CI)}.");
            if (Context.Cash < resources) await ErrorAsync($"You don't have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");

            var raidedDbUser = await _userRepo.FetchUserAsync(user);
            if (Math.Round(resources, 2) > Math.Round(raidedDbUser.Cash / 20m, 2))
                await ErrorAsync($"You are overkilling it. You only need {(raidedDbUser.Cash / 20).ToString("C", Config.CI)} " +
                      $"to rob 10% of their cash, that is {(raidedDbUser.Cash / 10).ToString("C", Config.CI)}.");
            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);
            if (Config.ROB_SUCCESS_ODDS > roll)
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, raidedDbUser, -stolen);
                await _userRepo.EditCashAsync(Context, stolen);
                await _userRepo.ModifyAsync(Context, x => x.Rob, DateTime.UtcNow);

                await DM(user.Id, $"{Context.User} just robbed you and managed to walk away with {stolen.ToString("C", Config.CI)}.");

                await Reply($"With a {Config.ROB_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.ToString("C", Config.CI)}. " +
                            $"Balance: {(Context.Cash + stolen).ToString("C", Config.CI)}.");
            }
            else
            {
                await _userRepo.EditCashAsync(Context, stolen);
                await _userRepo.ModifyAsync(Context, x => x.Rob, DateTime.UtcNow);

                await DM(user.Id, $"{Context.User} tried to rob your sweet cash, but the nigga slipped on a banana peel and got arrested :joy: :joy: :joy:.");

                await Reply($"With a {Config.ROB_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.ToString("C", Config.CI)} " +
                            $"and lost all resources in the process.");
            }
        }

    }
}
