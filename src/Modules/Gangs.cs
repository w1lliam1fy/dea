using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Database.Repository;
using System.Linq;
using DEA.Database.Models;
using MongoDB.Driver;
using DEA.Services;
using DEA.Common;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    public class Gangs : DEAModule
    {

        [Command("CreateGang")]
        [Require(Attributes.NoGang)]
        [Summary("Allows you to create a gang at a hefty price.")]
        public async Task ResetCooldowns([Remainder] string name)
        {
            if (Context.Cash < Config.GANG_CREATION_COST)
                Error($"You do not have {Config.GANG_CREATION_COST.ToString("C", Config.CI)}. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            if (!Config.ALPHANUMERICAL.IsMatch(name)) Error("Gang names may not contain any non alphanumeric characters.");
            var gang = await GangRepository.CreateGangAsync(Context, name);
            await UserRepository.EditCashAsync(Context, -Config.GANG_CREATION_COST);
            await Reply($"You have successfully created the {gang.Name} gang!");
        }

        [Command("AddGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Allows you to add a member to your gang.")]
        public async Task AddToGang([Remainder] IGuildUser newMember)
        {
            if (await GangRepository.InGangAsync(newMember)) Error("This user is already in a gang.");
            if (Context.Gang.Members.Length == 4) Error("Your gang is already full!");
            await GangRepository.AddMemberAsync(Context.Gang, newMember.Id);
            await Reply($"{newMember} is now a new member of your gang!");
            var channel = await newMember.CreateDMChannelAsync();
            await ResponseMethods.DM(channel, $"Congrats! You are now a member of {Context.Gang.Name}!");
        }

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        public async Task GangInfo([Remainder] string gangName = null)
        {
            Gang gang;
            if (gangName == null) gang = Context.Gang;
            else gang = await GangRepository.FetchGangAsync(gangName, Context.Guild.Id);
            if (gang == null) Error("You are not in a gang.");
            var members = string.Empty;
            foreach (var member in gang.Members)
                members += $"<@{member}>, ";
            if (members.Length != 0) members = $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n";
            var description = $"__**Leader:**__ <@{gang.LeaderId}>\n" + members + $"__**Wealth:**__ {gang.Wealth.ToString("C", Config.CI)}\n" +
                              $"__**Interest rate:**__ {Services.Math.CalculateIntrestRate(gang.Wealth).ToString("P")}";
            await Send(description, gang.Name);
        }

        [Command("GangLb")]
        [Alias("gangs")]
        [Summary("Shows the wealthiest gangs.")]
        public async Task Ganglb()
        {
            var gangs = DEABot.Gangs.Find(y => y.GuildId == Context.Guild.Id).ToList();

            if (gangs.Count == 0) Error("There aren't any gangs yet.");

            var sortedGangs = gangs.OrderByDescending(x => x.Wealth).ToList();
            string description = string.Empty;

            for (int i = 0; i < sortedGangs.Count(); i++)
            {
                if (i + 1 > Config.GANGSLB_CAP) break;
                description += $"{i + 1}. {sortedGangs[i].Name}: {sortedGangs[i].Wealth.ToString("C", Config.CI)}\n";
            }

            await Send(description, "The Wealthiest Gangs");
        }

        [Command("LeaveGang")]
        [Require(Attributes.InGang)]
        [Summary("Allows you to break all ties with a gang.")]
        public async Task LeaveGang()
        {
            if (Context.Gang.LeaderId == Context.User.Id)
                Error($"You may not leave a gang if you are the owner. Either destroy the gang with the `{Context.Prefix}DestroyGang` command, or " +
                                    $"transfer the ownership of the gang to another member with the `{Context.Prefix}TransferLeadership` command.");
            await GangRepository.RemoveMemberAsync(Context.Gang, Context.User.Id);
            await Reply($"You have successfully left {Context.Gang.Name}.");
            var userToDM = Context.Guild.GetUser(Context.Gang.LeaderId);
            if (userToDM != null)
            {
                var channel = await userToDM.CreateDMChannelAsync();
                await ResponseMethods.DM(channel, $"{Context.User.Mention} has left {Context.Gang.Name}.");
            }
        }

        [Command("KickGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Kicks a user from your gang.")]
        public async Task KickFromGang([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id)
                Error("You may not kick yourself!");
            if (!GangRepository.IsMemberOf(Context.Gang, gangMember.Id))
                Error("This user is not a member of your gang!");
            await GangRepository.RemoveMemberAsync(Context.Gang, gangMember.Id);
            await Reply($"You have successfully kicked {gangMember} from {Context.Gang.Name}.");
            var channel = await gangMember.CreateDMChannelAsync();
            await ResponseMethods.DM(channel, $"You have been kicked from {Context.Gang.Name}.");
        }

        [Command("DestroyGang")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        public async Task DestroyGang()
        {
            await GangRepository.DestroyGangAsync(Context.User as IGuildUser);
            await Reply($"You have successfully destroyed your gang.");
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Changes the name of your gang.")]
        public async Task ChangeGangName([Remainder] string newName)
        {
            if (Context.Cash < Config.GANG_NAME_CHANGE_COST)
                Error($"You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            var gangs = DEABot.Gangs.Find(y => y.GuildId == Context.Guild.Id).ToList();
            if (gangs.Any(x => x.Name.ToLower() == newName.ToLower())) Error($"There is already a gang by the name {newName}.");
            if (!Config.ALPHANUMERICAL.IsMatch(newName)) Error("Gang names may not contain any non alphanumeric characters.");
            await UserRepository.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
            await GangRepository.ModifyAsync(Context, x => x.Name, newName);
            await Reply($"You have successfully changed your gang name to {newName} at the cost of {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}.");
        }

        [Command("TransferLeadership")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Transfers the leadership of your gang to another member.")]
        public async Task TransferLeadership([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id) Error("You are already the leader of this gang!");
            if (!GangRepository.IsMemberOf(Context.Gang, gangMember.Id)) Error("This user is not a member of your gang!");
            await GangRepository.RemoveMemberAsync(Context.Gang, gangMember.Id);
            await GangRepository.ModifyAsync(Context.User as IGuildUser, x => x.LeaderId, gangMember.Id);
            await GangRepository.AddMemberAsync(Context.Gang, Context.User.Id);
            await Reply($"You have successfully transferred the leadership of {Context.Gang.Name} to {ResponseMethods.Name(gangMember, await UserRepository.FetchUserAsync(gangMember))}.");
        }

        [Command("Deposit")]
        [Require(Attributes.InGang)]
        [Summary("Deposit cash into your gang's funds.")]
        public async Task Deposit(decimal cash)
        {
            if (cash < Config.MIN_DEPOSIT) Error($"The lowest deposit is {Config.MIN_DEPOSIT.ToString("C", Config.CI)}.");
            if (Context.Cash < cash) Error($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            await UserRepository.EditCashAsync(Context, -cash);
            await GangRepository.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth + cash);
            await Reply($"You have successfully deposited {cash.ToString("C", Config.CI)}. " +
                        $"{Context.Gang.Name}'s Wealth: {(Context.Gang.Wealth + cash).ToString("C", Config.CI)}");
            var userToDM = Context.Guild.GetUser(Context.Gang.LeaderId);
            if (userToDM != null)
            {
                var channel = await userToDM.CreateDMChannelAsync();
                await ResponseMethods.DM(channel, $"{Context.User} deposited {cash.ToString("C", Config.CI)} into your gang's wealth.");
            }
        }

        [Command("Withdraw")]
        [Require(Attributes.InGang)]
        [RequireCooldown]
        [Summary("Withdraw cash from your gang's funds.")]
        public async Task Withdraw(decimal cash)
        {
            if (cash < Config.MIN_WITHDRAW) Error($"The minimum withdrawal is {Config.MIN_WITHDRAW.ToString("C", Config.CI)}.");
            if (cash > Context.Gang.Wealth * Config.WITHDRAW_CAP)
                Error($"You may only withdraw {Config.WITHDRAW_CAP.ToString("P")} of your gang's wealth, " +
                                    $"that is {(Context.Gang.Wealth * Config.WITHDRAW_CAP).ToString("C", Config.CI)}.");
            await UserRepository.ModifyAsync(Context, x => x.Withdraw, DateTime.UtcNow);
            await GangRepository.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth - cash);
            await UserRepository.EditCashAsync(Context, +cash);
            await Reply($"You have successfully withdrawn {cash.ToString("C", Config.CI)}. " +
                        $"{Context.Gang.Name}'s Wealth: {(Context.Gang.Wealth - cash).ToString("C", Config.CI)}.");
            var userToDM = Context.Guild.GetUser(Context.Gang.LeaderId);
            if (userToDM != null)
            {
                var channel = await userToDM.CreateDMChannelAsync();
                await ResponseMethods.DM(channel, $"{Context.User} has withdrawn {cash.ToString("C", Config.CI)} from your gang's wealth.");
            }
        }

        [Command("Raid")]
        [Require(Attributes.InGang)]
        [RequireCooldown]
        [Summary("Raid another gang in attempt to steal some of their wealth.")]
        public async Task Raid(decimal resources, [Remainder] string gangName)
        {
            if (resources < Config.MIN_RESOURCES) Error($"The minimum amount of money to spend on resources for a raid is {Config.MIN_RESOURCES.ToString("C", Config.CI)}.");
            if (Context.Gang.Wealth < resources) Error($"Your gang does not have enough money. {Context.Gang.Name}'s Wealth {Context.Gang.Wealth.ToString("C", Config.CI)}.");

            var raidedGang = await GangRepository.FetchGangAsync(gangName, Context.Guild.Id);
            if (System.Math.Round(resources, 2) > System.Math.Round(raidedGang.Wealth / 20m, 2))
                Error($"You are overkilling it. You only need {(raidedGang.Wealth / 20).ToString("C", Config.CI)} " +
                      $"to steal 10% of their cash, that is {(raidedGang.Wealth / 10).ToString("C", Config.CI)}.");
            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);
            if (Config.RAID_SUCCESS_ODDS > roll)
            {
                await GangRepository.ModifyAsync(gangName, Context.Guild.Id, x => x.Wealth, raidedGang.Wealth - stolen);
                await GangRepository.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth + stolen);
                await GangRepository.ModifyAsync(Context, x => x.Raid, DateTime.UtcNow);

                var userToDM = Context.Guild.GetUser(raidedGang.LeaderId);
                if (userToDM != null)
                {
                    var channel = await userToDM.CreateDMChannelAsync();
                    await ResponseMethods.DM(channel, $"{Context.Gang.Name} just raided your gang's wealth and managed to walk away with {stolen.ToString("C", Config.CI)}.");
                }

                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.ToString("C", Config.CI)}. " +
                            $"{Context.Gang.Name}'s Wealth {(Context.Gang.Wealth + stolen).ToString("C", Config.CI)}.");
            }
            else
            {
                await GangRepository.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth - resources);
                await GangRepository.ModifyAsync(Context, x => x.Raid, DateTime.UtcNow);

                var userToDM = Context.Guild.GetUser(raidedGang.LeaderId);
                if (userToDM != null)
                {
                    var channel = await userToDM.CreateDMChannelAsync();
                    await ResponseMethods.DM(channel, $"{Context.Gang.Name} tried to raid your gang's stash, but one of your loyal sicarios gunned them out.");
                }
                
                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.ToString("C", Config.CI)} " +
                            $"and lost all resources in the process.");
            }
        }

    }
}
