using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repositories;
using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Services;
using DEA.Common.Extensions;
using DEA.Common.Data;

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
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Whore = DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.WHORE_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.WHORE_FINE);

                await ReplyAsync($"What are the fucking odds that one of your main clients was a cop... " +
                            $"You are lucky you only got a {Config.WHORE_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyWhored = (new Random().Next((int)(Config.MIN_WHORE) * 100, (int)(Config.MAX_WHORE) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyWhored);

                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.USD()}. Balance: {Context.Cash.USD()}.");
            }
        }

        [Command("Jump")]
        [Require(Attributes.Jump)]
        [Summary("Jump some random nigga in the hood.")]
        public async Task Jump()
        {
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Jump = DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.JUMP_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.JUMP_FINE);

                await ReplyAsync($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                            $"Court's final ruling was a {Config.JUMP_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyJumped = (new Random().Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyJumped);

                await ReplyAsync($"You jump some random nigga on the streets and manage to get {moneyJumped.USD()}. Balance: {Context.Cash.USD()}.");
            }
        }

        [Command("Steal")]
        [Require(Attributes.Steal)]
        [Summary("Snipe some goodies from your local stores.")]
        public async Task Steal()
        {
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Steal = DateTime.UtcNow);

            int roll = new Random().Next(1, 101);
            if (roll > Config.STEAL_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.STEAL_FINE);
                await ReplyAsync($"You were on your way out with the cash, but then some hot chick asked you if you " +
                            $"wanted to bust a nut. Turns out she was cop, and raped you before turning you in. Since she passed on some " +
                            $"nice words to the judge about you not resisting arrest, you managed to walk away with only a " +
                            $"{Config.STEAL_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyStolen = (new Random().Next((int)(Config.MIN_STEAL) * 100, (int)(Config.MAX_STEAL) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyStolen);

                string randomStore = Config.STORES[new Random().Next(1, Config.STORES.Length) - 1];
                await ReplyAsync($"You walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                            $"with {moneyStolen.USD()}. Balance: {Context.Cash.USD()}.");
            }
        }

        [Command("Bully")]
        [Require(Attributes.Bully)]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(IGuildUser userToBully, [Remainder] string nickname)
        {
            if (nickname.Length > 32)
            {
                ReplyError("The length of a nickname may not be longer than 32 characters.");
            }
            else if (_moderationService.GetPermLevel(Context, userToBully) > 0)
            {
                ReplyError("You may not bully a moderator.");
            }
            else if ((await _userRepo.GetUserAsync(userToBully)).Cash >= Context.Cash)
            {
                ReplyError("You may not bully a user with more money than you.");
            }

            await userToBully.ModifyAsync(x => x.Nickname = nickname);
            await SendAsync($"{userToBully.Boldify()} just got ***BULLIED*** by {Context.User.Boldify()} with his new nickname: \"{nickname}\".");
        }

        [Command("Rob")]
        [Require(Attributes.Rob)]
        [Summary("Slam anyone's bank account.")]
        public async Task Rob(decimal resources, [Remainder] IGuildUser user)
        {
            if (user.Id == Context.User.Id)
            {
                ReplyError("Only the *retards* try to rob themselves. Are you a retard?");
            }
            else if (resources < Config.MIN_RESOURCES)
            {
                ReplyError($"The minimum amount of money to spend on resources for a robbery is {Config.MIN_RESOURCES.USD()}.");
            }
            else if (Context.Cash < resources)
            {
                ReplyError($"You don't have enough money. Balance: {Context.Cash.USD()}.");
            }

            var raidedDbUser = await _userRepo.GetUserAsync(user);
            if (resources > Math.Round(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE / 2, 2))
            {
                ReplyError($"You are overkilling it. You only need {(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE / 2).USD()} " +
                           $"to rob {Config.MAX_ROB_PERCENTAGE.ToString("P")} of their cash, that is {(raidedDbUser.Cash * Config.MAX_ROB_PERCENTAGE).USD()}.");
            }

            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);

            var successOdds = await _gangRepo.InGangAsync(Context.GUser) ? Config.ROB_SUCCESS_ODDS - 5 : Config.ROB_SUCCESS_ODDS;

            if (successOdds > roll)
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, raidedDbUser, -stolen);
                await _userRepo.EditCashAsync(Context, stolen);

                await user.Id.DMAsync(Context.Client, $"{Context.User} just robbed you and managed to walk away with {stolen.USD()}.");

                await ReplyAsync($"With a {successOdds}.00% chance of success, you successfully stole {stolen.USD()}. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(Context, -resources);

                await user.Id.DMAsync(Context.Client, $"{Context.User} tried to rob your sweet cash, but the nigga slipped on a banana peel and got arrested :joy: :joy: :joy:.");

                await ReplyAsync($"With a {successOdds}.00% chance of success, you failed to steal {stolen.USD()} " +
                                 $"and lost all resources in the process. Balance: {Context.Cash.USD()}.");
            }
            await _userRepo.ModifyAsync(Context.DbUser, x => x.Rob = DateTime.UtcNow);
        }
        [Command("Shop")]
        [Summary("List of available shop items.")]
        public async Task Shop([Summary("Bullets")]string item = null)
        {
            switch (item)
            {
                case $"{Items.Revolver}":
                    if (Config.REVOLVER_COST > Context.Cash)
                    {
                        ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                    }
                    if (Context.DbUser.Inventory.Contains("Revolver")){
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory["Revolver"] += 1);
                    }
                    else {
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory.Add("Revolver", 1));
                    }
                    await _userRepo.EditCash(Context.DbUser, -Config.REVOLVER_COST);
                    await ReplyAsync("Don't forget to load it.");
                    break;
                case $"{Items.AR15}":
                    if (Config.AR15_COST > Context.Cash)
                    {
                        ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                    }
                    if (Context.DbUser.Inventory.Contains("AR15")){
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory["AR15"] += 1);
                    }
                    else {
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory.Add("AR15", 1));
                    }                    
                    await _userRepo.EditCash(Context.DbUser, -Config.AR15_COST);
                    await ReplyAsync("Be careful, those things are automatic.");
                    break;
                case $"{Items.Dragunov}":
                    if (Config.DRAGUNOV_COST > Context.Cash)
                    {
                        ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                    }         
                    if (Context.DbUser.Inventory.Contains("Dragunov")){
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory["Dragunov"] += 1);
                    }
                    else {
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory.Add("Dragunov", 1));
                    }                    
                    await _userRepo.EditCash(Context.DbUser, -Config.DRAGUNOV_COST);
                    await ReplyAsync("360noscope that kiddo.");
                    break;
                case $"{Items.Bullets}":
                    if (Config.BULLETS_COST > Context.Cash)
                    {
                        ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                    }
                    if (Context.DbUser.Inventory.Contains("Bullets")){
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory["Bullets"] += 1);
                    }
                    else {
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory.Add("Bullets", 1));
                    }                    
                    await _userRepo.EditCash(Context.DbUser, -Config.BULLETS_COST);
                    await ReplyAsync("Remember, you put this in the guns ***not*** the toaster.");
                    break;
                case $"{Items.Kevlar}":
                    if (Config.KEVLAR_COST > Context.Cash)
                    {
                        ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
                    }
                    if (Context.DbUser.Inventory.Contains("Kevlar")){
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory["Kevlar"] += 1);
                    }
                    else {
                        await _userRepo.ModifyAsync(Context.DbUser, x => x.Inventory.Add("Kevlar", 1));
                    }                   
                    await _userRepo.EditCash(Context.DbUser, -CONFIG.KEVLAR_COST);
                    await ReplyAsync("Hopefully this adds a little piece of armor to that stomach of yours.");
                    break;
                case null:
                     await SendAsync($@"\n **Cost: {Config.REVOLVER_COST}$** | Command: `{Config.Prefix}shop revolver` | Description: 
                                    Pop a glock in a foe. Enough to increase chance by 5%.\n **Cost: {Config.AR15_COST}$** | Command: `{Config.Prefix}shop AR15`
                                    Description: Assault fire. Assault weapon. 15% assault increase.\n **Cost: {Config.DRAGUNOV_COST}$** |
                                    Command: `{Config.Prefix}shop dragunov` | Description: 360 noscope sniper shot, 20% increase.\n **Cost: {Config.BULLETS_COST}$** |
                                    Command: `{Config.Prefix}shop bullets` | Description: You can't shoot a gun without bullets, or did you think?\n
                                    **Cost: {Config.KEVLAR_COST}$** | Command: `{Config.Prefix}shop kevlar` | Description: Adds a piece of armor for protection. 20% less likely to be fatally shot", "Available Shop Items");
                    break;                   
                default:
                    await ReplyAsync("That shop item doesn't exist");
                    break;
            }
        }
    }
}
