using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Gangs
{
    [Global]
    public partial class Gangs : DEAModule
    {
        private readonly GangRepository _gangRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly RateLimitService _rateLimitService;

        public Gangs(GangRepository gangRepo, UserRepository userRepo, InteractiveService interactiveService, RateLimitService rateLimitService)
        {
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _rateLimitService = rateLimitService;
        }
    }
}
