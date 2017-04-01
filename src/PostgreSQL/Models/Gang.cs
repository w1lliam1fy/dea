using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    [Table("gangs")]
    public class Gang
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("leaderid")]
        [DataType("BIGINT")]
        public ulong LeaderId { get; set; }
        [Column("guildid")]
        [DataType("BIGINT")]
        public ulong GuildId { get; set; }
        [Column("members")]
        public List<ulong> Members { get; set; }
        [Column("wealth")]
        public double Wealth { get; set; } = 0.0;
        [Column("raid")]
        public DateTimeOffset Raid { get; set; } = DateTimeOffset.Now.AddYears(-1);
    }
}
