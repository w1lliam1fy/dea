using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("Raid")]
        [Require(Attributes.InGang)]
        [Cooldown(1, 8, Scale.Hours)]
        [Summary("Raid another gang in attempt to steal some of their wealth.")]
        public async Task Raid(decimal resources, [Summary("SLAM EM BOYS")] [Remainder] string gangName)
        {
            if (resources < Config.MIN_RESOURCES)
            {
                ReplyError($"The minimum amount of money to spend on resources for a raid is {Config.MIN_RESOURCES.USD()}.");
            }
            else if (Context.Gang.Wealth < resources)
            {
                ReplyError($"Your gang does not have enough money. {Context.Gang.Name}'s Wealth {Context.Gang.Wealth.USD()}.");
            }

            var raidedGang = await _gangRepo.GetGangAsync(gangName, Context.Guild.Id);
            if (Math.Round(resources, 2) > Math.Round(raidedGang.Wealth * Config.MAX_RAID_PERCENTAGE / 2, 2))
            {
                ReplyError($"You are overkilling it. You only need {(raidedGang.Wealth * Config.MAX_RAID_PERCENTAGE / 2).USD()} " +
                           $"to steal {Config.MAX_RAID_PERCENTAGE.ToString("P")} of their cash, that is {(raidedGang.Wealth * Config.MAX_RAID_PERCENTAGE).USD()}.");
            }

            var stolen = resources * 2;

            int roll = Config.RAND.Next(1, 101);
            if (Config.RAID_SUCCESS_ODDS > roll)
            {
                await _gangRepo.ModifyGangAsync(gangName, Context.Guild.Id, x => x.Wealth = raidedGang.Wealth - stolen);
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Wealth = Context.Gang.Wealth + stolen);
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Raid = DateTime.UtcNow);

                await raidedGang.LeaderId.DMAsync(Context.Client, $"{Context.Gang.Name} just raided your gang's wealth and managed to walk away with {stolen.USD()}.");

                await ReplyAsync($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.USD()}. " +
                                 $"{Context.Gang.Name}'s Wealth {Context.Gang.Wealth.USD()}.");
            }
            else
            {
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Wealth = Context.Gang.Wealth - resources);
                await _gangRepo.ModifyAsync(Context.Gang, x => x.Raid = DateTime.UtcNow);

                await raidedGang.LeaderId.DMAsync(Context.Client, $"{Context.Gang.Name} tried to raid your gang's stash, but one of your loyal sicarios gunned them out.");

                await ReplyAsync($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.USD()} " +
                                 $"and lost all resources in the process.");
            }
        }
    }
}
