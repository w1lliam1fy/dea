using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Bully")]
        [Require(Attributes.Bully)]
        [Remarks("\"Sexy John#0007\" Retard LOL")]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(IGuildUser userToBully, [Remainder] string nickname)
        {
            if (nickname.Length > 32)
            {
                ReplyError("The length of a nickname may not be longer than 32 characters.");
            }
            else if (_moderationService.GetPermLevel(Context.DbGuild, userToBully) > 0)
            {
                ReplyError("You may not bully a moderator.");
            }
            else if ((await _userRepo.GetUserAsync(userToBully)).Cash >= Context.Cash)
            {
                ReplyError("You may not bully a user with more money than you.");
            }

            await userToBully.ModifyAsync(x => x.Nickname = nickname);
            await SendAsync($"{userToBully.Boldify()} just got ***BULLIED*** by {Context.User.Boldify()} with his new nickname: \"{nickname}\".");
        }
    }
}
