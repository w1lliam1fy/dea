using DEA.Common.Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class InteractiveService
    {
        private readonly DiscordSocketClient _client;

        public InteractiveService(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task<IUserMessage> WaitForMessage(IMessageChannel channel, Predicate<IUserMessage> filter, TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                timeout = Config.DEFAULT_WAITFORMESSAGE;
            }

            var blockToken = new CancellationTokenSource();
            IUserMessage response = null;

            Func<IMessage, Task> isValid = (messageParameter) =>
            {
                var message = messageParameter as IUserMessage;
                if (message == null)
                {
                    return Task.CompletedTask;
                }
                else if (!filter(message))
                {
                    return Task.CompletedTask;
                }
                else if (message.Channel.Id != channel.Id)
                {
                    return Task.CompletedTask;
                }

                response = message;
                blockToken.Cancel(true);
                return Task.CompletedTask;
            };

            _client.MessageReceived += isValid;
            try
            {
                if (timeout == TimeSpan.Zero)
                {
                    await Task.Delay(-1, blockToken.Token);
                }
                else
                {
                    await Task.Delay(timeout.Value, blockToken.Token);
                }
            }
            catch (TaskCanceledException)
            {
                return response;
            }
            catch
            {
                throw;
            }
            finally
            {
                _client.MessageReceived -= isValid;
            }
            return null;
        }
    }
}
