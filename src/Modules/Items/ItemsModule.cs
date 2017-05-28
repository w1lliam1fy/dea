using DEA.Common;
using DEA.Common.Items;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;
using System.Linq;

namespace DEA.Modules.Items
{
    [Global]
    public partial class Items : DEAModule
    {
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly GameService _gameService;
        private readonly Item[] _items;
        private readonly Fish[] _fish;
        private readonly Meat[] _meat;
        private readonly Crate[] _crates;
        private readonly CrateItem[] _crateItems;
        private readonly RateLimitService _rateLimitService;
        private readonly int _itemWeaponOdds;

        public Items(UserRepository userRepo, InteractiveService interactiveService, GuildRepository guildRepo, GangRepository gangRepo, 
                     GameService gameService, Item[] items, Fish[] fish,  Meat[] meat, Crate[] crates, CrateItem[] crateItems, 
                     RateLimitService rateLimitService)
        {
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _gameService = gameService;
            _items = items;
            _fish = fish;
            _meat = meat;
            _crates = crates;
            _crateItems = crateItems;
            _rateLimitService = rateLimitService;
            _itemWeaponOdds = _crateItems.Sum(x => x.CrateOdds);
        }
    }
}
