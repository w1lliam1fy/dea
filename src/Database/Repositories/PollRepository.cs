using DEA.Common;
using DEA.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class PollRepository
    {
        private readonly IMongoCollection<Poll> _polls;

        public PollRepository(IMongoCollection<Poll> polls)
        {
            _polls = polls;
        }

        public async Task<Poll> FetchePollAsync(int index, ulong guildId)
        {
            var polls = await (await _polls.FindAsync(y => y.GuildId == guildId)).ToListAsync();
            try
            {
                return polls[index - 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new DEAException("This poll index does not exist.");
            }
        }

        public Task ModifyAsync(Poll poll, Expression<Func<Poll, BsonDocument>> field, BsonDocument value)
        {
            var builder = Builders<Poll>.Update;
            return _polls.UpdateOneAsync(y => y.Id == poll.Id, builder.Set(field, value));
        }

        public async Task<Poll> CreatePollAsync(DEAContext context, string name, string[] choices, TimeSpan? length = null, bool elderOnly = false, bool modOnly = false, bool createdByMod = false)
        {
            Expression<Func<Poll, bool>> expression = x => x.Name.ToLower() == name.ToLower() && x.GuildId == context.Guild.Id;
            if (await (await _polls.FindAsync(expression)).AnyAsync())
                throw new DEAException($"There is already a poll by the name \"{name}\".");
            if (name.Length > Config.MAX_POLL_SIZE)
                throw new DEAException($"The poll name may not be larger than {Config.MAX_POLL_SIZE} characters.");
            if (length.HasValue && length.Value.TotalMilliseconds > Config.MAX_POLL_LENGTH.TotalMilliseconds)
                throw new DEAException($"The poll length may not be longer than one week.");

            var createdPoll = new Poll(context.User.Id, context.Guild.Id, name, choices)
            {
                ElderOnly = elderOnly,
                ModOnly = modOnly,
                CreatedByMod = createdByMod,
            };

            if (length.HasValue) createdPoll.Length = length.Value.TotalMilliseconds;
            await _polls.InsertOneAsync(createdPoll, null, default(CancellationToken));
            return createdPoll;
        }

        public async Task RemovePollAsync(int index, ulong guildId)
        {
            var polls = await (await _polls.FindAsync(y => y.GuildId == guildId)).ToListAsync();
            try
            {
                var poll = polls[index - 1];
                await _polls.DeleteOneAsync(y => y.Id == poll.Id);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new DEAException("This poll index does not exist.");
            }
        }

    }
}
