using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using DEA.Services.Handlers;

namespace DEA.Modules.General
{
    [Global]
    public partial class General : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;
        private readonly RankHandler _RankHandler;
        private readonly GameService _gameService;
        private readonly RateLimitService _rateLimitService;
        private readonly Item[] _items;

        public General(UserRepository userRepo, GuildRepository guildRepo, GangRepository gangRepo, RankHandler rankHandler, GameService gameService, RateLimitService rateLimitService, Item[] items)
        {
            _userRepo = userRepo;
            _guildRepo = guildRepo;
            _gangRepo = gangRepo;
            _RankHandler = rankHandler;
            _gameService = gameService;
            _rateLimitService = rateLimitService;
            _items = items;
        }
    }
}
