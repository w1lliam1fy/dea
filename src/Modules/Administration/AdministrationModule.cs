using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;

namespace DEA.Modules.Administration
{
    [Require(Attributes.Admin)]
    public partial class Administration : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public Administration(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }
    }
}
