using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("KickGangMember")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Remarks("Sexy John#0007")]
        [Summary("Kicks a user from your gang.")]
        public async Task KickFromGang([Remainder] IGuildUser gangMember)
        {
            if (gangMember.Id == Context.User.Id)
            {
                ReplyError("You may not kick yourself!");
            }
            else if (!_gangRepo.IsMemberOfAsync(Context.Gang, gangMember.Id))
            {
                ReplyError("This user is not a member of your gang!");
            }

            await _gangRepo.RemoveMemberAsync(Context.Gang, gangMember.Id);
            await ReplyAsync($"You have successfully kicked {gangMember.Boldify()} from {Context.Gang.Name}.");

            await gangMember.Id.DMAsync(Context.Client, $"You have been kicked from {Context.Gang.Name}.");
        }
    }
}
