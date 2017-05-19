using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using DEA.Services.Handlers;

namespace DEA.Modules.Owners
{
    [Require(Attributes.ServerOwner)]
    public partial class Owners : DEAModule
    {
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;
        private readonly UserRepository _userRepo;
        private readonly RankHandler _rankHandler;
        private readonly GameService _gameService;
        private readonly Item[] _items;

        public Owners(GuildRepository guildRepo, UserRepository userRepo, GangRepository gangRepo, RankHandler rankHandler, GameService gameService, Item[] items)
        {
            _guildRepo = guildRepo;
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _rankHandler = rankHandler;
            _gameService = gameService;
            _items = items;
        }
    }
}
