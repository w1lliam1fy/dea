using DEA.Common.Extensions.DiscordExtensions;
using DEA.Common.Preconditions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Common
{
    [Global]
    public abstract class Module : ModuleBase<Context>
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
            throw new FriendlyException(message);
        }
    }
}
