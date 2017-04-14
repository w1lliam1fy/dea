using DEA.Services.Handlers;
using DEA.Services.Timers;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        public Ready()
        {
            DEABot.Client.Ready += HandleReady;
        }

        private async Task HandleReady()
        {
            await DEABot.Client.SetGameAsync("USE $help");
            new MessageRecieved();
            new UserEvents();
            new RoleEvents();
            new ChannelEvents();
            new ErrorHandler();
            new ApplyIntrestRate();
            new AutoUnmute();
            new ResetTempMultiplier();
        }

    }
}
