using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Invite")]
        [Summary("Invite DEA to your server!")]
        public Task Invite()
        {
            return ReplyAsync($"Click on the following link to add DEA to your server: https://discordapp.com/oauth2/authorize?client_id={Context.Guild.CurrentUser.Id}&scope=bot&permissions=410119182");
        }
    }
}
