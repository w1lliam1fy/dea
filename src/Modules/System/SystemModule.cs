using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Common.Utilities;
using Discord.Commands;

namespace DEA.Modules.System
{
    [Global]
    public partial class System : DEAModule
    {
        private readonly CommandService _commandService;
        private readonly Statistics _statistics;

        public System(CommandService commandService, Statistics statistics)
        {
            _commandService = commandService;
            _statistics = statistics;
        }
    }
}
