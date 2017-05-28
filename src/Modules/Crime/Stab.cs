using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Common.Items;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Stab")]
        [Cooldown]
        [Remarks("Sexy John#0007 Huntsman Knife")]
        [Summary("Attempt to stab a user.")]
        public async Task Stab(IGuildUser userToStab, [Own] [Remainder] Knife knife)
        {
            if (userToStab.Id == Context.User.Id)
            {
                ReplyError("Hey, look at that retard! He's trying to stab himself lmfao.");
            }

            var dbUser = await _userRepo.GetUserAsync(userToStab);

            if (Config.RAND.Next(1, 101) < knife.Accuracy)
            {
                var invData = _gameService.InventoryData(dbUser);
                var damage = invData.Any(x => x is Armour) ? (int)(knife.Damage * 0.8) : knife.Damage;
                //TODO: Rework armour.

                await _userRepo.ModifyAsync(dbUser, x => x.Health -= damage);

                if (dbUser.Health <= 0)
                {
                    foreach (var item in dbUser.Inventory.Elements)
                    {
                        await _gameService.ModifyInventoryAsync(Context.DbUser, item.Name);
                    }

                    await _userRepo.DeleteAsync(dbUser);
                    await userToStab.TryDMAsync($"Unfortunately, you were killed by {Context.User.Boldify()}. All your data has been reset.");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);
                    await ReplyAsync($"Woah, you just killed {userToStab.Boldify()}. You just earned {dbUser.Bounty.USD()} **AND** their inventory, congrats.");
                }
                else
                {
                    await userToStab.TryDMAsync($"{Context.User} tried to kill you, but nigga *AH, HA, HA, HA, STAYIN' ALIVE*. -{damage} health. Current Health: {dbUser.Health}");
                    await ReplyAsync($"Just stabbed that nigga in the heart, you just dealt {damage} damage to {userToStab.Boldify()}.");
                }
            }
            else
            {
                await ReplyAsync($"This nigga actually did some acrobatics shit and bounced out of the way before you stabbed him.");
            }
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Stab", Config.STAB_COOLDOWN));
        }
    }
}
