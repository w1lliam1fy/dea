using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEA
{
    public static class RankHandler
    {

        public static bool CheckRankCompletion(Guild guild) { // maybe these things should throw errors themselves rather than return values
            foreach (ulong rankid in guild.RankIds) 
                if (rankid <= 0)
                    return false;

            return true;
        }

        public static bool CheckRankExistance(Guild guild, SocketGuild socketGuild) {
            if (!CheckRankCompletion(guild))
                return false;
            
            foreach (ulong rankid in guild.RankIds) 
                if (socketGuild.GetRole(rankid) == null)
                    return false;

            return true;
        }

        public static bool CheckRankOverlap(Guild guild, ulong id) {
            foreach (ulong rankid in guild.RankIds) 
                if (rankid == id)
                    return false;

            return true;
        }

        public static async Task<IRole> GetRank(IGuild guild, ulong userId, ulong guildId) {
            using (var db = new DbContext()) {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var cguild = await guildRepo.FetchGuildAsync(guildId);
                float cash = await userRepo.GetCashAsync(userId);

                int i = -1;
                foreach (float cost in Config.RANKS) {
                    if (cash < cost)
                        break;
                    i++;
                }

                if (i >= 0 && i < cguild.RankIds.Length) {
                    if (cguild.RankIds[i] > 0) {
                        return guild.GetRole(cguild.RankIds[i]);
                    } else {
                        throw new Exception($"The {i + 1}th role does not yet exist. Set it with `$SetRankRoles {i + 1}`");
                    }
                } else if (i == -1) {
                    return null;
                } else {
                    throw new Exception($"Rank {i+1} exists in the config but not in your guild.\n```diff\n" +
                                        $"- This is an error with DEA itself and is not your fault\n```\n" +
                                        $"Please report this error to https://github.com/RealBlazeIt/DEA/issues"); // Fixing this: add it to guild.cs
                }
            }
        }
        public static async Task Handle(IGuild guild, ulong userId)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                float cash = await userRepo.GetCashAsync(userId);
                var user = await guild.GetUserAsync(userId); //FETCHES THE USER
                var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER
                var guildData = await guildRepo.FetchGuildAsync(guild.Id); //FETCHES THE GUILD DATA
                /*var role1 = guild.GetRole(guildData.Rank1Id); //FETCHES ALL RANK ROLES
                var role2 = guild.GetRole(guildData.Rank2Id);
                var role3 = guild.GetRole(guildData.Rank3Id);
                var role4 = guild.GetRole(guildData.Rank4Id);*/
                var sponsorRole = guild.GetRole(Config.SPONSORED_ROLE_ID);
                List<IRole> rolesToAdd = new List<IRole>();
                List<IRole> rolesToRemove = new List<IRole>();
                if (guild != null && user != null)
                {
                    //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                    for(int i = 0; i < Config.RANKS.Length; i++) { 
                        var role1 = guild.GetRole(guildData.RankIds[i]);
                        if (role1 != null && role1.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position) {
                            if (cash >= Config.RANKS[i] && !user.RoleIds.Any(x => x == role1.Id)) rolesToAdd.Add(role1);
                            if (cash <  Config.RANKS[i] && user.RoleIds.Any(x => x == role1.Id)) rolesToRemove.Add(role1);
                        }
                    }
                    if (sponsorRole != null && sponsorRole.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (Config.SPONSOR_IDS.Any(x => x == user.Id) && !user.RoleIds.Any(x => x == sponsorRole.Id)) rolesToAdd.Add(sponsorRole);
                        if (!Config.SPONSOR_IDS.Any(x => x == user.Id) && user.RoleIds.Any(x => x == sponsorRole.Id)) rolesToRemove.Add(sponsorRole);
                    }   
                    if (rolesToAdd.Count >= 1)
                        await user.AddRolesAsync(rolesToAdd);
                    if (rolesToRemove.Count >= 1)
                        await user.RemoveRolesAsync(rolesToRemove);
                }
            }
        }
    }
}