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
        [Remarks("SLAM EM BOYS")]
        [Summary("Allows you to create a gang at a hefty price.")]
        public async Task CreateGang([Remainder] string name)
        {
            if (Context.Cash < Config.GangCreationCost)
            {
                ReplyError($"You do not have {Config.GangCreationCost.USD()}. Balance: {Context.Cash.USD()}.");
            }
            else if (!Config.AlphaNumerical.IsMatch(name))
            {
                ReplyError("Gang names may not contain any non alphanumeric characters.");
            }
            else if (await _gangRepo.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == Context.Guild.Id))
            {
                ReplyError($"There is already a gang by the name {name}.");
            }
            else if (name.Length > Config.MaxGangNameChar)
            {
                ReplyError($"The length of a gang name may not be longer than {Config.MaxGangNameChar} characters.");
            }

            await _userRepo.EditCashAsync(Context, -Config.GangCreationCost);
            var gang = await _gangRepo.CreateGangAsync(Context, name);

            await ReplyAsync($"You have successfully created the {gang.Name} gang.");
        }
    }
}
