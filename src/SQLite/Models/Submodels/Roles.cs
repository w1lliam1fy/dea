using System.Collections.Generic;

namespace DEA.SQLite.Models.Submodels
{
    public class Roles
    {
        public ulong[] ModRoles { get; set; }

        public List<RankRole> RankRoles { get; set; }

        public ulong NsfwRoleId { get; set; }

        public ulong MutedRoleId { get; set; }
    }
}
