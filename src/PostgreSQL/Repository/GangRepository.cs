using DEA.SQLite.Models;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public static class GangRepository
    {

        public static async Task ModifyAsync(Func<Gang, Task> function, SocketCommandContext context)
        {
            var gang = await FetchGangAsync(context.User.Id, context.Guild.Id);
            await function(gang);
            await BaseRepository<Gang>.UpdateAsync(gang);
        }

        public static async Task ModifyAsync(Func<Gang, Task> function, ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            await function(gang);
            await BaseRepository<Gang>.UpdateAsync(gang);
        }

        public static async Task<Gang> FetchGangAsync(SocketCommandContext context)
        {
            var gang = await BaseRepository<Gang>.SearchFor(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) 
                                       && c.GuildId == context.Guild.Id).FirstOrDefaultAsync();
            if (gang == null) throw new Exception("This user is not in a gang.");
            return gang;
        }

        public static async Task<Gang> FetchGangAsync(ulong userId, ulong guildId)
        {
            var gang = await BaseRepository<Gang>.SearchFor(c => (c.LeaderId == userId || c.Members.Any(x => x == userId)) && c.GuildId == guildId).FirstOrDefaultAsync();
            if (gang == null) throw new Exception("This user is not in a gang.");
            return gang;
        }

        public static async Task<Gang> FetchGangAsync(string gangName, ulong guildId)
        {
            var gang = await BaseRepository<Gang>.SearchFor(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId).FirstOrDefaultAsync();
            if (gang == null) throw new Exception("This user is not in a gang.");
            return gang;
        }

        public static async Task<Gang> CreateGangAsync(ulong leaderId, ulong guildId, string name)
        {
            if (await BaseRepository<Gang>.GetAll().AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == guildId)) throw new Exception($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT) throw new Exception($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");
            var CreatedGang = new Gang()
            {
                GuildId = guildId,
                LeaderId = leaderId,
                Name = name
            };
            await BaseRepository<Gang>.InsertAsync(CreatedGang);
            return CreatedGang;
        }

        public static async Task<Gang> DestroyGangAsync(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            await BaseRepository<Gang>.DeleteAsync(gang);
            return gang;
        }

        public static async Task<bool> InGangAsync(ulong userId, ulong guildId)
        {
            return await BaseRepository<Gang>.SearchFor(c => (c.LeaderId == userId || c.Members.Any(x => x == userId)) && c.GuildId == guildId).AnyAsync();
        }

        public static async Task<bool> IsMemberOfAsync(ulong memberId, ulong guildId, ulong userId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId)) return true;
            return false;
        }

        public static async Task<bool> IsFullAsync(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            if (gang.Members.Count == 4) return true;
            return false;
        }

        public static async Task<bool> RemoveMemberAsync(ulong memberId, ulong guildId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            var result = gang.Members.Remove(memberId);
            await BaseRepository<Gang>.UpdateAsync(gang);
            return result;
        }

        public static async Task AddMemberAsync(ulong userId, ulong guildId, ulong newMemberId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            gang.Members.Add(newMemberId);
            await BaseRepository<Gang>.UpdateAsync(gang);
        }

        public static async Task<List<Gang>> AllAsync(ulong guildId)
        {
            return (await BaseRepository<Gang>.GetAll().ToListAsync()).FindAll(x => x.GuildId == guildId);
        }
    }
}