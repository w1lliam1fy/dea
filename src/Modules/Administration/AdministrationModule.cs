using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using Discord.Commands;

namespace DEA.Modules.Administration
{
    [Require(Attributes.Admin)]
    [Summary("These commands may only be used by a user with the set mod role with a permission level of 2, the Administrator permission.")]
    public partial class Administration : DEAModule
    {
        private readonly GuildRepository _guildRepo;

        public Administration(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }
    }
}
