namespace DEA.Database.Models
{

    public partial class Gang : Model
    {
        public Gang(ulong leaderId, ulong guildId, string name)
        {
            LeaderId = leaderId;
            GuildId = guildId;
            Name = name;
        }

        public string Name { get; set; }

        public ulong LeaderId { get; set; }

        public ulong GuildId { get; set; }

        public decimal Wealth { get; set; } = 0;

        public ulong[] Members { get; set; } = new ulong[] { };
    }
}