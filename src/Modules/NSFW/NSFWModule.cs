using DEA.Common;
using DEA.Database.Repositories;

namespace DEA.Modules.NSFW
{
    public partial class NSFW : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public NSFW(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }
    }
}
