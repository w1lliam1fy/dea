using DEA.Common.Data;
using DEA.Common.Preconditions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Collect")]
        [Summary("Collect a portion from your slaves.")]
        [RequireCooldown]
        [Require(Attributes.SlaveOwner)]
        public async Task Collect()
        {
            var collection = await _userRepo.AllAsync(x => x.SlaveOf == Context.User.Id && x.GuildId == Context.Guild.Id);
            if (collection.Count == 0)
            {
                ReplyError("You are not an owner of any slaves.");
            }

            foreach (var slave in collection)
            {
                await _userRepo.EditCashAsync(Context, slave.Cash * 0.8m);
                await _userRepo.EditCashAsync(await (Context.Guild as IGuild).GetUserAsync(slave.UserId), Context.DbGuild, slave, -slave.Cash * 0.8m);
            }
            await SendAsync($"Successfully collected {Config.SLAVE_COLLECT_VALUE * 100}% of all slave cash.");
        }
    }
}
