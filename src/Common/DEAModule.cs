using DEA.Services;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Common
{
    public class DEAModule : ModuleBase<DEAContext>
    {

        public async Task<IUserMessage> Reply( string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{ResponseMethods.Name(Context.User as IGuildUser, Context.DbUser)}, {description}",
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return await ReplyAsync(string.Empty, embed: builder);
        }

        public async Task<IUserMessage> Send(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = description,
                Color = Config.COLORS[rand.Next(1, Config.COLORS.Length) - 1]
            };
            if (title != null) builder.Title = title;
            if (color.RawValue != default(Color).RawValue) builder.Color = color;

            return await ReplyAsync(string.Empty, embed: builder);
        }

        public string Name()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return (string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**";
            else
                return $"**{Context.DbUser.Name}**";
        }

        public string TitleName()
        {
            var user = Context.User as IGuildUser;
            if (string.IsNullOrWhiteSpace(Context.DbUser.Name))
                return (string.IsNullOrWhiteSpace(user.Nickname)) ? user.Username : user.Nickname;
            else
                return Context.DbUser.Name;
        }

        public void Error(string message)
        {
            throw new DEAException(message);
        }
    }
}
