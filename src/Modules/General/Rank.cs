using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Rank")]
        [Alias("Info")]
        [Summary("View the detailed ranking information of any user.")]
        public async Task Rank([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            var users = await (await _userRepo.Collection.FindAsync(y => y.GuildId == Context.Guild.Id)).ToListAsync();
            var sorted = users.OrderByDescending(x => x.Cash).ToList();
            var slaveOwner = await (Context.Guild as IGuild).GetUserAsync(dbUser.SlaveOf);

            IRole rank = await _rankHandler.GetRankAsync(Context, dbUser);
            var description = $"Balance: {dbUser.Cash.USD()}\n" +
                              $"Health: {dbUser.Health}\n" +
                              $"Position: #{sorted.FindIndex(x => x.UserId == user.Id) + 1}\n" +
                              (rank == null ? string.Empty : $"Rank: {rank.Mention}\n") +
                              (slaveOwner == null ? string.Empty : $"Slave Owner: {slaveOwner.Boldify()}\n") +
                              (dbUser.Bounty == 0 ? string.Empty : $"Bounty: {dbUser.Bounty.USD()}");

            await SendAsync(description, $"Ranking of {user}");
        }
    }
}
