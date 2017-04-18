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
using DEA.Services.Static;

namespace DEA.Modules
{
    public class Gangs : DEAModule
    {
        private GangRepository _gangRepo;
        private UserRepository _userRepo;
        private ResponseService _responseService;
        private InteractiveService _interactiveService;
        private IMongoCollection<Gang> _gangs;

        public Gangs(GangRepository gangRepo, UserRepository userRepo, ResponseService responseService, InteractiveService interactiveService, IMongoCollection<Gang> gangs)
        {
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _responseService = responseService;
            _interactiveService = interactiveService;
            _gangs = gangs;
        }

        [Command("CreateGang")]
        [Require(Attributes.NoGang)]
        [Summary("Allows you to create a gang at a hefty price.")]
        public async Task CreateGang([Remainder] string name)
        {
            if (Context.Cash < Config.GANG_CREATION_COST)
                await ErrorAsync($"You do not have {Config.GANG_CREATION_COST.ToString("C", Config.CI)}. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            if (!Config.ALPHANUMERICAL.IsMatch(name)) await ErrorAsync("Gang names may not contain any non alphanumeric characters.");
            var gang = await _gangRepo.CreateGangAsync(Context, name);
            await _userRepo.EditCashAsync(Context, -Config.GANG_CREATION_COST);
            await Reply($"You have successfully created the {gang.Name} gang!");
        }

        [Command("JoinGang")]
        [Require(Attributes.NoGang)]
        [Summary("Sends a request to join a gang.")]
        public async Task AddToGang([Remainder] string gangName)
        {
            var gang = await _gangRepo.FetchGangAsync(gangName, Context.Guild.Id);
            if (gang.Members.Length == 4) await ErrorAsync("This gang is already full!");
            var leader = Context.Guild.GetUser(gang.LeaderId);
            await Reply($"The leader of {gang.Name} has been informed of your request to join their gang.");
            if (leader != null)
            {
                var leaderDM = await leader.CreateDMChannelAsync();
                var key = new Random().Next();
                await leaderDM.SendMessageAsync($"{Context.User} has requested to join your gang. Please respond with \"{key}\" within 5 minutes to add this user to your gang.");
                var answer = await _interactiveService.WaitForMessage(leaderDM, x => x.Content == key.ToString(), TimeSpan.FromMinutes(5));
                if (answer != null)
                {
                    if (await _gangRepo.InGangAsync(Context.User as IGuildUser))
                        await _responseService.Send(leaderDM, "This user has already joined a different gang.");
                    else if ((await _gangRepo.FetchGangAsync(leader)).Members.Length == 4)
                        await _responseService.Send(leaderDM, "Your gang is already full.");
                    else
                    {
                        await _gangRepo.AddMemberAsync(gang, Context.User.Id);
                        await _responseService.Send(leaderDM, $"You have successfully added {Context.User} as a member of your gang.");
                        await DM(Context.User.Id, $"Congrats! {leader} has accepted your request to join {gang.Name}!");
                    }
                }
            }
            else
                await Reply("The leader of that gang is no longer in this server. ***RIP GANG ROFL***");
        }

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        public async Task GangInfo([Remainder] string gangName = null)
        {
            Gang gang;
            if (gangName == null) gang = Context.Gang;
            else gang = await _gangRepo.FetchGangAsync(gangName, Context.Guild.Id);
            if (gang == null && gangName == null) await ErrorAsync("You are not in a gang.");
            var members = string.Empty;
            foreach (var member in gang.Members)
                members += $"<@{member}>, ";
            if (members.Length != 0) members = $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n";
            var description = $"__**Leader:**__ <@{gang.LeaderId}>\n" + members + $"__**Wealth:**__ {gang.Wealth.ToString("C", Config.CI)}\n" +
                              $"__**Interest rate:**__ {InterestRate.Calculate(gang.Wealth).ToString("P")}";
            await Send(description, gang.Name);
        }

        [Command("GangLb")]
        [Alias("gangs")]
        [Summary("Shows the wealthiest gangs.")]
        public async Task Ganglb()
        {
            var gangs = _gangs.Find(y => y.GuildId == Context.Guild.Id).ToList();

            if (gangs.Count == 0) await ErrorAsync("There aren't any gangs yet.");

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
                await ErrorAsync($"You may not leave a gang if you are the owner. Either destroy the gang with the `{Context.Prefix}DestroyGang` command, or " +
                                    $"transfer the ownership of the gang to another member with the `{Context.Prefix}TransferLeadership` command.");
            await _gangRepo.RemoveMemberAsync(Context.Gang, Context.User.Id);
            await Reply($"You have successfully left {Context.Gang.Name}.");
            await DM(Context.Gang.LeaderId, $"{Context.User.Mention} has left {Context.Gang.Name}.");
        }

        [Command("KickGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Kicks a user from your gang.")]
        public async Task KickFromGang([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id)
                await ErrorAsync("You may not kick yourself!");
            if (!await _gangRepo.IsMemberOfAsync(Context.Gang, gangMember.Id))
                await ErrorAsync("This user is not a member of your gang!");
            await _gangRepo.RemoveMemberAsync(Context.Gang, gangMember.Id);
            await Reply($"You have successfully kicked {gangMember} from {Context.Gang.Name}.");
            await DM(gangMember.Id, $"You have been kicked from {Context.Gang.Name}.");
        }

        [Command("DestroyGang")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        public async Task DestroyGang()
        {
            await _gangRepo.DestroyGangAsync(Context.User as IGuildUser);
            await Reply($"You have successfully destroyed your gang.");
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Changes the name of your gang.")]
        public async Task ChangeGangName([Remainder] string newName)
        {
            if (Context.Cash < Config.GANG_NAME_CHANGE_COST)
                await ErrorAsync($"You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            var gangs = _gangs.Find(y => y.GuildId == Context.Guild.Id).ToList();
            if (gangs.Any(x => x.Name.ToLower() == newName.ToLower())) await ErrorAsync($"There is already a gang by the name {newName}.");
            if (!Config.ALPHANUMERICAL.IsMatch(newName)) await ErrorAsync("Gang names may not contain any non alphanumeric characters.");
            await _userRepo.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
            await _gangRepo.ModifyAsync(Context, x => x.Name, newName);
            await Reply($"You have successfully changed your gang name to {newName} at the cost of {Config.GANG_NAME_CHANGE_COST.ToString("C", Config.CI)}.");
        }

        [Command("TransferLeadership")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Summary("Transfers the leadership of your gang to another member.")]
        public async Task TransferLeadership([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id) await ErrorAsync("You are already the leader of this gang!");
            if (!await _gangRepo.IsMemberOfAsync(Context.Gang, gangMember.Id)) await ErrorAsync("This user is not a member of your gang!");
            await _gangRepo.RemoveMemberAsync(Context.Gang, gangMember.Id);
            await _gangRepo.ModifyAsync(Context.User as IGuildUser, x => x.LeaderId, gangMember.Id);
            await _gangRepo.AddMemberAsync(Context.Gang, Context.User.Id);
            await Reply($"You have successfully transferred the leadership of {Context.Gang.Name} to {await NameAsync(gangMember, await _userRepo.FetchUserAsync(gangMember))}.");
            await DM(gangMember.Id, $"{Context.User} has trasnferred the ownership of {Context.Gang.Name} to you!");
        }

        [Command("Deposit")]
        [Require(Attributes.InGang)]
        [Summary("Deposit cash into your gang's funds.")]
        public async Task Deposit(decimal cash)
        {
            if (cash < Config.MIN_DEPOSIT) await ErrorAsync($"The lowest deposit is {Config.MIN_DEPOSIT.ToString("C", Config.CI)}.");
            if (Context.Cash < cash) await ErrorAsync($"You do not have enough money. Balance: {Context.Cash.ToString("C", Config.CI)}.");
            await _userRepo.EditCashAsync(Context, -cash);
            await _gangRepo.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth + cash);
            await Reply($"You have successfully deposited {cash.ToString("C", Config.CI)}. " +
                        $"{Context.Gang.Name}'s Wealth: {(Context.Gang.Wealth + cash).ToString("C", Config.CI)}");
            await DM(Context.Gang.LeaderId, $"{Context.User} deposited {cash.ToString("C", Config.CI)} into your gang's wealth.");
        }

        [Command("Withdraw")]
        [Require(Attributes.InGang)]
        [RequireCooldown]
        [Summary("Withdraw cash from your gang's funds.")]
        public async Task Withdraw(decimal cash)
        {
            if (cash < Config.MIN_WITHDRAW) await ErrorAsync($"The minimum withdrawal is {Config.MIN_WITHDRAW.ToString("C", Config.CI)}.");
            if (cash > Context.Gang.Wealth * Config.WITHDRAW_CAP)
                await ErrorAsync($"You may only withdraw {Config.WITHDRAW_CAP.ToString("P")} of your gang's wealth, " +
                                    $"that is {(Context.Gang.Wealth * Config.WITHDRAW_CAP).ToString("C", Config.CI)}.");
            await _userRepo.ModifyAsync(Context, x => x.Withdraw, DateTime.UtcNow);
            await _gangRepo.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth - cash);
            await _userRepo.EditCashAsync(Context, +cash);
            await Reply($"You have successfully withdrawn {cash.ToString("C", Config.CI)}. " +
                        $"{Context.Gang.Name}'s Wealth: {(Context.Gang.Wealth - cash).ToString("C", Config.CI)}.");
            await DM(Context.Gang.LeaderId, $"{Context.User} has withdrawn {cash.ToString("C", Config.CI)} from your gang's wealth.");
        }

        [Command("Raid")]
        [Require(Attributes.InGang)]
        [RequireCooldown]
        [Summary("Raid another gang in attempt to steal some of their wealth.")]
        public async Task Raid(decimal resources, [Remainder] string gangName)
        {
            if (resources < Config.MIN_RESOURCES) await ErrorAsync($"The minimum amount of money to spend on resources for a raid is {Config.MIN_RESOURCES.ToString("C", Config.CI)}.");
            if (Context.Gang.Wealth < resources) await ErrorAsync($"Your gang does not have enough money. {Context.Gang.Name}'s Wealth {Context.Gang.Wealth.ToString("C", Config.CI)}.");

            var raidedGang = await _gangRepo.FetchGangAsync(gangName, Context.Guild.Id);
            if (Math.Round(resources, 2) > Math.Round(raidedGang.Wealth / 20m, 2))
                await ErrorAsync($"You are overkilling it. You only need {(raidedGang.Wealth / 20).ToString("C", Config.CI)} " +
                      $"to steal 10% of their cash, that is {(raidedGang.Wealth / 10).ToString("C", Config.CI)}.");
            var stolen = resources * 2;

            int roll = new Random().Next(1, 101);
            if (Config.RAID_SUCCESS_ODDS > roll)
            {
                await _gangRepo.ModifyAsync(gangName, Context.Guild.Id, x => x.Wealth, raidedGang.Wealth - stolen);
                await _gangRepo.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth + stolen);
                await _gangRepo.ModifyAsync(Context, x => x.Raid, DateTime.UtcNow);

                await DM(raidedGang.LeaderId, $"{Context.Gang.Name} just raided your gang's wealth and managed to walk away with {stolen.ToString("C", Config.CI)}.");

                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you successfully stole {stolen.ToString("C", Config.CI)}. " +
                            $"{Context.Gang.Name}'s Wealth {(Context.Gang.Wealth + stolen).ToString("C", Config.CI)}.");
            }
            else
            {
                await _gangRepo.ModifyAsync(Context, x => x.Wealth, Context.Gang.Wealth - resources);
                await _gangRepo.ModifyAsync(Context, x => x.Raid, DateTime.UtcNow);

                await DM(raidedGang.LeaderId, $"{Context.Gang.Name} tried to raid your gang's stash, but one of your loyal sicarios gunned them out.");

                await Reply($"With a {Config.RAID_SUCCESS_ODDS}.00% chance of success, you failed to steal {stolen.ToString("C", Config.CI)} " +
                            $"and lost all resources in the process.");
            }
        }

    }
}
