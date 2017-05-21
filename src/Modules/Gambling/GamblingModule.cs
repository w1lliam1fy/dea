using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Services;

namespace DEA.Modules.Gambling
{
    [Global]
    public partial class Gambling : DEAModule
    {
        private readonly GameService _gameService;

        public Gambling(GameService gameService)
        {
            _gameService = gameService;
        }
    }
}
