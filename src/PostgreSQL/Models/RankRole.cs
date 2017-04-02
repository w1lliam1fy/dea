using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.PostgreSQL.Models
{
    [Table("rankroles")]
    public class RankRole
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("guildid")]
        //Foreign key back to the guild
        public ulong GuildId { get; set; }
        [Column("roleid")]
        public ulong RoleId { get; set; }
        [Column("cashrequired")]
        public double CashRequired { get; set; }
    }
}
