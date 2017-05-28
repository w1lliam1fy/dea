using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Extensions;

namespace DEA.Modules.Moderation
{
    public partial class Moderation
    {
        [Command("Unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Remarks("\"Sexy John#0007\" We long for his sexiness back")]
        [Summary("Unban a user.")]
        public async Task Unban(string username, [Remainder] string reason = null)
        {
            var guildBans = await Context.Guild.GetBansAsync();

            var match = guildBans.Where(x => x.User.ToString().ToLower().Contains(username.ToLower()));

            var count = match.Count();

            if (count >= 2)
            {
                var matches = string.Empty;
                foreach (var restBan in match)
                {
                    matches += $"{restBan.User}\n";
                }

                ReplyError($"There are multiple matches to your unban request:\n{matches}");
            }
            else if (count == 0)
            {
                ReplyError("You may not unban someone who isn't banned.");
            }

            var user = match.First().User;

            await Context.Guild.RemoveBanAsync(user);

            await SendAsync($"{Context.User.Boldify()} has successfully unbanned {user.Boldify()}.");

            await _moderationService.TryInformSubjectAsync(Context.User, "Unban", user, reason);
            await _moderationService.TryModLogAsync(Context.DbGuild, Context.Guild, "Unban", new Color(0, 255, 0), reason, Context.User, user);
        }
    }
}
