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
        [Require(Attributes.GangLeader)]
        [Remarks("JERK EM OFF BOYS")]
        [Summary("Changes the name of your gang.")]
        public async Task ChangeGangName([Remainder] string newName)
        {
            if (Context.Cash < Config.GangNameChangeCost)
            {
                ReplyError($"You do not have {Config.GangNameChangeCost.USD()}. Balance: {Context.Cash.USD()}.");
            }

            var gangs = await _gangRepo.AllAsync();

            if (gangs.Any(x => x.Name.ToLower() == newName.ToLower()))
            {
                ReplyError($"There is already a gang by the name {newName}.");
            }
            else if (!Config.AlphaNumerical.IsMatch(newName))
            {
                ReplyError("Gang names may not contain any non alphanumeric characters.");
            }
            else if (newName.Length > Config.MaxGangNameChar)
            {
                ReplyError($"The length of a gang name may not be longer than {Config.MaxGangNameChar} characters.");
            }

            await _userRepo.EditCashAsync(Context, -Config.GangNameChangeCost);
            await _gangRepo.ModifyAsync(Context.Gang, x => x.Name = newName);

            await ReplyAsync($"You have successfully changed your gang name to {newName} at the cost of {Config.GangNameChangeCost.USD()}.");
        }
    }
}
