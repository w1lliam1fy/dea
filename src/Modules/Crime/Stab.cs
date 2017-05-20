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
        [Command("Stab")]
        [Cooldown]
        [Summary("Attempt to stab a user.")]
        public async Task Stab(IGuildUser userToStab)
        {
            var userItemData = _gameService.InventoryData(Context.DbUser);

            if (userToStab.Id == Context.User.Id)
            {
                ReplyError("Hey, look at that retard! He's trying to stab himself lmfao.");
            }
            else if (!userItemData.Any(x => x.ItemType == "Knife"))
            {
                ReplyError("You must have a knife to stab someone.");
            }

            var dbUser = await _userRepo.GetUserAsync(userToStab);
            var sorted = userItemData.OrderByDescending(x => x.Damage);
            var strongestWeapon = sorted.First(x => x.ItemType == "Knife");

            if (Config.RAND.Next(1, 101) < strongestWeapon.Accuracy)
            {
                var userDM = await userToStab.CreateDMChannelAsync();
                var damage = dbUser.Inventory.Contains("Kevlar") ? (int)(strongestWeapon.Damage * 0.8) : strongestWeapon.Damage;

                await _userRepo.ModifyAsync(dbUser, x => x.Health -= damage);

                if (dbUser.Health <= 0)
                {
                    foreach (var item in dbUser.Inventory.Elements)
                    {
                        await _gameService.ModifyInventoryAsync(Context.DbUser, item.Name);
                    }

                    await _userRepo.DeleteAsync(x => x.Id == dbUser.Id);
                    await userToStab.DMAsync($"Unfortunately, you were killed by {Context.User.Boldify()}. All your data has been reset.");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);
                    await ReplyAsync($"Woah, you just killed {userToStab.Boldify()}. You just earned {dbUser.Bounty.USD()} **AND** their inventory, congrats.");
                }
                else
                {
                    await userToStab.DMAsync($"{Context.User} tried to kill you, but nigga *AH, HA, HA, HA, STAYIN' ALIVE*. -{damage} health. Current Health: {dbUser.Health}");
                    await ReplyAsync($"Just stabbed that nigga in the heart, you just dealt {damage} damage to {userToStab.Boldify()}.");
                }
            }
            else
            {
                await ReplyAsync($"This nigga actually did some acrobatics shit and bounced out of the way before you stabbed him.");
            }
            _rateLimitService.Add(Context.User.Id, Context.Guild.Id, "Stab", Config.STAB_COOLDOWN);
        }
    }
}