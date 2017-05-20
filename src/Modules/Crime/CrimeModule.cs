using DEA.Common;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Crime
{
    public partial class Crime : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly GangRepository _gangRepo;
        private readonly ModerationService _moderationService;
        private readonly GameService _gameService;
        private readonly InteractiveService _interactiveService;
        private readonly Item[] _items;
        private readonly RateLimitService _rateLimitService;

        public Crime(UserRepository userRepo, GangRepository gangRepo, ModerationService moderationService, GameService gameService, InteractiveService interactiveService, Item[] items, RateLimitService rateLimitService)
        {
            _userRepo = userRepo;
            _gangRepo = gangRepo;
            _moderationService = moderationService;
            _gameService = gameService;
            _interactiveService = interactiveService;
            _items = items;
            _rateLimitService = rateLimitService;
        }
    }
}
