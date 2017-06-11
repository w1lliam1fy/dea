using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Extensions;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Donate")]
        [Alias("Sauce")]
        [Remarks("\"Sexy John#0007\" 10000.01")]
        [Summary("Sauce some cash to one of your mates.")]
        public async Task Donate(IGuildUser user, decimal money)
        {
            if (user.Id == Context.User.Id)
            {
                ReplyError("Hey kids! Look at that retard, he is trying to give money to himself!");
            }
            else if (money < Config.MinDonate)
            {
                ReplyError($"Lowest donation is {Config.MinDonate}$.");
            }
            else if (Context.Cash < money)
            {
                ReplyError($"You do not have enough money. Balance: {Context.Cash.USD()}.");
            }

            await _userRepo.EditCashAsync(Context, -money);
            decimal deaMoney = money * Config.DeaCut;

            var otherDbUser = await _userRepo.GetUserAsync(user);
            await _userRepo.EditCashAsync(user, Context.DbGuild, otherDbUser, money - deaMoney);

            await ReplyAsync($"Successfully donated {(money - deaMoney).USD()} to {user.Boldify()}.\nDEA has taken a {deaMoney.USD()} cut out of this donation. Balance: {Context.Cash.USD()}.");
        }
    }
}
