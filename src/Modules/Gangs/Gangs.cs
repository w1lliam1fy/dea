using DEA.Common;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using System.Collections.Generic;

namespace DEA.Modules.Gangs
{
    public partial class Gangs : DEAModule
    {
        private readonly GangRepository _gangRepo;
        private readonly UserRepository _userRepo;
        private readonly InteractiveService _interactiveService;
        private readonly List<CommandTimeout> _commandTimeouts;

        public Gangs(GangRepository gangRepo, UserRepository userRepo, InteractiveService interactiveService, List<CommandTimeout> commandTimeouts)
        {
            _gangRepo = gangRepo;
            _userRepo = userRepo;
            _interactiveService = interactiveService;
            _commandTimeouts = commandTimeouts;
        }
    }
}
