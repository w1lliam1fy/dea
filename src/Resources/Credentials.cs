namespace DEA.Resources
{
    public class Credentials
    {
        public string Token { get; set; }

        public ulong[] OwnerIds { get; set; }

        public int ShardCount { get; set; } = 1;

        public string PostgreUserId { get; set; }

        public string PostgrePassword { get; set; }

        public string PostgreServer { get; set; }

        public string PostgreDatabase { get; set; }

        public bool PostgrePooling { get; set; } = true;
    }
}