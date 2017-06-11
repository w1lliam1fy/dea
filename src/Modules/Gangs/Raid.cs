using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Services.Static;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("Raid")]
        [Require(Attributes.InGang)]
        [Cooldown]
        [Remarks("\"SLAM EM BOYS\" 50")]
        [Summary("Raid another gang in attempt to steal some of their wealth.")]
        public async Task Raid(string gangName, decimal resources)
        {
            if (resources < Config.MinResources)
            {
                ReplyError($"The minimum amount of money to spend on resources for a raid is {Config.MinResources.USD()}.");
            }
            else if (Context.Gang.Wealth < resources)
            {
                ReplyError($"Your gang does not have enough money. {Context.Gang.Name}'s Wealth {Context.Gang.Wealth.USD()}.");
            }

            var raidedGang = await _gangRepo.GetGangAsync(gangName, Context.Guild.Id);

            if (raidedGang == null)
            {
                ReplyError("This gang does not exist.");
            }
            else if (Math.Round(resources, 2) > Math.Round(raidedGang.Wealth * Config.RaidCap / 2, 2))
            {
                ReplyError($"You are overkilling it. You only need {(raidedGang.Wealth * Config.RaidCap / 2).USD()} " +
                           $"to steal {Config.RaidCap.ToString("P")} of their cash, that is {(raidedGang.Wealth * Config.RaidCap).USD()}.");
            }

            var stolen = resources * 2;

            int roll = CryptoRandom.Roll();
            var membersDeduction = raidedGang.Members.Length * 5;

            if (Config.RaidOdds - membersDeduction > roll)
            {
                await _gangRepo.ModifyGangAsync(gangName, Context.Guild.Id, x => x.Wealth = raidedGang.Wealth - stolen);
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Wealth = Context.Gang.Wealth + stolen);

                await raidedGang.LeaderId.TryDMAsync(Context.Client, $"{Context.Gang.Name} just raided your gang's wealth and managed to walk away with {stolen.USD()}.");

                await ReplyAsync($"With a {Config.RaidOdds}.00% chance of success, you successfully stole {stolen.USD()}. " +
                                 $"{Context.Gang.Name}'s Wealth {Context.Gang.Wealth.USD()}.");
            }
            else
            {
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Wealth = Context.Gang.Wealth - resources);

                await raidedGang.LeaderId.TryDMAsync(Context.Client, $"{Context.Gang.Name} tried to raid your gang's stash, but one of your loyal sicarios gunned them out.");

                await ReplyAsync($"With a {Config.RaidOdds}.00% chance of success, you failed to steal {stolen.USD()} " +
                                 $"and lost all resources in the process.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Raid", Config.RaidCooldown));
        }
    }
}
