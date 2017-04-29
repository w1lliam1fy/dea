using DEA.Common.Extensions.DiscordExtensions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.Common
{
    public abstract class DEAModule : ModuleBase<DEAContext>
    {

        /// <summary>
        /// Replies to the context user, starting the message with their username, discriminator and a comma.
        /// </summary>
        /// <param name="message">The content of the embed.</param>
        /// <param name="isTTS">This parameter is ignored, only present due to the ModuleBase override.</param>
        /// <param name="embed">This parameter is ignored, only present due to the ModuleBase override.</param>
        /// <param name="options">This parameter is ignored, only present due to the ModuleBase override.</param>
        /// <returns>Task returning the sent message.</returns>
        protected override Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null) =>
            Context.Channel.ReplyAsync(Context.User, message);

        /// <summary>
        /// Sends a embedded message.
        /// </summary>
        /// <param name="description">The content of the embed.</param>
        /// <param name="title">The title of the embed.</param>
        /// <param name="color">The color of the embed.</param>
        /// <returns>Task returning the sent message.</returns>
        public Task<IUserMessage> SendAsync(string description, string title = null, Color color = default(Color)) =>
            Context.Channel.SendAsync(description, title, color);

        /// <summary>
        /// Throws a DEAException which will get caught by the error handler.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public void ReplyError(string message) =>
            throw new DEAException(message);

    }
}
