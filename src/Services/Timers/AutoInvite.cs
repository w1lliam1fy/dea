using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class AutoInvite
    {
        private IDependencyMap _map;
        private DiscordSocketClient _client;
        private ResponseService _responseService;

        private Timer _timer;

        public AutoInvite(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _responseService = _map.Get<ResponseService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.AUTO_INVITE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var msgsToDelete = new List<IUserMessage>();
            foreach (var guild in _client.Guilds)
            {
                var perms = (guild.CurrentUser as IGuildUser).GetPermissions(guild.DefaultChannel);
                if (perms.SendMessages && perms.EmbedLinks)
                {
                    var msg = await _responseService.Send(guild.DefaultChannel,
                       "DEA is a public Discord Bot known for its infamous Nacro's references and spicy memes.\n" +
                       "To be able to use all owner commands such as `$reset` and `$add`, you may add DEA to your own server.\n" +
                       $"Click the following link to do so: https://discordapp.com/oauth2/authorize?client_id={guild.CurrentUser.Id}&scope=bot&permissions=410119182");
                    msgsToDelete.Add(msg);
                }
            }
            await Task.Delay(90000);
            foreach (var msg in msgsToDelete)
                await msg.DeleteAsync();
        }
    }
}