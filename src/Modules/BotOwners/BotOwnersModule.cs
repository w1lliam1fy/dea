using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;

namespace DEA.Modules.BotOwners
{
    [Require(Attributes.BotOwner)]
    public partial class BotOwners : DEAModule
    {
        private readonly GuildRepository _guildRepo;
        private readonly BlacklistRepository _blacklistRepo;

        public BotOwners(GuildRepository guildRepo, BlacklistRepository blacklistRepo)
        {
            _guildRepo = guildRepo;
            _blacklistRepo = blacklistRepo;
        }
    }
}
