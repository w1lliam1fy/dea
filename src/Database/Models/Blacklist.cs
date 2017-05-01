namespace DEA.Database.Models
{
    public partial class Blacklist : Model
    {
        public Blacklist(ulong userId)
        {
            UserId = userId;
        }

        public ulong UserId { get; set; }

        public ulong[] GuildIds { get; set; } = new ulong[] { };

    }
}
