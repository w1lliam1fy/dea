using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Modules")]
        [Alias("Module")]
        [Summary("All command modules.")]
        public Task Modules()
        {
            string modules = string.Empty;
            foreach (var module in _commandService.Modules)
            {
                modules += $"{module.Name}, ";
            }

            return ReplyAsync("Current command modules: " + modules.Remove(modules.Length - 2) + ".");
        }
    }
}
