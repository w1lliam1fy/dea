using DEA.SQLite.Repository;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace DEA.SQLite.Models
{
    public class Guild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        public string Prefix { get; set; } = "$";

        public ulong ModRoleId { get; set; } = 0;

        public ulong ModLogChannelId { get; set; } = 0;

        public ulong DetailedLogsChannelId { get; set; } = 0;

        public ulong NSFWChannelId { get; set; } = 0;

        public ulong MutedRoleId { get; set; } = 0;

        public uint CaseNumber { get; set; } = 1;

        public bool DM { get; set; } = false;

        public bool NSFW { get; set; } = false;

        public ulong GambleChannelId { get; set; } = 0;

        // SQLITE cannot store arrays, so this is required as well as adding the rank to the getter/setter of RankIds

        public ulong Rank1Id { get; set; } = 0;
        public ulong Rank2Id { get; set; } = 0;
        public ulong Rank3Id { get; set; } = 0;
        public ulong Rank4Id { get; set; } = 0;
        
        [NotMapped]
        public ulong[] RankIds {
            get {
                return new ulong[] { Rank1Id, Rank2Id, Rank3Id, Rank4Id };
            }
        }

        public async Task SetRankIds(GuildRepository guildRepo, ulong guildId, ulong[] value) {
            await guildRepo.ModifyAsync(x => {
                x.Rank1Id = value[0];
                x.Rank2Id = value[1];
                x.Rank3Id = value[2];
                x.Rank4Id = value[3];
                return Task.CompletedTask;
            }, guildId);
        }

        public ulong NSFWRoleId { get; set; } = 0;

    }
}
