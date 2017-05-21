using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("Deposit")]
        [Require(Attributes.InGang)]
        [Remarks("50")]
        [Summary("Deposit cash into your gang's funds.")]
        public async Task Deposit(decimal cash)
        {
            if (cash < Config.MIN_DEPOSIT)
            {
                ReplyError($"The lowest deposit is {Config.MIN_DEPOSIT.USD()}.");
            }
            else if (Context.Cash < cash)
            {
                ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
            }

            await _userRepo.EditCashAsync(Context, -cash);
            await _gangRepo.ModifyAsync(Context.Gang, x => x.Wealth = Context.Gang.Wealth + cash);

            await ReplyAsync($"You have successfully deposited {cash.USD()}. " +
                        $"{Context.Gang.Name}'s Wealth: {Context.Gang.Wealth.USD()}");

            await Context.Gang.LeaderId.DMAsync(Context.Client, $"{Context.User.Boldify()} deposited {cash.USD()} into your gang's wealth.");
        }
    }
}
