using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Common
{
    public abstract class DEAModule : ModuleBase<DEAContext>
    {
        protected override Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.ReplyAsync(Context.User, message);
        }

        public Task<IUserMessage> SendAsync(string description, string title = null, Color color = default(Color))
        {
            return Context.Channel.SendAsync(description, title, color);
        }

        public void ReplyError(string message)
        {
            throw new DEAException(message);
        }
    }
}
