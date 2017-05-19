using DEA.Common;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using DEA.Services.Handlers;

namespace DEA.Modules.General
{
    public partial class General : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;
        private readonly RankHandler _rankHandler;
        private readonly GameService _gameService;
        private readonly Item[] _items;

        public General(UserRepository userRepo, GuildRepository guildRepo, GangRepository gangRepo, RankHandler rankHandler, GameService gameService, Item[] items)
        {
            _userRepo = userRepo;
            _guildRepo = guildRepo;
            _gangRepo = gangRepo;
            _rankHandler = rankHandler;
            _gameService = gameService;
            _items = items;
        }
    }
}
