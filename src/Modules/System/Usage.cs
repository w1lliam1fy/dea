using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Modules.System
{
    public partial class System
    {
        [Command("Usage")]
        [Summary("Explanation of how commands are used.")]
        public Task Usage()
        {
            return SendAsync("**Optional paramater:** `[]`\n\n**Required paramater:** `<>`\n\n**Parameter with spaces:** `\"This is one parameter\"`",
                             "Command Usage");
        }
    }
}
