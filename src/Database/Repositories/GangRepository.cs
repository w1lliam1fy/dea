using DEA.Common;
using DEA.Database.Models;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Database.Repository
{
    public static class GangRepository
    {

        public static async Task ModifyAsync(DEAContext context, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && 
                                              c.GuildId == context.Guild.Id, builder.Set(field, value));
        }

        public static async Task ModifyAsync(IGuildUser member, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => (c.LeaderId == member.Id || c.Members.Any(x => x == member.Id)) && 
                                              c.GuildId == member.GuildId, builder.Set(field, value));
        }

        public static async Task ModifyAsync(IGuildUser member, Expression<Func<Gang, ulong>> field, ulong value)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => (c.LeaderId == member.Id || c.Members.Any(x => x == member.Id)) &&
                                              c.GuildId == member.GuildId, builder.Set(field, value));
        }

        public static async Task ModifyAsync(string gangName, ulong guildId, Expression<Func<Gang, BsonValue>> field, BsonValue value)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId, builder.Set(field, value));
        }

        public static async Task<Gang> FetchGangAsync(DEAContext context)
        {
            var gang = await (await DEABot.Gangs.FindAsync(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && c.GuildId == context.Guild.Id)).SingleOrDefaultAsync();
            if (gang == default(Gang)) throw new DEAException("You are not in a gang.");
            return gang;
        }

        public static async Task<Gang> FetchGangAsync(IGuildUser user)
        {
            var gang = await (await DEABot.Gangs.FindAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId)).SingleOrDefaultAsync();
            if (gang == default(Gang)) throw new DEAException("You are not in a gang.");
            return gang;
        }

        public static async Task<Gang> FetchGangAsync(string gangName, ulong guildId)
        {
            var gang = await (await DEABot.Gangs.FindAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId)).SingleOrDefaultAsync();
            if (gang == default(Gang)) throw new DEAException("You are not in a gang.");
            return gang;
        }

        public static async Task<Gang> CreateGangAsync(DEAContext context, string name)
        {
            Expression<Func<Gang, bool>> expression = x => x.Name.ToLower() == name.ToLower() && x.GuildId == context.Guild.Id;
            if (await (await DEABot.Gangs.FindAsync(expression)).AnyAsync()) throw new DEAException($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT) throw new DEAException($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");
            var createdGang = new Gang()
            {
                GuildId = context.Guild.Id,
                LeaderId = context.User.Id,
                Name = name 
            };
            await DEABot.Gangs.InsertOneAsync(createdGang, null, default(CancellationToken));
            return createdGang;
        }

        public static async Task DestroyGangAsync(IGuildUser user)
        {
            await DEABot.Gangs.DeleteOneAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId);
        }

        public static async Task<bool> InGangAsync(IGuildUser user)
        {
            return await (await DEABot.Gangs.FindAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId)).AnyAsync();
        }

        public static bool IsMemberOf(Gang gang, ulong userId)
        {
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId))
                return true;
            else
                return false;
        }

        public static async Task RemoveMemberAsync(Gang gang, ulong memberId)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => c.Id == gang.Id, builder.Pull(x => x.Members, memberId));
        }

        public static async Task AddMemberAsync(Gang gang, ulong newMemberId)
        {
            var builder = Builders<Gang>.Update;
            await DEABot.Gangs.UpdateOneAsync(c => c.Id == gang.Id, builder.Push(x => x.Members, newMemberId));
        }

    }
}