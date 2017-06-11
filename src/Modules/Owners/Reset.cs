using Discord.Commands;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace DEA.Modules.Owners
{
    public partial class Owners
    {
        [Command("Reset")]
        [Summary("Resets all user and gang data in your server.")]
        public async Task Reset()
        {
            await ReplyAsync("Are you sure you wish to reset all DEA related data within your server? Reply with \"yes\" to continue.");
            var response = await _interactiveService.WaitForMessage(Context.Channel, x => x.Author.Id == Context.User.Id && x.Content.ToLower() == "yes");

            if (response != null)
            {
                await _userRepo.DeleteAsync(x => x.GuildId == Context.Guild.Id);
                await _gangRepo.DeleteAsync(x => x.GuildId == Context.Guild.Id);
                await _guildRepo.ModifyAsync(Context.DbGuild, x => x.Trivia = new BsonDocument());

                await ReplyAsync("You have successfully reset all data in your server!");
            } 
        }
    }
}
