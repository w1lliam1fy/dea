using DEA.Database.Models;
using DEA.Database.Repository;
using DEA.Services;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DEA.Resources
{
    public class DEAModule : ModuleBase<SocketCommandContext>
    {

        public User DbUser;
        public decimal Cash;
        
        public Guild DbGuild;
        public string Prefix;

        public Gang Gang;

        public void InitializeData()
        {
            var user = UserRepository.FetchUser(Context);
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            DbUser = user;
            Cash = DbUser.Cash;

            DbGuild = guild;
            Prefix = DbGuild.Prefix;

            if (GangRepository.InGang(Context.User.Id, Context.Guild.Id))
                Gang = GangRepository.FetchGang(Context);
        }

        public async Task<IUserMessage> Reply(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();

            var builder = new EmbedBuilder()
            {
                Description = $"{ResponseMethods.Name(Context.User as IGuildUser)}, {description}",
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
            return (string.IsNullOrWhiteSpace(user.Nickname)) ? $"**{user.Username}**" : $"**{user.Nickname}**";
        }

        public void Error(string message)
        {
            throw new DEAException(message);
        }
    }
}
