using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("LeaveGang")]
        [Require(Attributes.InGang)]
        [Summary("Allows you to break all ties with a gang.")]
        public async Task LeaveGang()
        {
            if (Context.Gang.LeaderId == Context.User.Id)
            {
                ReplyError($"You may not leave a gang if you are the owner. You may destroy the gang with the `{Context.Prefix}DestroyGang` command.");
            }

            await _gangRepo.RemoveMemberAsync(Context.Gang, Context.User.Id);
            await ReplyAsync($"You have successfully left {Context.Gang.Name}.");

            await Context.Gang.LeaderId.TryDMAsync(Context.Client, $"{Context.User.Boldify()} has left {Context.Gang.Name}.");
        }
    }
}