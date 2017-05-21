using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using Discord.Commands;

namespace DEA.Modules.NSFW
{
    [Global]
    [RequireNsfw]
    public partial class NSFW : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public NSFW(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }
    }
}
