using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Extensions;
using DEA.Common.Items;
using DEA.Common.Preconditions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Trade")]
        [Remarks("\"Sexy John#0007\" 1 \"Bear Grylls Meat\" 1 Gold Crate")]
        [Summary("Request to trade with any user.")]
        public async Task Trade(IGuildUser userToTrade, int exchangeItemQuantity, [Own] Item itemInExchange, int requestedItemQuantity, Item requestedItem)
        {
            if (userToTrade.Id == Context.User.Id)
            {
                ReplyError("It takes great skill and concetration to actually reach full retard by trading with yourself. You are not quite there.");
            }

            var userDM = await userToTrade.CreateDMChannelAsync();
            var key = Config.RAND.Next();
            var firstS = exchangeItemQuantity == 1 ? string.Empty : "s";
            var secondS = requestedItemQuantity == 1 ? string.Empty : "s";

            await userDM.SendAsync($"**Offer:** {exchangeItemQuantity} {itemInExchange.Name}{firstS}\n" +
                                   $"**Request:** {requestedItemQuantity} {requestedItem.Name}{secondS}\n\n" +
                                   $"Please respond with \"{key}\" within 5 minutes to accept this trade.",
                                   $"Trade Request from {Context.User}");

            await ReplyAsync($"You have successfully informed {userToTrade.Boldify()} of your trade request.");

            var answer = await _interactiveService.WaitForMessage(userDM, x => x.Content == key.ToString(), TimeSpan.FromMinutes(5));

            if (answer != null)
            {
                var newOffererDbuser = await _userRepo.GetUserAsync(Context.GUser);
                var newRequesterDbuser = await _userRepo.GetUserAsync(userToTrade);

                if (!newOffererDbuser.Inventory.Contains(itemInExchange.Name))
                {
                    await userDM.SendErrorAsync($"{Context.User.Boldify()} does not own the following item: {itemInExchange.Name}.");
                }
                else if (!newRequesterDbuser.Inventory.Contains(requestedItem.Name))
                {
                    await userDM.SendErrorAsync($"You do not own the following item: {requestedItem.Name}.");
                }
                else if (!(newOffererDbuser.Inventory[itemInExchange.Name].AsInt32 >= exchangeItemQuantity))
                {
                    await userDM.SendErrorAsync($"{Context.User.Boldify()} does not own {exchangeItemQuantity} {itemInExchange.Name}{firstS}.");
                }
                else if (!(newRequesterDbuser.Inventory[requestedItem.Name].AsInt32 >= requestedItemQuantity))
                {
                    await userDM.SendErrorAsync($"You do not own {requestedItemQuantity} {requestedItem.Name}{secondS}.");
                }
                else
                {
                    await _gameService.ModifyInventoryAsync(newOffererDbuser, itemInExchange.Name, -exchangeItemQuantity);
                    await _gameService.ModifyInventoryAsync(newOffererDbuser, requestedItem.Name, requestedItemQuantity);

                    await _gameService.ModifyInventoryAsync(newRequesterDbuser, itemInExchange.Name, exchangeItemQuantity);
                    await _gameService.ModifyInventoryAsync(newRequesterDbuser, requestedItem.Name, -requestedItemQuantity);

                    var message = $"**Offer:** {exchangeItemQuantity} {itemInExchange.Name}{firstS}\n" +
                                  $"**Request:** {requestedItemQuantity} {requestedItem.Name}{secondS}\n\n";

                    await userDM.SendAsync(message, $"Completed Trade with {Context.User}");
                    await Context.GUser.TryDMAsync(message, $"Completed Trade with {userToTrade}");
                }
            }
        }
    }
}
