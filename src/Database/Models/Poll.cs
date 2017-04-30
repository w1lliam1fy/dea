using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace DEA.Database.Models
{
    public partial class Poll : Model
    {
        public Poll(ulong creatorId, ulong guildId, string name, string[] choices)
        {
            Name = name;
            CreatorId = creatorId;
            GuildId = guildId;
            Choices = choices;
        }

        public string Name { get; set; }

        public ulong CreatorId { get; set; }

        public ulong GuildId { get; set; }

        public string[] Choices { get; set; } = new string[] { };

        public BsonDocument VotesDocument { get; set; } = new BsonDocument();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public double Length { get; set; } = Config.DEFAULT_POLL_LENGTH.TotalMilliseconds;

        public bool ElderOnly { get; set; } = false;

        public bool ModOnly { get; set; } = false;

        public bool CreatedByMod { get; set; } = false;

        public IReadOnlyDictionary<string, int> Votes()
        {
            var votesDictionary = new Dictionary<string, int>();

            for (int i = 0; i < Choices.Length; i++)
            {
                votesDictionary.Add(Choices[i], 0);
            }

            foreach (var vote in VotesDocument)
            {
                votesDictionary[vote.Value.AsString]++;
            }

            return votesDictionary;
        }

    }
}
