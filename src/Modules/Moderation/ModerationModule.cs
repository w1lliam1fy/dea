using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;
using Discord.Commands;

namespace DEA.Modules.Moderation
{
    [Require(Attributes.Moderator)]
    [Summary("These commands may only be used by a user with the set mod role with a permission level of 1, or the Administrator permission.")]
    public partial class Moderation : Module
    {
        private readonly MuteRepository _muteRepo;
        private readonly ModerationService _moderationService;

        public Moderation(MuteRepository muteRepo, ModerationService moderationService)
        {
            _muteRepo = muteRepo;
            _moderationService = moderationService;
        }
    }
}
