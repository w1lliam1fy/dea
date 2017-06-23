using DEA.Common.Extensions.DiscordExtensions;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.Administration
{
    public partial class Administration
    {
        [Command("RoleIDs")]
        [Summary("Gets the ID of all roles in the guild.")]
        public async Task RoleIDs()
        {
            string message = null;
            foreach (var role in Context.Guild.Roles)
            {
                message += $"{role.Name}: {role.Id}\n";
            }

            var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendAsync(message);

            await ReplyAsync("All Role IDs have been DMed to you.");
        }
    }
}
