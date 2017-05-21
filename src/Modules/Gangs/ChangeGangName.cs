using DEA.Common.Data;
using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;
using MongoDB.Driver;
using DEA.Common.Extensions;
using System.Linq;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [Require(Attributes.InGang, Attributes.GangLeader)]
        [Remarks("JERK EM OFF BOYS")]
        [Summary("Changes the name of your gang.")]
        public async Task ChangeGangName([Remainder] string newName)
        {
            if (Context.Cash < Config.GANG_NAME_CHANGE_COST)
            {
                ReplyError($"You do not have {Config.GANG_NAME_CHANGE_COST.USD()}. Balance: {Context.Cash.USD()}.");
            }

            var gangs = await (await _gangRepo.Collection.FindAsync(y => y.GuildId == Context.Guild.Id)).ToListAsync();

            if (gangs.Any(x => x.Name.ToLower() == newName.ToLower()))
            {
                ReplyError($"There is already a gang by the name {newName}.");
            }
            else if (!Config.ALPHANUMERICAL.IsMatch(newName))
            {
                ReplyError("Gang names may not contain any non alphanumeric characters.");
            }

            await _userRepo.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
            await _gangRepo.ModifyAsync(Context.Gang, x => x.Name = newName);

            await ReplyAsync($"You have successfully changed your gang name to {newName} at the cost of {Config.GANG_NAME_CHANGE_COST.USD()}.");
        }
    }
}
