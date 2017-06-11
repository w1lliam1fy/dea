using DEA.Common;
using DEA.Database.Models;
using Discord;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Database.Repositories
{
    public sealed class GangRepository : BaseRepository<Gang>
    {
        public GangRepository(IMongoCollection<Gang> gangs) : base(gangs) { }

        public Task ModifyGangAsync(IGuildUser member, Action<Gang> function)
        {
            return ModifyAsync(c => (c.LeaderId == member.Id || c.Members.Any(x => x == member.Id)) && c.GuildId == member.GuildId, function);
        }

        public Task ModifyGangAsync(string gangName, ulong guildId, Action<Gang> function)
        {
            return ModifyAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId, function);
        }

        public async Task<Gang> GetGangAsync(IGuildUser user)
        {
            var gang = await GetAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId);
            return gang;
        }

        public async Task<Gang> GetGangAsync(string gangName, ulong guildId)
        {
            var gang = await GetAsync(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId);
            return gang;
        }

        public async Task<Gang> CreateGangAsync(Context context, string name)
        {
            var createdGang = new Gang(context.User.Id, context.Guild.Id, name);
            await InsertAsync(createdGang);
            return createdGang;
        }

        public Task DestroyGangAsync(IGuildUser user)
        {
            return DeleteAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId);
        }

        public Task<bool> InGangAsync(IGuildUser user)
        {
            return AnyAsync(c => (c.LeaderId == user.Id || c.Members.Any(x => x == user.Id)) && c.GuildId == user.GuildId);
        }
        
        public bool IsMemberOfAsync(Gang gang, ulong userId)
        {
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task RemoveMemberAsync(Gang gang, ulong memberId)
        {
            return PullAsync(c => c.Id == gang.Id, "Members", memberId);
        }

        public Task AddMemberAsync(Gang gang, ulong newMemberId)
        {
            return PushAsync(c => c.Id == gang.Id, "Members", newMemberId);
        }

    }
}