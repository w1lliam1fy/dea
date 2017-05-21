using DEA.Common;
using DEA.Common.Preconditions;
using DEA.Database.Repositories;
using DEA.Services;

namespace DEA.Modules.Polls
{
    [Global]
    public partial class Polls : DEAModule
    {
        private readonly ModerationService _moderationService;
        private readonly PollRepository _pollRepo;

        public Polls(ModerationService moderationService, PollRepository pollRepo)
        {
            _moderationService = moderationService;
            _pollRepo = pollRepo;
        }
    }
}
