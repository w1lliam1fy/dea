using DEA.Common;
using DEA.Services;

namespace DEA.Modules.Gambling
{
    public partial class Gambling : DEAModule
    {
        private readonly GameService _gameService;

        public Gambling(GameService gameService)
        {
            _gameService = gameService;
        }
    }
}
