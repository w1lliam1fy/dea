using DEA.Common.Extensions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("AddBounty")]
        [Remarks("\"Sexy John#0007\" 50")]
        [Summary("Add a bounty of any user.")]
        public async Task SetBounty(IGuildUser userToSet, decimal bounty)
        {
            if (bounty < Config.MIN_BOUNTY)
            {
                ReplyError($"You cannot set a bounty less than {Config.MIN_BOUNTY.USD()}.");
            }
            else if (userToSet.Id == Context.User.Id)
            {
                ReplyError("Are you trying to make yourself a target? smh.");
            }
            else if (bounty > Context.Cash)
            {
                ReplyError($"You don't have enough money. Balance: {Context.Cash.USD()}.");
            }

            await _userRepo.ModifyUserAsync(userToSet, x => x.Bounty += bounty);
            await _userRepo.EditCashAsync(Context, -bounty);
            await ReplyAsync($"Successfully added {bounty.USD()} to {userToSet}'s bounty.");
        }
    }
}
