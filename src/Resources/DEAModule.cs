using DEA.Database.Models;
using DEA.Database.Repository;
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

        protected override void BeforeExecute()
        {
            var user = UserRepository.FetchUser(Context);
            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            DbUser = user;
            Cash = user.Cash;

            DbGuild = guild;
            Prefix = guild.Prefix;

            if (GangRepository.InGang(Context.User.Id, Context.Guild.Id))
                Gang = GangRepository.FetchGang(Context);
        }

        public async Task<IUserMessage> Reply(string description, string title = null, Color color = default(Color))
        {
            var rand = new Random();
            var builder = new EmbedBuilder()
            {
                Description = $"{Context.User.Mention}, {description}",
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

        public void Error(string message)
        {
            throw new DEAException(message);
        }
    }
}
