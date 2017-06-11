using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Services.Static;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Rob")]
        [Require(Attributes.Rob)]
        [Cooldown]
        [Remarks("\"Sexy John#0007\" 50")]
        [Summary("Slam anyone's bank account.")]
        public async Task Rob(IGuildUser user, decimal resources)
        {
            if (user.Id == Context.User.Id)
            {
                ReplyError("Only the *retards* try to rob themselves. Are you a retard?");
            }
            else if (resources < Config.MinResources)
            {
                ReplyError($"The minimum amount of money to spend on resources for a robbery is {Config.MinResources.USD()}.");
            }
            else if (Context.Cash < resources)
            {
                ReplyError($"You don't have enough money. Balance: {Context.Cash.USD()}.");
            }

            var raidedDbUser = await _userRepo.GetUserAsync(user);
            if (resources > Math.Round(raidedDbUser.Cash * Config.RobCap / 2, 2))
            {
                ReplyError($"You are overkilling it. You only need {(raidedDbUser.Cash * Config.RobCap / 2).USD()} " +
                           $"to rob {Config.RobCap.ToString("P")} of their cash, that is {(raidedDbUser.Cash * Config.RobCap).USD()}.");
            }

            var stolen = resources * 2;

            int roll = CryptoRandom.Roll();

            var successOdds = await _gangRepo.InGangAsync(Context.GUser) ? Config.RobOdds - 5 : Config.RobOdds;

            if (successOdds > roll)
            {
                await _userRepo.EditCashAsync(user, Context.DbGuild, raidedDbUser, -stolen);
                await _userRepo.EditCashAsync(Context, stolen);

                await user.Id.TryDMAsync(Context.Client, $"{Context.User} just robbed you and managed to walk away with {stolen.USD()}.");

                await ReplyAsync($"With a {successOdds}.00% chance of success, you successfully stole {stolen.USD()}. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                await _userRepo.EditCashAsync(Context, -resources);

                await user.Id.TryDMAsync(Context.Client, $"{Context.User} tried to rob your sweet cash, but the nigga slipped on a banana peel and got arrested :joy: :joy: :joy:.");

                await ReplyAsync($"With a {successOdds}.00% chance of success, you failed to steal {stolen.USD()} " +
                                 $"and lost all resources in the process. Balance: {Context.Cash.USD()}.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Rob", Config.RobCooldown));
        }
    }
}
