using DEA.Common.Extensions;
using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using DEA.Common.Data;
using DEA.Common.Preconditions;

namespace DEA.Modules.Crime
{
    public partial class Crime
    {
        [Command("Shoot")]
        [Cooldown(1, 2, Scale.Hours)]
        [Summary("Attempt to shoot a user.")]
        public async Task Shoot(IGuildUser userToShoot)
        {
            var userItemData = _gameService.InventoryData(Context.DbUser);

            if (userToShoot.Id == Context.User.Id)
            {
                ReplyError("Hey, look at that retard! He's trying to shoot himself l0l.");
            }
            else if (!userItemData.Any(x => x.ItemType == "Gun"))
            {
                ReplyError("You must have a gun to shoot someone.");
            }
            else if (!Context.DbUser.Inventory.Any(x => x.Name == "Bullet"))
            {
                ReplyError("You need bullets to shoot a gun.");
            }

            var dbUser = await _userRepo.GetUserAsync(userToShoot);
            var sorted = userItemData.OrderByDescending(x => x.Damage);
            var strongestWeapon = sorted.First(x => x.ItemType == "Gun");

            if (Config.RAND.Next(1, 101) < strongestWeapon.Accuracy)
            {
                var damage = dbUser.Inventory.Contains("Kevlar") ? (int)(strongestWeapon.Damage * 0.8) : strongestWeapon.Damage;

                await _userRepo.ModifyAsync(dbUser, x => x.Health -= damage);

                await _gameService.ModifyInventoryAsync(Context.DbUser, "Bullet", -1);
                if (dbUser.Health <= 0)
                {
                    await _userRepo.Collection.DeleteOneAsync(x => x.UserId == dbUser.UserId && x.GuildId == dbUser.GuildId);
                    await userToShoot.DMAsync($"Unfortunately, you were killed by {Context.User.Boldify()}. All your data has been reset.");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);
                    await ReplyAsync($"Woah, you just killed {userToShoot.Boldify()}. You just earned {dbUser.Bounty.USD()}, congrats.");
                }
                else
                {
                    

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);

                    await ReplyAsync($"Nice shot, you just dealt {damage} damage to {userToShoot.Boldify()}.");

                    await userToShoot.DMAsync($"{Context.User} tried to kill you, but nigga you dodged that shit ez pz. -{damage} health. Current Health: {dbUser.Health}");
                }
            }
            else
            {
                await ReplyAsync($"The nigga fucking dodged the bullet, literally. What in the sac of nuts.");
            }

            await _gameService.ModifyInventoryAsync(Context.DbUser, "Bullet", -1);
        }
    }
}
