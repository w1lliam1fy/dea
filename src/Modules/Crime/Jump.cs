using DEA.Common.Data;
using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Jump")]
        [Require(Attributes.Jump)]
        [Cooldown]
        [Summary("Jump some random nigga in the hood.")]
        public async Task Jump()
        {
            int roll = Config.RAND.Next(1, 101);
            if (roll > Config.JUMP_ODDS)
            {
                await _userRepo.EditCashAsync(Context, -Config.JUMP_FINE);

                await ReplyAsync($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                            $"Court's final ruling was a {Config.JUMP_FINE.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyJumped = (Config.RAND.Next((int)(Config.MIN_JUMP) * 100, (int)(Config.MAX_JUMP) * 100)) / 100m;
                await _userRepo.EditCashAsync(Context, moneyJumped);

                await ReplyAsync($"You jump some random nigga on the streets and manage to get {moneyJumped.USD()}. Balance: {Context.Cash.USD()}.");
            }
            _rateLimitService.Add(Context.User.Id, Context.Guild.Id, "Jump", Config.JUMP_COOLDOWN);
        }
    }
}
