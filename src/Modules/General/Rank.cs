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
        [Alias("Health")]
        [Remarks("Sexy John#0007")]
        [Summary("View the detailed ranking information of any user.")]
        public async Task Rank([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;

            var guildInterface = Context.Guild as IGuild;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);
            var users = await _userRepo.AllAsync(x => x.GuildId == Context.Guild.Id);
            var sorted = users.OrderByDescending(x => x.Cash).ToList();
            var slaveOwner = await guildInterface.GetUserAsync(dbUser.SlaveOf);
            var ownedSlaves = users.Where(x => x.SlaveOf == user.Id && x.GuildId == Context.Guild.Id);

            var slaveInfo = "**Owned Slaves:** ";
            

            foreach (var dbUserSlave in ownedSlaves)
            {
                var slaveUser = await guildInterface.GetUserAsync(dbUserSlave.UserId);

                if (slaveUser == null)
                {
                    continue;
                }

                slaveInfo += $"{slaveUser}, ";
            }

            IRole rank = await _RankHandler.GetRankAsync(Context, dbUser);
            var description = $"**Balance:** {dbUser.Cash.USD()}\n" +
                              $"**Health:** {dbUser.Health}\n" +
                              $"**Position:** #{sorted.FindIndex(x => x.UserId == user.Id) + 1}\n" +
                              (rank == null ? string.Empty : $"**Rank:** {rank.Mention}\n") +
                              (slaveOwner == null ? string.Empty : $"**Slave Owner:** {slaveOwner}\n") +
                              (dbUser.Bounty == 0 ? string.Empty : $"**Bounty:** {dbUser.Bounty.USD()}") +
                              (ownedSlaves.Any() ? slaveInfo.Remove(slaveInfo.Length - 2) : string.Empty);

            await SendAsync(description, $"Ranking of {user}");
        }
    }
}
