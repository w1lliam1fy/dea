namespace DEA.Common
{
    public class Credentials
    {
        public string Token { get; set; }

        public ulong[] OwnerIds { get; set; }

        public int ShardCount { get; set; } = 1;

        public string MongoDBConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; }
    }
}