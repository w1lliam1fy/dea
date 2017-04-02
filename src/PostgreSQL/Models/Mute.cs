using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.PostgreSQL.Models
{
    [Table("mutes")]
    public class Mute
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("userid")]
        public ulong UserId { get; set; }
        [Column("guildid")]
        //Foreign key back to the guild
        public ulong GuildId { get; set; }
        [Column("mutelength")]
        public TimeSpan MuteLength { get; set; } = Config.DEFAULT_MUTE_TIME;
        [Column("mutedat")]
        public DateTimeOffset MutedAt { get; set; } = DateTimeOffset.Now;
    }
}