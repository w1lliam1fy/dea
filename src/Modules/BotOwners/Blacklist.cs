using DEA.Database.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.BotOwners
{
    public partial class BotOwners
    {
        [Command("Blacklist")]
        [Summary("Blacklist a user from DEA entirely to the fullest extent.")]
        public async Task Blacklist(ulong userId)
        {
            var username = string.Empty;
            var avatarUrl = string.Empty;

            try
            {
                var user = await (Context.Client as IDiscordClient).GetUserAsync(userId);

                username = user.Username;
                avatarUrl = user.GetAvatarUrl();
            }
            catch { }

            var blacklist = new Blacklist(userId, username, avatarUrl);
            await _blacklistRepo.InsertAsync(blacklist);

            foreach (var guild in Context.Client.Guilds)
            {
                if (guild.OwnerId == userId)
                {
                    await _blacklistRepo.AddGuildAsync(userId, guild.Id);
                    await guild.LeaveAsync();
                }
            }

            await ReplyAsync($"You have successfully blacklisted the following ID: {userId}.");
        }
    }
}