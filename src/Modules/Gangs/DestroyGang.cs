using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("DestroyGang")]
        [Require(Attributes.GangLeader)]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        public async Task DestroyGang()
        {
            await _gangRepo.DestroyGangAsync(Context.GUser);
            await ReplyAsync($"You have successfully destroyed your gang.");
        }
    }
}