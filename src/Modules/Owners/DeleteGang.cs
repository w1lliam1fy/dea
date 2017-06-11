using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("DeleteGang")]
        [Alias("ResetGang")]
        [Remarks("Blood")]
        [Summary("Deletes a gang.")]
        public async Task DeleteGang([Remainder] string gangName)
        {
            var gang = await _gangRepo.GetGangAsync(gangName, Context.Guild.Id);

            if (gang == null)
            {
                ReplyError("This gang does not exist.");
            }
            else
            {
                await _gangRepo.DeleteAsync(gang);
                await SendAsync($"You have successfully deleted {gang.Name}.");
            }
        }
    }
}
