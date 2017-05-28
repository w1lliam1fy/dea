using DEA.Common.Extensions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Suicide")]
        [Alias("kms", "commitsuicide")]
        [Summary("Kill yourself.")]
        public async Task Suicide()
        {
            if (Context.Cash < Config.SUICIDE_COST)
            {
                ReplyError($"You need {Config.SUICIDE_COST.USD()} to be able to buy a high quality noose. Balance: {Context.Cash.USD()}.");
            }
            
            if (Context.DbUser.SlaveOf != 0)
            {
                var dbUser = await _userRepo.GetUserAsync(Context.DbUser.SlaveOf, Context.Guild.Id);

                foreach(var item in Context.DbUser.Inventory.Elements)
                {
                    await _gameService.ModifyInventoryAsync(dbUser, item.Name);
                }
            }
            await _userRepo.DeleteAsync(Context.DbUser);

            await ReplyAsync($"You have successfully killed yourself.");
        }
    }
}
