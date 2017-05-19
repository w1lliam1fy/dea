using DEA.Common;
using DEA.Common.Data;
using Discord.Commands;

namespace DEA.Modules.System
{
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
