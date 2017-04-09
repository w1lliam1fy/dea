using DEA.Services;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        public Ready()
        {
            DEABot.Client.Ready += HandleReady;

            new UserEvents();
            new RoleEvents();
            new ChannelEvents();
            new RecurringMethods();
            new ErrorHandler();
        }

        private async Task HandleReady()
        {
            await DEABot.Client.SetGameAsync("USE $help");
        }

    }
}
