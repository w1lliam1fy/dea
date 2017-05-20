using DEA.Common;
using DEA.Common.Data;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using System.Linq;

namespace DEA.Modules.Items
{
    public partial class Items : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly GameService _gameService;
        private readonly Item[] _items;
        private readonly RateLimitService _rateLimitService;
        private readonly int _itemWeaponOdds;

        public Items(UserRepository userRepo, InteractiveService interactiveService, GuildRepository guildRepo, GangRepository gangRepo, GameService gameService, Item[] items, RateLimitService rateLimitService)
        {
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _gameService = gameService;
            _items = items;
            _rateLimitService = rateLimitService;
            _itemWeaponOdds = _items.Where(x => Config.WEAPON_TYPES.Any(y => y == x.ItemType) || x.Name == "Kevlar").Sum(x => x.Odds);
        }
    }
}
