using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using DEA.Common.Data;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Extensions;

namespace DEA.Modules.Items
{
    public partial class Items
    {
        [Command("Trade")]
        [Summary("Request to trade with any user.")]
        public async Task Trade(IGuildUser userToTrade, [Summary("2")] int exchangeItemQuantity, [Summary("Gold Crate")] string itemInExchange, [Summary("4")] int requestedItemQuantity, [Remainder] [Summary("Sardine")]string requestedItem)
        {
            if (userToTrade.Id == Context.User.Id)
            {
                ReplyError("It takes great skill and concetration to actually reach full retard by trading with yourself. You are not quite there.");
            }

            var element = _items.FirstOrDefault(x => x.Name.ToLower() == itemInExchange.ToLower());
            var elementFor = _items.FirstOrDefault(x => x.Name.ToLower() == requestedItem.ToLower());

            if (requestedItemQuantity < 1 || exchangeItemQuantity < 1)
            {
                ReplyError("Item quantity must be greater than 0.");
            } 
            else if (element == null)
            {
                ReplyError($"{itemInExchange} is not an item.");
            }
            else if (elementFor == null)
            {
                ReplyError($"{requestedItem} is not an item.");
            }

            var userDM = await userToTrade.CreateDMChannelAsync();
            var key = Config.RAND.Next();
            var firstS = exchangeItemQuantity == 1 ? string.Empty : "s";
            var secondS = requestedItemQuantity == 1 ? string.Empty : "s";

            await userDM.SendAsync($"**Offer:** {exchangeItemQuantity} {itemInExchange}{firstS}\n" +
                                   $"**Request:** {requestedItemQuantity} {requestedItem}{secondS}\n\n" +
                                   $"Please respond with \"{key}\" within 5 minutes to accept this trade.",
                                   $"Trade Request from {Context.User}");

            await ReplyAsync($"You have successfully informed {userToTrade.Boldify()} of your trade request.");

            var answer = await _interactiveService.WaitForMessage(userDM, x => x.Content == key.ToString(), TimeSpan.FromMinutes(5));

            if (answer != null)
            {
                var newOffererDbuser = await _userRepo.GetUserAsync(Context.GUser);
                var newRequesterDbuser = await _userRepo.GetUserAsync(userToTrade);

                if (!newOffererDbuser.Inventory.Contains(element.Name))
                {
                    await userDM.SendError($"{Context.User.Boldify()} does not own the following item: {element.Name}.");
                }
                else if (!newRequesterDbuser.Inventory.Contains(elementFor.Name))
                {
                    await userDM.SendError($"You do not own the following item: {elementFor.Name}.");
                }
                else if (!(newOffererDbuser.Inventory[element.Name].AsInt32 >= exchangeItemQuantity))
                {
                    await userDM.SendError($"{Context.User.Boldify()} does not own {exchangeItemQuantity} {element.Name}{firstS}.");
                }
                else if (!(newRequesterDbuser.Inventory[elementFor.Name].AsInt32 >= requestedItemQuantity))
                {
                    await userDM.SendError($"You do not own {requestedItemQuantity} {elementFor.Name}{secondS}.");
                }
                else
                {
                    await _gameService.ModifyInventoryAsync(newOffererDbuser, element.Name, -exchangeItemQuantity);
                    await _gameService.ModifyInventoryAsync(newOffererDbuser, elementFor.Name, requestedItemQuantity);

                    await _gameService.ModifyInventoryAsync(newRequesterDbuser, element.Name, exchangeItemQuantity);
                    await _gameService.ModifyInventoryAsync(newRequesterDbuser, elementFor.Name, -requestedItemQuantity);

                    await ReplyAsync("The trade has been successfully completed trade.");
                    await userDM.SendAsync("The trade has been successfully completed trade.");
                }
            }
        }
    }
}
