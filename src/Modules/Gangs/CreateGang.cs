using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Gangs
{
    public partial class Gangs
    {
        [Command("CreateGang")]
        [Require(Attributes.NoGang)]
        [Summary("Allows you to create a gang at a hefty price.")]
        public async Task CreateGang([Summary("SLAM EM BOYS")] [Remainder] string name)
        {
            if (Context.Cash < Config.GANG_CREATION_COST)
            {
                ReplyError($"You do not have {Config.GANG_CREATION_COST.USD()}. Balance: {Context.Cash.USD()}.");
            }
            else if (!Config.ALPHANUMERICAL.IsMatch(name))
            {
                ReplyError("Gang names may not contain any non alphanumeric characters.");
            }

            var gang = await _gangRepo.CreateGangAsync(Context, name);
            await _userRepo.EditCashAsync(Context, -Config.GANG_CREATION_COST);

            await ReplyAsync($"You have successfully created the {gang.Name} gang.");
        }
    }
}