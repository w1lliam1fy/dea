using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Moderation
{
    [Require(Attributes.Moderator)]
    public partial class Moderation : DEAModule
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
