using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Items;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Enslave")]
        [Cooldown]
        [Remarks("Sexy John#0007 Karambit")]
        [Summary("Enslave any users at low health.")]
        public async Task Enslave(IGuildUser userToEnslave, [Own] [Remainder] Weapon weapon)
        {
            var user = await _userRepo.GetUserAsync(userToEnslave);
            if (userToEnslave.Id == Context.User.Id)
            {
                ReplyError("Hey good buddies, look at that retard, trying to enslave himself.");
            }
            else if (Context.DbUser.SlaveOf != 0)
            {
                ReplyError("You cannot enslave someone if you are a slave.");
            }
            else if (user.SlaveOf != 0)
            {
                ReplyError("This user is already a slave.");
            }
            else if (user.Health > Config.ENSLAVE_HEALTH)
            {
                ReplyError($"The user must be under {Config.ENSLAVE_HEALTH} health to enslave.");
            }

            if (weapon.Accuracy >= Config.Random.Roll())
            {
                await _userRepo.ModifyAsync(user, x => x.SlaveOf = Context.User.Id);
                await ReplyAsync($"You have successfully enslaved {userToEnslave.Boldify()}. {Config.SLAVE_COLLECT_VALUE.ToString("P")} of all cash earned by all your slaves will go straight to you when you use `{Context.DbGuild.Prefix}Collect`.");

                await userToEnslave.TryDMAsync($"AH SHIT NIGGA! Looks like {Context.User.Boldify()} got you enslaved. The only way out is `{Context.DbGuild.Prefix}suicide`.");
            }
            else
            {
                await ReplyAsync("YOU GOT SLAMMED RIGHT IN THE CUNT BY A NIGGA! :joy: :joy: :joy: Only took him 10 seconds to get you to the ground LMFAO.");

                await userToEnslave.TryDMAsync($"{Context.User.Boldify()} tried to enslave you but accidentally got pregnant and now he can't move :joy: :joy: :joy:.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Enslave", Config.ENSLAVE_COOLDOWN));
        }
    }
}
