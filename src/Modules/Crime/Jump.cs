using DEA.Common.Extensions;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Services.Static;
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
            if (CryptoRandom.Roll() > Config.JumpOdds)
            {
                await _userRepo.EditCashAsync(Context, -Config.JumpFine);

                await ReplyAsync($"Turns out the nigga was a black belt, whooped your ass, and brought you in. " +
                                 $"Court's final ruling was a {Config.JumpFine.USD()} fine. Balance: {Context.Cash.USD()}.");
            }
            else
            {
                decimal moneyJumped = CryptoRandom.NextDecimal(Config.MinJump, Config.MaxJump);
                await _userRepo.EditCashAsync(Context, moneyJumped);

                await ReplyAsync($"You jump some random nigga on the streets and manage to get {moneyJumped.USD()}. Balance: {Context.Cash.USD()}.");
            }
            _cooldownService.TryAdd(new CommandCooldown(Context.User.Id, Context.Guild.Id, "Jump", Config.JumpCooldown));
        }
    }
}
