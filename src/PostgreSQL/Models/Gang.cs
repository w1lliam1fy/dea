using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.PostgreSQL.Models
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
        public ulong LeaderId { get; set; }
        [Column("guildid")]
        public ulong GuildId { get; set; }
        //foreign key back to guild
        [Column("members")]
        public List<User> Members { get; set; }
        [Column("wealth")]
        public double Wealth { get; set; } = 0.0;
        [Column("raid")]
        public DateTimeOffset Raid { get; set; } = DateTimeOffset.Now.AddYears(-1);
    }
}
