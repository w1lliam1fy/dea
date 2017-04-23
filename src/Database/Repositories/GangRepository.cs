using DEA.Common;
using DEA.Database.Models;
using Discord;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public class GangRepository
    {
        private readonly IMongoCollection<Gang> _gangs;

        public GangRepository(IMongoCollection<Gang> gangs)
        {
            _gangs = gangs;
        }

        public Task ModifyAsync(DEAContext context, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && 
                                              c.GuildId == context.Guild.Id, builder.Set(field, value));
        }

        public Task ModifyAsync(IGuildUser member, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => (c.LeaderId == member.Id || c.Members.Any(x => x == member.Id)) && 
                                              c.GuildId == member.GuildId, builder.Set(field, value));
        }

        private Task ModifyAsync(IGuildUser member, Expression<Func<Gang, ulong>> field, ulong value)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => (c.LeaderId == member.Id || c.Members.Any(x => x == member.Id)) &&
                                              c.GuildId == member.GuildId, builder.Set(field, value));
        }

        public Task ModifyAsync(string gangName, ulong guildId, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId, builder.Set(field, value));
        }

        public async Task<Gang> FetchGangAsync(DEAContext context)
        {
            var gang = await (await _gangs.FindAsync(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && c.GuildId == context.Guild.Id)).SingleOrDefaultAsync();
            if (gang == default(Gang))
                throw new DEAException("You are not in a gang.");
            return gang;
        }

        public async Task<Gang> FetchGangAsync(IGuildUser user)
        {
            var gang = await (await _gangs.FindAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId)).SingleOrDefaultAsync();
            if (gang == default(Gang))
                throw new DEAException("This user is not in a gang.");
            return gang;
        }

        public async Task<Gang> FetchGangAsync(string gangName, ulong guildId)
        {
            var gang = await (await _gangs.FindAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId)).SingleOrDefaultAsync();
            if (gang == default(Gang))
                throw new DEAException("This gang does not exist.");
            return gang;
        }

        public async Task<Gang> CreateGangAsync(DEAContext context, string name)
        {
            Expression<Func<Gang, bool>> expression = x => x.Name.ToLower() == name.ToLower() && x.GuildId == context.Guild.Id;
            if (await (await _gangs.FindAsync(expression)).AnyAsync())
                throw new DEAException($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT)
                throw new DEAException($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");

            var createdGang = new Gang(context.User.Id, context.Guild.Id, name);
            await _gangs.InsertOneAsync(createdGang, null, default(CancellationToken));
            return createdGang;
        }

        public Task DestroyGangAsync(IGuildUser user)
        {
            return _gangs.DeleteOneAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId);
        }

        public async Task<bool> InGangAsync(IGuildUser user)
        {
            return await (await _gangs.FindAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId)).AnyAsync();
        }

        public Task<bool> IsMemberOfAsync(Gang gang, ulong userId)
        {
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId))
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }

        public Task RemoveMemberAsync(Gang gang, ulong memberId)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => c.Id == gang.Id, builder.Pull(x => x.Members, memberId));
        }

        public Task AddMemberAsync(Gang gang, ulong newMemberId)
        {
            var builder = Builders<Gang>.Update;
            return _gangs.UpdateOneAsync(c => c.Id == gang.Id, builder.Push(x => x.Members, newMemberId));
        }

    }
}