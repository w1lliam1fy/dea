using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using MongoDB.Driver;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Common.Items;
using System.Linq;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Shoot")]
        [Cooldown]
        [Remarks("Sexy John#0007 Revolver")]
        [Summary("Attempt to shoot a user.")]
        public async Task Shoot(IGuildUser userToShoot, [Own] [Remainder] Gun gun)
        {
            if (userToShoot.Id == Context.User.Id)
            {
                ReplyError("Hey, look at that retard! He's trying to shoot himself l0l.");
            }

            var dbUser = await _userRepo.GetUserAsync(userToShoot);

            if (Config.RAND.Next(1, 101) < gun.Accuracy)
            {
                var invData = _gameService.InventoryData(dbUser);
                var damage = invData.Any(x => x is Armour) ? (int)(gun.Damage * 0.8) : gun.Damage;
                //TODO: Rework armour.

                await _userRepo.ModifyAsync(dbUser, x => x.Health -= damage);

                await _gameService.ModifyInventoryAsync(Context.DbUser, "Bullet", -1);

                if (dbUser.Health <= 0)
                {

                    foreach (var item in dbUser.Inventory.Elements)
                    {
                        await _gameService.ModifyInventoryAsync(Context.DbUser, item.Name);
                    }

                    await _userRepo.DeleteAsync(dbUser);
                    await userToShoot.TryDMAsync($"Unfortunately, you were killed by {Context.User.Boldify()}. All your data has been reset.");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);

                    await ReplyAsync($"Woah, you just killed {userToShoot.Boldify()}. You just earned {dbUser.Bounty.USD()} **AND** their inventory, congrats.");
                }
                else
                {
                    await ReplyAsync($"Nice shot, you just dealt {damage} damage to {userToShoot.Boldify()}.");
                    await userToShoot.TryDMAsync($"{Context.User} tried to kill you, but nigga you *AH, HA, HA, HA, STAYIN' ALIVE*. -{damage} health. Current Health: {dbUser.Health}");
                }
            }
            else
            {
                await ReplyAsync($"The nigga fucking dodged the bullet, literally. What in the sac of nuts.");
            }
            await _gameService.ModifyInventoryAsync(Context.DbUser, "Bullet", -1);
            _rateLimitService.TryAdd(new RateLimit(Context.User.Id, Context.Guild.Id, "Shoot", Config.SHOOT_COOLDOWN));
        }
    }
}
