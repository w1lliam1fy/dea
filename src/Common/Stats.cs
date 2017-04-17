using DEA.Database.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DEA.Common
{
    public class Stats
    {
        private IMongoCollection<User> _users;
        private IMongoCollection<Guild> _guilds;
        private IMongoCollection<Gang> _gangs;
        private IMongoCollection<Mute> _mutes;

        public Stats(IMongoCollection<User> users, IMongoCollection<Guild> guilds, IMongoCollection<Gang> gangs, IMongoCollection<Mute> mutes)
        {
            _users = users;
            _guilds = guilds;
            _gangs = gangs;
            _mutes = mutes;
        }

        public int MessagesRecieved { get; set; }

        public int CommandsRun { get ; set; }

        public Task<long> UserDocuments() => _users.CountAsync(Builders<User>.Filter.Empty);

        public Task<long> GuildDocuments() => _guilds.CountAsync(Builders<Guild>.Filter.Empty);

        public Task<long> GangDocuments() => _gangs.CountAsync(Builders<Gang>.Filter.Empty);

        public Task<long> MuteDocuments() => _mutes.CountAsync(Builders<Mute>.Filter.Empty);

        public async Task<long> DbDocuments() => await GuildDocuments() + await GangDocuments() + await MuteDocuments() + await UserDocuments();

    }
}
