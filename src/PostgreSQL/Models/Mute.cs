using System;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    [Table("mutes")]
    public class Mute
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("userid")]
        [DataType("BIGINT")]
        public ulong UserId { get; set; }
        [Column("guildid")]
        [DataType("BIGINT")]
        public ulong GuildId { get; set; }
        [Column("mutelength")]
        public NpgsqlTimeSpan MuteLength { get; set; } = Config.DEFAULT_MUTE_TIME;
        [Column("mutedat")]
        public DateTimeOffset MutedAt { get; set; } = DateTimeOffset.Now;
    }
}