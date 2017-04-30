using DEA.Database.Models;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private readonly IDependencyMap _map;
        private readonly IMongoCollection<Guild> _guilds;
        private readonly IMongoCollection<NewGuild> _newGuilds;
        private readonly DiscordSocketClient _client;

        public Ready(IDependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _guilds = _map.Get<IMongoCollection<Guild>>();
            _newGuilds = _map.Get<IMongoCollection<NewGuild>>();
            _client.Ready += HandleReady;
        }

        private Task HandleReady()
        {
            foreach (var guild in _guilds.FindSync(Builders<Guild>.Filter.Empty).ToList())
            {
                var newGuild = new NewGuild(guild.Id)
                {
                    AutoTrivia = guild.AutoTrivia,
                    CaseNumber = guild.CaseNumber,
                    GambleChannelId = guild.GambleChannelId,
                    GlobalChattingMultiplier = guild.GlobalChattingMultiplier,
                    GuildId = guild.Id,
                    Id = new MongoDB.Bson.ObjectId(),
                    ModLogChannelId = guild.ModLogChannelId,
                    ModRoles = guild.ModRoles,
                    MutedRoleId = guild.MutedRoleId,
                    Nsfw = guild.Nsfw,
                    NsfwChannelId = guild.NsfwChannelId,
                    Prefix = guild.Prefix,
                    RankRoles = guild.RankRoles,
                    TempMultiplierIncreaseRate = guild.TempMultiplierIncreaseRate,
                    Trivia = guild.Trivia,
                };
                _newGuilds.InsertOne(newGuild);
            }

            return Task.Run(async () => await _client.SetGameAsync("USE $help"));
        }
    }
}
