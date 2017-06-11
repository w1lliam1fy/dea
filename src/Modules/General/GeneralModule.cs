using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;
using DEA.Services.Handlers;

namespace DEA.Modules.General
{ 
    public partial class General : Module
    {
        private readonly UserRepository _userRepo;
        private readonly GuildRepository _guildRepo;
        private readonly GangRepository _gangRepo;
        private readonly RankHandler _RankHandler;
        private readonly GameService _gameService;
        private readonly CooldownService _cooldownService;

        public General(UserRepository userRepo, GuildRepository guildRepo, GangRepository gangRepo, RankHandler rankHandler, GameService gameService, CooldownService cooldownService)
        {
            _userRepo = userRepo;
            _guildRepo = guildRepo;
            _gangRepo = gangRepo;
            _RankHandler = rankHandler;
            _gameService = gameService;
            _cooldownService = cooldownService;
        }
    }
}
