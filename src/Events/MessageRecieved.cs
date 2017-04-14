using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using DEA.Database.Repository;
using DEA.Services.Handlers;

namespace DEA.Events
{
    public class MessageRecieved
    {

        public MessageRecieved()
        {
            DEABot.Client.MessageReceived += HandleMessageRecievedAsync;
        }

        private async Task HandleMessageRecievedAsync(SocketMessage s)
        {
            DEABot.Messages++;

            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var Context = new SocketCommandContext(DEABot.Client, msg);

            if (Context.Guild == null) return;

            if (Context.User.IsBot) return;

            int argPos = 0;

            var guild = GuildRepository.FetchGuild(Context.Guild.Id);

            if (!msg.HasStringPrefix(guild.Prefix, ref argPos) || !msg.HasMentionPrefix(DEABot.Client.CurrentUser, ref argPos) && 
                msg.Content.Length >= Config.MIN_CHAR_LENGTH && !msg.Content.StartsWith(":"))
            {
                var user = UserRepository.FetchUser(Context);

                if (DateTime.UtcNow.Subtract(user.Message).TotalMilliseconds > user.MessageCooldown)
                {
                    UserRepository.Modify(DEABot.UserUpdateBuilder.Combine(
                        DEABot.UserUpdateBuilder.Set(x => x.Cash, guild.GlobalChattingMultiplier * user.TemporaryMultiplier * user.InvestmentMultiplier + user.Cash),
                        DEABot.UserUpdateBuilder.Set(x => x.TemporaryMultiplier, user.TemporaryMultiplier + guild.TempMultiplierIncreaseRate),
                        DEABot.UserUpdateBuilder.Set(x => x.Message, DateTime.UtcNow)), Context);
                    await RankHandler.Handle(Context.Guild, Context.User.Id);
                }
            }
        }
    }
}
