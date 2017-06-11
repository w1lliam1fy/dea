using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Gangs
{
    public partial class Gangs : Module
    {
        private readonly GangRepository _gangRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly CooldownService _cooldownService;

        public Gangs(GangRepository gangRepo, UserRepository userRepo, InteractiveService interactiveService, CooldownService CooldownService)
        {
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _cooldownService = CooldownService;
        }
    }
}
