using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Items
{
    public partial class Items : Module
    {
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly GameService _gameService;
        private readonly RateLimitService _rateLimitService;

        public Items(UserRepository userRepo, InteractiveService interactiveService, GameService gameService, RateLimitService rateLimitService)
        {
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _gameService = gameService;
            _rateLimitService = rateLimitService;
        }
    }
}
