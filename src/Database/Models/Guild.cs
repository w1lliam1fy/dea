using System.Collections.Generic;

namespace DEA.Database.Models
{
    
    public partial class Guild
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Guild()
        {
            Gangs = new HashSet<Gang>();
            ModRoles = new HashSet<ModRole>();
            Mutes = new HashSet<Mute>();
            RankRoles = new HashSet<RankRole>();
            Users = new HashSet<User>();
        }

        public decimal Id { get; set; }

        //Global
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mute> Mutes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gang> Gangs { get; set; }

        //Roles

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModRole> ModRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RankRole> RankRoles { get; set; }

        public decimal MutedRoleId { get; set; }

        public decimal NsfwRoleId { get; set; }

        //Channels

        public decimal DetailedLogsId { get; set; }

        public decimal NsfwId { get; set; }

        public decimal GambleId { get; set; }

        public decimal ModLogId { get; set; }

        //Options

        public bool Nsfw { get; set; } = false;

        public string Prefix { get; set; } = "$";

        public double GlobalChattingMultiplier = 1.0;

        public double TempMultiplierIncreaseRate = 0.10;

        public double JumpRequirement { get; set; } = 500.0;

        public double StealRequirement { get; set; } = 2500.0;

        public double BullyRequirement { get; set; } = 5000.0;

        public double RobRequirement { get; set; } = 10000.0;

        public double FiftyX2Requirement { get; set; } = 25000.0;

        //Misc

        public int CaseNumber { get; set; } = 1;

    }
}