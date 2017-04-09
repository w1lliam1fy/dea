using DEA.Database.Models;
using DEA.Resources;
using Discord.Commands;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DEA.Database.Repository
{
    public static class GangRepository
    {

        public static void Modify(UpdateDefinition<Gang> update, SocketCommandContext context)
        {
            FetchGang(context);
            DEABot.Gangs.UpdateOne(c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && c.GuildId == context.Guild.Id, update);
        }

        public static void Modify(UpdateDefinition<Gang> update, ulong memberId, ulong guildId)
        {
            FetchGang(memberId, guildId);
            DEABot.Gangs.UpdateOne(c => (c.LeaderId == memberId || c.Members.Any(x => x == memberId)) && c.GuildId == guildId, update);
        }

        public static void Modify(UpdateDefinition<Gang> update, string gangName, ulong guildId)
        {
            FetchGang(gangName, guildId);
            DEABot.Gangs.UpdateOne(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId, update);
        }

        public static Gang FetchGang(SocketCommandContext context)
        {
            Expression<Func<Gang, bool>> expression = c => (c.LeaderId == context.User.Id || c.Members.Any(x => x == context.User.Id)) && c.GuildId == context.Guild.Id;
            if (!DEABot.Gangs.Find(expression).Limit(1).Any()) throw new DEAException("You are not in a gang.");
            return DEABot.Gangs.Find(expression).Limit(1).First();
        }

        public static Gang FetchGang(ulong userId, ulong guildId)
        {
            Expression<Func<Gang, bool>> expression = c => (c.LeaderId == userId || c.Members.Any(x => x == userId)) && c.GuildId == guildId;
            if (!DEABot.Gangs.Find(expression).Limit(1).Any()) throw new DEAException("This user is not in a gang.");
            return DEABot.Gangs.Find(expression).Limit(1).First();
        }

        public static Gang FetchGang(string gangName, ulong guildId)
        {
            Expression<Func<Gang, bool>> expression = c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId;
            if (!DEABot.Gangs.Find(expression).Limit(1).Any()) throw new DEAException("This gang does not exist.");
            return DEABot.Gangs.Find(expression).Limit(1).First();
        }

        public static Gang CreateGang(ulong leaderId, ulong guildId, string name)
        {
            Expression<Func<Gang, bool>> expression = x => x.Name.ToLower() == name.ToLower() && x.GuildId == guildId;
            if (DEABot.Gangs.Find(expression).Limit(1).Any()) throw new DEAException($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT) throw new DEAException($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");
            var createdGang = new Gang()
            {
                GuildId = guildId,
                LeaderId = leaderId,
                Name = name 
            };
            DEABot.Gangs.InsertOne(createdGang);
            return createdGang;
        }

        public static void DestroyGang(ulong userId, ulong guildId)
        {
            DEABot.Gangs.DeleteOne(c => (c.LeaderId == userId || c.Members.Any(x => x == userId)) && c.GuildId == guildId);
        }

        public static bool InGang(ulong userId, ulong guildId)
        {
            return DEABot.Gangs.Find(c => (c.LeaderId == userId || c.Members.Any(x => x == userId)) && c.GuildId == guildId).Limit(1).Any();
        }

        public static bool IsMemberOf(ulong memberId, ulong guildId, ulong userId)
        {
            var gang = FetchGang(memberId, guildId);
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId))
                return true;
            else
                return false;
        }

        public static bool IsFull(ulong userId, ulong guildId)
        {
            var gang = FetchGang(userId, guildId);
            foreach (var member in gang.Members)
                if (member == 0) return false;
            return true;
        }

        public static void RemoveMember(ulong memberId, ulong guildId)
        {
            var gang = FetchGang(memberId, guildId);
            for (int i = 0; i < gang.Members.Length; i++)
                if (gang.Members[i] == memberId)
                {
                    gang.Members[i] = 0;
                    Modify(DEABot.GangUpdateBuilder.Set(x => x.Members, gang.Members), memberId, guildId);
                    break;
                }
        }

        public static void AddMember(ulong userId, ulong guildId, ulong newMemberId)
        {
            var gang = FetchGang(userId, guildId);
            for (int i = 0; i < gang.Members.Length; i++)
                if (gang.Members[i] == 0)
                {
                    gang.Members[i] = newMemberId;
                    Modify(DEABot.GangUpdateBuilder.Set(x => x.Members, gang.Members), userId, guildId);
                    break;
                }
        }

    }
}