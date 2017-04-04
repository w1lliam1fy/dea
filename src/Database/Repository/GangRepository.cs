using DEA.Database.Models;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Database.Repository
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
            using (var db = new DEAContext())
            {
                var gang = await db.Gangs.FirstAsync(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x.UserId == context.User.Id))
                                           && c.Guild.Id == context.Guild.Id);
                if (gang == null) throw new Exception("You are not in a gang.");
                return gang;
            }  
        }

        public static async Task<Gang> FetchGangAsync(ulong userId, ulong guildId)
        {
            using (var db = new DEAContext())
            {
                var gang = await db.Gangs.FirstAsync(c => (c.LeaderId == userId || c.Members.Any(x => x.UserId == userId)) && c.Guild.Id == guildId);
                if (gang == null) throw new Exception("This user is not in a gang.");
                return gang;
            }
        }

        public static async Task<Gang> FetchGangAsync(string gangName, ulong guildId)
        {
            using (var db = new DEAContext())
            {
                var gang = await db.Gangs.FirstAsync(c => c.Name.ToLower() == gangName.ToLower() && c.Guild.Id == guildId);
                if (gang == null) throw new Exception("This gang does not exist.");
                return gang;
            }
        }

        public static async Task<Gang> CreateGangAsync(ulong leaderId, ulong guildId, string name)
        {
            var guild = await GuildRepository.FetchGuildAsync(guildId);
            if (guild.Gangs.Any(x => x.Name.ToLower() == name.ToLower() && x.Guild.Id == guildId)) throw new Exception($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT) throw new Exception($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");
            var createdGang = new Gang()
            {
                LeaderId = leaderId,
                Name = name,
                Guild = guild
            };
            guild.Gangs.Add(createdGang);
            await BaseRepository<Guild>.UpdateAsync(guild);
            return createdGang;
        }

        public static async Task<Gang> DestroyGangAsync(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            await BaseRepository<Gang>.DeleteAsync(gang);
            return gang;
        }

        public static async Task<bool> InGangAsync(ulong userId, ulong guildId)
        {
            using (var db = new DEAContext())
            {
                return await db.Gangs.AnyAsync(c => (c.LeaderId == userId || c.Members.Any(x => x.UserId == userId)) && c.Guild.Id == guildId);
            }
            
        }

        public static async Task<bool> IsMemberOfAsync(ulong memberId, ulong guildId, ulong userId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            if (gang.LeaderId == userId || gang.Members.Any(x => x.UserId == userId)) return true;
            return false;
        }

        public static async Task<bool> IsFullAsync(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            if (gang.Members.Count == 4) return true;
            return false;
        }

        public static async Task RemoveMemberAsync(ulong memberId, ulong guildId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            var user = await UserRepository.FetchUserAsync(memberId, guildId);
            gang.Members.Remove(user);
            await BaseRepository<Gang>.UpdateAsync(gang);
        }

        public static async Task AddMemberAsync(ulong userId, ulong guildId, ulong newMemberId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            var user = await UserRepository.FetchUserAsync(userId, guildId);
            gang.Members.Add(user);
            await BaseRepository<Gang>.UpdateAsync(gang);
        }

        public static async Task<ICollection<Gang>> AllAsync(ulong guildId)
        {
            return (await GuildRepository.FetchGuildAsync(guildId)).Gangs;
        }
    }
}