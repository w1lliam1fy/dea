using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    [Table("gangs")]
    public class Guild
    { 
        [Key]
        [Column("id")]
        [DataType("BIGINT")]
        public ulong Id { get; set; }

        //Roles
        [Column("modroles")]
        public List<ulong> ModRoles { get; set; }
        [Column("rankroles")]
        public Dictionary<ulong, double> RankRoles { get; set; }
        [Column("nsfwroleid")]
        public ulong NsfwRoleId { get; set; }
        [Column("muteroleid")]
        public ulong MutedRoleId { get; set; }

        //Channels
        [Column("modlogid")]
        public ulong ModLogId { get; set; } = 0;
        [Column("detailedlogsid")]
        public ulong DetailedLogsId { get; set; } = 0;
        [Column("gambleid")]
        public ulong GambleId { get; set; } = 0;
        [Column("nsfwid")]
        public ulong NsfwId { get; set; } = 0;

        //Options
        [Column("prefix")]
        public string Prefix { get; set; } = "$";
        [Column("nsfw")]
        public bool Nsfw { get; set; } = false;
        [Column("globalchattingmultiplier")]
        public double GlobalChattingMultiplier = 1.0;
        [Column("tempmultiplierincreaserate")]
        public double TempMultiplierIncreaseRate = 0.1;
        [Column("jumprequirement")]
        public double JumpRequirement { get; set; } = 500.0;
        [Column("stealrequirement")]
        public double StealRequirement { get; set; } = 2500.0;
        [Column("robrequirement")]
        public double RobRequirement { get; set; } = 5000.0;
        [Column("bullyrequirement")]
        public double BullyRequirement { get; set; } = 10000.0;
        [Column("fiftyx2requirement")]
        public double FiftyX2Requirement { get; set; } = 25000.0;

        //Misc
        [Column("casenumber")]
        public uint CaseNumber { get; set; } = 1;
    }
}
