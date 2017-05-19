using Discord.Commands;
using System.Threading.Tasks;
using DEA.Common.Preconditions;

namespace DEA.Modules.Trivia
{
    public partial class Trivia
    {
        [Command("ChangeAutoTriviaSettings")]
        [Alias("EnableAutoTrivia", "DisableAutoTrivia")]
        [Require(Attributes.Admin)]
        [Summary("Enables/disables the auto trivia feature: Sends a trivia question in the default text channel every 2 minutes.")]
        public async Task ChangeAutoTriviaSettings()
        {
            if (Context.DbGuild.AutoTrivia)
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.AutoTrivia = false);
                await ReplyAsync($"You have successfully disabled auto trivia!");
            }
            else
            {
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.AutoTrivia = true);
                await ReplyAsync($"You have successfully enabled auto trivia!");
            }
        }
    }
}
