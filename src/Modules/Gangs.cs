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
using System.Text.RegularExpressions;
using DEA.Common.Preconditions;

namespace DEA.Modules
{
    public class Gangs : DEAModule
    {
        protected override void BeforeExecute()
        {
            InitializeData();
        }

        [Command("CreateGang")]
        [Require(Attributes.NoGang)]
        [Summary("Allows you to create a gang at a hefty price.")]
        public async Task ResetCooldowns([Remainder] string name)
        {
            if (Cash < Config.GANG_CREATION_COST)
                Error($"You do not have {Config.GANG_CREATION_COST.ToString("C", Config.CI)}. Balance: {Cash.ToString("C", Config.CI)}.");
            if (!new Regex(@"^[a-zA-Z0-9\s]*$").IsMatch(name)) Error("Gang names may not contain any non alphanumeric characters.");
            var gang = GangRepository.CreateGang(Context.User.Id, Context.Guild.Id, name);
            await UserRepository.EditCashAsync(Context, -Config.GANG_CREATION_COST);
            await Reply($"You have successfully created the {gang.Name} gang!");
        }

        [Command("AddGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Allows you to add a member to your gang.")]
        public async Task AddToGang([Remainder] IGuildUser newMember)
        {
            if (GangRepository.InGang(newMember.Id, Context.Guild.Id)) Error("This user is already in a gang.");
            if (GangRepository.IsFull(Context.User.Id, Context.Guild.Id)) Error("Your gang is already full!");
            GangRepository.AddMember(Context.User.Id, Context.Guild.Id, newMember.Id);
            await Reply($"{newMember} is now a new member of your gang!");
            var channel = await newMember.CreateDMChannelAsync();
            await ResponseMethods.DM(channel, $"Congrats! You are now a member of {Gang.Name}!");
        }

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        public async Task GangInfo([Remainder] string gangName = null)
        {
            Gang gang;
            if (gangName == null) gang = Gang;
            else gang = GangRepository.FetchGang(gangName, Context.Guild.Id);
            if (gang == null) Error("You are not in a gang.");
            var members = string.Empty;
            var leader = string.Empty;
            if (Context.Guild.GetUser(gang.LeaderId) != null) leader = $"<@{gang.LeaderId}>";
            foreach (var member in gang.Members)
                if (Context.Guild.GetUser(member) != null) members += $"<@{member}>, ";
            if (members.Length != 0) members = $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n";
            var description = $"__**Leader:**__ {leader}\n" + members + $"__**Wealth:**__ {gang.Wealth.ToString("C", Config.CI)}\n" +
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
            if (Gang.LeaderId == Context.User.Id)
                Error($"You may not leave a gang if you are the owner. Either destroy the gang with the `{Prefix}DestroyGang` command, or " +
                                    $"transfer the ownership of the gang to another member with the `{Prefix}TransferLeadership` command.");
            GangRepository.RemoveMember(Context.User.Id, Context.Guild.Id);
            await Reply($"You have successfully left {Gang.Name}.");
            var userToDM = Context.Guild.GetUser(Gang.LeaderId);
            if (userToDM != null)
            {
                var channel = await userToDM.CreateDMChannelAsync();
                await ResponseMethods.DM(channel, $"{Context.User.Mention} has left {Gang.Name}.");
            }
        }

        [Command("KickGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Kicks a user from your gang.")]
        public async Task KickFromGang([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id)
                Error("You may not kick yourself!");
            if (!GangRepository.IsMemberOf(Context.User.Id, Context.Guild.Id, gangMember.Id))
                Error("This user is not a member of your gang!");
            GangRepository.RemoveMember(gangMember.Id, Context.Guild.Id);
            await Reply($"You have successfully kicked {gangMember} from {Gang.Name}");
            var channel = await gangMember.CreateDMChannelAsync();
            await ResponseMethods.DM(channel, $"You have been kicked from {Gang.Name}.");
        }

        [Command("DestroyGang")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        public async Task DestroyGang()
        {
            GangRepository.DestroyGang(Context.User.Id, Context.Guild.Id);
            await Reply($"You have successfully destroyed your gang.");
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Changes the name of your gang.")]
        public async Task ChangeGangName([Remainder] string newName)
        {
            if (Cash < Config.GANG_NAME_CHANGE_COST)
                Error($"You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}. Balance: {Cash.ToString("C", Config.CI)}.");
            var gangs = DEABot.Gangs.Find(y => y.GuildId == Context.Guild.Id).ToList();
            if (gangs.Any(x => x.Name.ToLower() == newName.ToLower())) Error($"There is already a gang by the name {newName}.");
            if (!new Regex(@"^[a-zA-Z0-9\s]*$").IsMatch(newName)) Error("Gang names may not contain any non alphanumeric characters.");
            await UserRepository.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
            GangRepository.Modify(DEABot.GangUpdateBuilder.Set(x => x.Name, newName), Context);
            await Reply($"You have successfully changed your gang name to {newName} at the cost of {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}.");
        }

        [Command("TransferLeadership")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Transfers the leadership of your gang to another member.")]
        public async Task TransferLeadership([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id) Error("You are already the leader of this gang!");
            if (!GangRepository.IsMemberOf(Context.User.Id, Context.Guild.Id, gangMember.Id)) Error("This user is not a member of your gang!");
            for (int i = 0; i < Gang.Members.Length; i++)
                if (Gang.Members[i] == gangMember.Id)
                {
                    Gang.Members[i] = Context.User.Id;
                    GangRepository.Modify(DEABot.GangUpdateBuilder.Combine(
                        DEABot.GangUpdateBuilder.Set(x => x.LeaderId, gangMember.Id),
                        DEABot.GangUpdateBuilder.Set(x => x.Members, Gang.Members)), Context);
                    break;
                }
            await Reply($"You have successfully transferred the leadership of {Gang.Name} to {ResponseMethods.Name(gangMember)}.");
        }

        [Command("Deposit")]
        [Require(Attributes.InGang)]
        [Summary("Deposit cash into your gang's funds.")]
        public async Task Deposit(decimal cash)
        {
            if (cash < Config.MIN_DEPOSIT) Error($"The lowest deposit is {Config.MIN_DEPOSIT.ToString("C", Config.CI)}.");
            if (Cash < cash) Error($"You do not have enough money. Balance: {Cash.ToString("C", Config.CI)}.");
            await UserRepository.EditCashAsync(Context, -cash);
            GangRepository.Modify(DEABot.GangUpdateBuilder.Set(x => x.Wealth, Gang.Wealth + cash), Context);
            await Reply($"You have successfully deposited {cash.ToString("C", Config.CI)}. " +
                        $"{Gang.Name}'s Wealth: {(Gang.Wealth + cash).ToString("C", Config.CI)}");
            var userToDM = Context.Guild.GetUser(Gang.LeaderId);
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
            if (cash > Gang.Wealth * Config.WITHDRAW_CAP)
                Error($"You may only withdraw {Config.WITHDRAW_CAP.ToString("P")} of your gang's wealth, " +
                                    $"that is {(Gang.Wealth * Config.WITHDRAW_CAP).ToString("C", Config.CI)}.");
            UserRepository.Modify(DEABot.UserUpdateBuilder.Set(x => x.Withdraw, DateTime.UtcNow), Context);
            GangRepository.Modify(DEABot.GangUpdateBuilder.Set(x => x.Wealth, Gang.Wealth - cash), Context);
            await UserRepository.EditCashAsync(Context, +cash);
            await Reply($"You have successfully withdrawn {cash.ToString("C", Config.CI)}. " +
                        $"{Gang.Name}'s Wealth: {(Gang.Wealth - cash).ToString("C", Config.CI)}.");
            var userToDM = Context.Guild.GetUser(Gang.LeaderId);
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
            if (Gang.Wealth < resources) Error($"Your gang does not have enough money. {Gang.Name}'s Wealth {Gang.Wealth.ToString("C", Config.CI)}.");

            var raidedGang = GangRepository.FetchGang(gangName, Context.Guild.Id);
            if (System.Math.Round(resources, 2) > System.Math.Round(raidedGang.Wealth / 20m, 2))
                Error($"You are overkilling it. You only need {(raidedGang.Wealth / 20).ToString("C", Config.CI)} " +
                      $"to steal 10% of their cash, that is {(raidedGang.Wealth / 10).ToString("C", Config.CI)}.");
            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);
            if (Config.RAID_SUCCESS_ODDS > roll)
            {
                GangRepository.Modify(DEABot.GangUpdateBuilder.Set(x => x.Wealth, raidedGang.Wealth - stolen), gangName, Context.Guild.Id);
                GangRepository.Modify(DEABot.GangUpdateBuilder.Combine(
                    DEABot.GangUpdateBuilder.Set(x => x.Wealth, Gang.Wealth + stolen),
                    DEABot.GangUpdateBuilder.Set(x => x.Raid, DateTime.UtcNow)), 
                    Context);

                var userToDM = Context.Guild.GetUser(raidedGang.LeaderId);
                if (userToDM != null)
                {
                    var channel = await userToDM.CreateDMChannelAsync();
                    await ResponseMethods.DM(channel, $"{Gang.Name} just raided your gang's wealth and managed to walk away with {stolen.ToString("C", Config.CI)}.");
                }

                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.ToString("C", Config.CI)}. " +
                            $"{Gang.Name}'s Wealth {(Gang.Wealth + stolen).ToString("C", Config.CI)}.");
            }
            else
            {
                GangRepository.Modify(DEABot.GangUpdateBuilder.Combine(
                    DEABot.GangUpdateBuilder.Set(x => x.Wealth, Gang.Wealth - resources),
                    DEABot.GangUpdateBuilder.Set(x => x.Raid, DateTime.UtcNow)),
                    Context);

                var userToDM = Context.Guild.GetUser(raidedGang.LeaderId);
                if (userToDM != null)
                {
                    var channel = await userToDM.CreateDMChannelAsync();
                    await ResponseMethods.DM(channel, $"{Gang.Name} tried to raid your gang's stash, but one of your loyal sicarios gunned them out.");
                }
                
                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.ToString("C", Config.CI)} " +
                            $"and lost all resources in the process.");
            }
        }

    }
}
