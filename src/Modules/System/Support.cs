using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Support")]
        [Summary("Join our Official DEA Support Server!")]
        public Task Support()
        {
            return ReplyAsync($"If you are looking for spicest of memes, or simply the best support for DEA, then join our goddamn dedicated server: https://discord.gg/gvyma7H.");
        }
    }
}
