using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Trivia
{
    [Global]
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
