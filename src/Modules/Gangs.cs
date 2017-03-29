using Discord;
using Discord.Commands;
using Discord.Addons.InteractiveCommands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Repository;
using System.Linq;
using DEA.SQLite.Models;

namespace DEA.Modules
{
    public class Gangs : InteractiveModuleBase<SocketCommandContext>
    {

        [Command("CreateGang")]
        [RequireNoGang]
        [Summary("Allows you to create a gang at a hefty price.")]
        [Remarks("Create <Name>")]
        public async Task ResetCooldowns([Remainder] string name)
        {
            var user = UserRepository.FetchUser(Context);
            if (user.Cash < Config.GANG_CREATION_COST)
                throw new Exception($"You do not have {Config.GANG_CREATION_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
            var gang = GangRepository.CreateGang(Context.User.Id, Context.Guild.Id, name);
            await UserRepository.EditCashAsync(Context, -Config.GANG_CREATION_COST);
            await ReplyAsync($"{Context.User.Mention}, You have successfully created the {gang.Name} gang!");
        }

        [Command("AddGangMember")]
        [RequireGangLeader]
        [Summary("Allows you to add a member to your gang.")]
        [Remarks("AddGangMember <@GangMember>")]
        public async Task AddToGang(IGuildUser user)
        {
            if (GangRepository.InGang(user.Id, Context.Guild.Id)) throw new Exception("This user is already in a gang.");
            if (GangRepository.IsFull(Context.User.Id, Context.Guild.Id)) throw new Exception("Your gang is already full!");
            GangRepository.AddMember(Context.User.Id, Context.Guild.Id, user.Id);
            await ReplyAsync($"{user} is now a new member of your gang!");
            var channel = await user.CreateDMChannelAsync();
            await channel.SendMessageAsync($"Congrats! You are now a member of {GangRepository.FetchGang(Context).Name}!");
        }

        /*[Command("JoinGang", RunMode = RunMode.Async)]
        [RequireNoGang]
        [Summary("Allows you to request to join a gang.")]
        [Remarks("JoinGang <@GangMember>")]
        private async Task JoinGang(IGuildUser user)
        {
            if (!GangRepository.InGang(user.Id, Context.Guild.Id)) throw new Exception("This user is not in a gang.");
            if (GangRepository.IsFull(user.Id, Context.Guild.Id)) throw new Exception("This gang is already full!");
            var gang = GangRepository.FetchGang(user.Id, user.GuildId);
            var channel = await user.CreateDMChannelAsync();
            await channel.SendMessageAsync($"{Context.User} has requested to join your gang. Reply with \"agree\" within the next 30 seconds to accept this request.");
            await ReplyAsync($"{Context.User.Mention}, The leader of {gang.Name} has been successfully informed of your request to join.");
            var response = await WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));
            await ReplyAsync($"{response.Content}");
            if (response.Content.ToLower() == "agree")
            {
                GangRepository.AddMember(gang.LeaderId, user.GuildId, Context.User.Id);
                await channel.SendMessageAsync($"{Context.User} is now a new member of your gang!");
                var informingChannel = await Context.User.CreateDMChannelAsync();
                await informingChannel.SendMessageAsync($"{Context.User.Mention}, Congrats! You are now a member of {gang.Name}!");
            }
        }*/

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        [Remarks("Gang [Gang name]")]
        public async Task Gang([Remainder] string gangName = null)
        {
            if (gangName == null && !(GangRepository.InGang(Context.User.Id, Context.Guild.Id))) throw new Exception($"You are not in a gang.");
            Gang gang;
            if (gangName == null) gang = GangRepository.FetchGang(Context);
            else gang = GangRepository.FetchGang(gangName, Context.Guild.Id);
            var members = "";
            var leader = "";
            if (Context.Client.GetUser(gang.LeaderId) != null) leader = $"<@{gang.LeaderId}";
            foreach (var member in gang.Members)
                if (Context.Client.GetUser(member) != null) members += $"<@{member}>, ";
            var InterestRate = 0.025f + ((gang.Wealth / 100) * .000075f);
            if (InterestRate > 0.1) InterestRate = 0.1f;
            var builder = new EmbedBuilder()
            {
                Title = gang.Name,
                Color = new Color(0x00AE86),
                Description = $"__**Leader:**__ {leader}\n" +
                              $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n" +
                              $"__**Wealth:**__ {gang.Wealth.ToString("C2")}\n" +
                              $"__**Interest rate:**__ {InterestRate.ToString("P")}"
            };
            await ReplyAsync("", embed: builder);
        }

        [Command("Gangs")]
        [Alias("ganglb")]
        [Summary("Shows the wealthiest gangs.")]
        [Remarks("Gangs")]
        public async Task Ganglb()
        {
            var gangs = GangRepository.FetchAll(Context.Guild.Id).OrderByDescending(x => x.Wealth).ToList();
            string message = "```asciidoc\n= The Wealthiest Gangs =\n";
            int longest = 0;

            for (int i = 0; i < gangs.Count(); i++)
            {
                if (i + 1 >= Config.GANGSLB_CAP) break;
                if (gangs[i].Name.Length > longest) longest = $"{i + 1}. {gangs[i].Name}".Length;
            }

            for (int i = 0; i < gangs.Count(); i++)
            {
                if (i + 1 >= Config.GANGSLB_CAP) break;
                message += $"{i + 1}. {gangs[i].Name}".PadRight(longest + 2) + $" :: {gangs[i].Wealth.ToString("C2")}\n";
            }

            await ReplyAsync($"{message}```");
        }

        [Command("LeaveGang")]
        [RequireInGang]
        [Summary("Allows you to break all ties with a gang.")]
        [Remarks("LeaveGang")]
        public async Task LeaveGang()
        {
            var gang = GangRepository.FetchGang(Context);
            var prefix = GuildRepository.FetchGuild(Context.Guild.Id).Prefix;
            if (gang.LeaderId == Context.User.Id)
                throw new Exception($"You may not leave a gang if you are the owner. Either destroy the gang with the `{prefix}DestroyGang` command, or " +
                                    $"transfer the ownership of the gang to another member with the `{prefix}TransferLeadership` command.");
            GangRepository.RemoveMember(Context.User.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully left {gang.Name}");
            var channel = await Context.Client.GetUser(gang.LeaderId).CreateDMChannelAsync();
            await channel.SendMessageAsync($"{Context.User} has left {gang.Name}.");
        }

        [Command("KickGangMember")]
        [RequireGangLeader]
        [Summary("Kicks a user from your gang.")]
        [Remarks("KickGangMember")]
        public async Task KickFromGang(IGuildUser user)
        {
            if (!GangRepository.IsMemberOf(Context.User.Id, Context.Guild.Id, user.Id)) throw new Exception("This user is not a member of your gang!");
            var gang = GangRepository.FetchGang(Context);
            GangRepository.RemoveMember(user.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully kicked {user} from {gang.Name}");
            var channel = await user.CreateDMChannelAsync();
            await channel.SendMessageAsync($"You have been kicked from {gang.Name}.");
        }

        [Command("DestroyGang")]
        [RequireGangLeader]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        [Remarks("DestroyGang")]
        public async Task DestroyGang()
        {
            var gang = GangRepository.DestroyGang(Context.User.Id, Context.Guild.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully destroyed {gang.Name}.");
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [RequireGangLeader]
        [Summary("Changes the name of your gang.")]
        [Remarks("ChangeGangName <New name>")]
        public async Task ChangeGangName([Remainder] string name)
        {
            var user = UserRepository.FetchUser(Context);
            if (user.Cash < Config.GANG_NAME_CHANGE_COST)
                throw new Exception($"You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
            if (GangRepository.FetchAll(Context.Guild.Id).Any(x => x.Name == name)) throw new Exception($"There is already a gang by the name {name}.");
            await UserRepository.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
            GangRepository.Modify(x => x.Name = name, Context.User.Id, Context.Guild.Id);
            await ReplyAsync($"You have successfully changed your gang name to {name} at the cost of {Config.GANG_NAME_CHANGE_COST.ToString("C2")}.");
        }

        [Command("TransferLeadership")]
        [RequireGangLeader]
        [Summary("Transfers the leadership of your gang to another member.")]
        [Remarks("TransferLeadership <@GangMember>")]
        public async Task TransferLeadership(IGuildUser user)
        {
            if (user.Id == Context.User.Id) throw new Exception("You are already the leader of this gang!");
            var gang = GangRepository.FetchGang(Context);
            if (!GangRepository.IsMemberOf(Context.User.Id, Context.Guild.Id, user.Id)) throw new Exception("This user is not a member of your gang!");
            GangRepository.RemoveMember(Context.User.Id, Context.Guild.Id);
            GangRepository.Modify(x => x.LeaderId = user.Id, Context);
            GangRepository.AddMember(Context.User.Id, Context.Guild.Id, Context.User.Id);
            await ReplyAsync($"{Context.User.Mention}, You have successfully transferred the leadership of {gang.Name} to {user.Mention}");
        }

        [Command("Deposit")]
        [RequireInGang]
        [Summary("Deposit cash into your gang's funds.")]
        [Remarks("Deposit <Cash>")]
        public async Task Deposit(double cash)
        {
            var user = UserRepository.FetchUser(Context);
            if (cash < Config.MIN_DEPOSIT) throw new Exception($"The lowest deposit is {Config.MIN_DEPOSIT.ToString("C2")}.");
            if (user.Cash < cash) throw new Exception($"You do not have enough money. Balance: {user.Cash.ToString("C2")}.");
            await UserRepository.EditCashAsync(Context, -cash);
            GangRepository.Modify(x => x.Wealth += cash, Context.User.Id, Context.Guild.Id);
            var gang = GangRepository.FetchGang(Context);
            await ReplyAsync($"{Context.User.Mention}, You have successfully deposited {cash.ToString("C2")}. " +
                             $"{gang.Name}'s Wealth: {gang.Wealth.ToString("C2")}");
        }

        [Command("Withdraw")]
        [RequireInGang]
        [RequireCooldown]
        [Summary("Withdraw cash from your gang's funds.")]
        [Remarks("Withdraw <Cash>")]
        public async Task Withdraw(double cash)
        {
            var gang = GangRepository.FetchGang(Context);
            var user = UserRepository.FetchUser(Context);
            if (cash < Config.MIN_WITHDRAW) throw new Exception($"The minimum withdrawal is {Config.MIN_WITHDRAW.ToString("C2")}.");
            if (cash > gang.Wealth * Config.WITHDRAW_CAP)
                throw new Exception($"You may only withdraw {Config.WITHDRAW_CAP.ToString("P")} of your gang's wealth, " +
                                    $"that is {(gang.Wealth * Config.WITHDRAW_CAP).ToString("C2")}.");
            UserRepository.Modify(x => x.Cooldowns.Withdraw = DateTimeOffset.Now, Context);
            GangRepository.Modify(x => x.Wealth -= cash, Context.User.Id, Context.Guild.Id);
            await UserRepository.EditCashAsync(Context, +cash);
            await ReplyAsync($"{Context.User.Mention}, You have successfully withdrawn {cash.ToString("C2")}. " +
                             $"{gang.Name}'s Wealth: {gang.Wealth.ToString("C2")}");
        }

    }
}
