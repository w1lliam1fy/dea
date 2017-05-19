using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Gangs
{
    public partial class Gangs : DEAModule
    {
        private readonly GangRepository _gangRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;

        public Gangs(GangRepository gangRepo, UserRepository userRepo, InteractiveService interactiveService)
        {
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
        }
    }
}
