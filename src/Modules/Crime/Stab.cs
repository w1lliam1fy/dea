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
        [Cooldown(1, 1, Scale.Hours)]
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
                    await _userRepo.Collection.DeleteOneAsync(x => x.UserId == dbUser.UserId && x.GuildId == dbUser.GuildId);
                    await userDM.SendAsync($"Unfortunately, you were killed by {Context.User.Boldify()}. All your data has been reset.");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);
                    await ReplyAsync($"Woah, you just killed {userToStab.Boldify()}. You just earned {dbUser.Bounty.USD()}, congrats.");
                }
                else
                {
                    await userDM.SendAsync($"{Context.User} tried to kill you. Unfortunately, it barely missed any organs. -{damage} health. Current Health: {dbUser.Health}");

                    await _userRepo.EditCashAsync(Context, dbUser.Bounty);

                    await ReplyAsync($"Just stabbed that nigga in the heart, you just dealt {damage} damage to {userToStab.Boldify()}.");
                }
            }
            else
            {
                await ReplyAsync($"This nigga actually did some acrobatics shit and bounced out of the way before you stabbed him.");
            }
        }
    }
}