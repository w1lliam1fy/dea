using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Trivia
{
    public partial class Trivia : DEAModule
    {
        private readonly GuildRepository _guildRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly GameService _gameService;

        public Trivia(GuildRepository guildRepo, UserRepository userRepo, InteractiveService interactiveService, GameService gameService)
        {
            _guildRepo = guildRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _gameService = gameService;
        }
    }
}
