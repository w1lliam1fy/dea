using DEA.SQLite.Models;
using Discord.Commands;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEA.SQLite.Repository
{
    public static class GangRepository
    {

        public static void Modify(Action<Gang> function, SocketCommandContext context)
        {
            var gang = FetchGang(context);
            function(gang);
            UpdateGang(gang);
        }

        public static void Modify(Action<Gang> function, ulong userId, ulong guildId)
        {
            var gang = FetchGang(userId, guildId);
            function(gang);
            UpdateGang(gang);
        }

        public static Gang FetchGang(SocketCommandContext context)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                var gang = gangs.FindOne(x => x.GuildId == context.Guild.Id && (x.LeaderId == context.User.Id || x.Members.Any(y => y == context.User.Id)));
                if (gang == null) throw new Exception("This user is not in a gang..");
                return gang;
            }
        }

        public static Gang FetchGang(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                var gang = gangs.FindOne(x => x.GuildId == guildId && (x.LeaderId == userId || x.Members.Any(y => y == userId)));
                if (gang == null) throw new Exception("This user is not in a gang..");
                return gang;
            }
        }

        public static Gang FetchGang(string name, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                var gang = gangs.FindOne(x => x.GuildId == guildId && x.Name.ToLower() == name.ToLower());
                if (gang == null) throw new Exception("This user is not in a gang.");
                return gang;
            }
        }

        public static Gang CreateGang(ulong leaderId, ulong guildId, string name)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                if (gangs.FindOne(x => x.Name.ToLower() == name.ToLower() && x.GuildId == guildId) == null)
                    throw new Exception($"A gang already exists by the name of {name}");
                var gang = new Gang()
                {
                    LeaderId = leaderId,
                    GuildId = guildId,
                    Name = name
                };
                gangs.Insert(gang);
                return gang;
            }
        }

        public static Gang DestroyGang(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                var gang = FetchGang(userId, guildId);
                gangs.Delete(x => x.GuildId == guildId && (x.LeaderId == userId || x.Members.Any(y => y == userId)));
                return gang;
            }
        }

        public static bool InGang(ulong userId, ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                var gang = gangs.FindOne(x => x.GuildId == guildId && (x.LeaderId == userId || x.Members.Any(y => y == userId)));
                if (gang == null)
                    return false;
                else
                    return true;
            }
        }

        public static bool IsMemberOf(ulong memberId, ulong guildId, ulong userId)
        {
            var gang = FetchGang(memberId, guildId);
            if (gang.LeaderId == userId || gang.Members.Any(x => x == userId)) return true;
            return false;
        }

        public static bool IsFull(ulong userId, ulong guildId)
        {
            var gang = FetchGang(userId, guildId);
            if (gang.Members.Count == 4)
                return true;
            else
                return false;
        }

        public static void RemoveMember(ulong memberId, ulong guildId)
        {
            var gang = FetchGang(memberId, guildId);
            gang.Members.Remove(memberId);
            UpdateGang(gang);
        }

        public static void AddMember(ulong userId, ulong guildId, ulong newMemberId)
        {
            var gang = FetchGang(userId, guildId);
            gang.Members.Add(newMemberId);
            UpdateGang(gang);
        }

        public static IEnumerable<Gang> FetchAll(ulong guildId)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var users = db.GetCollection<Gang>("Gangs");
                return users.Find(x => x.GuildId == guildId);
            }
        }

        private static void UpdateGang(Gang gang)
        {
            using (var db = new LiteDatabase(Config.DB_CONNECTION_STRING))
            {
                var gangs = db.GetCollection<Gang>("Gangs");
                gangs.Update(gang);
            }
        }

    }
}
