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

        public static bool CheckRankExistance(Guild guild, SocketGuild socketGuild) {
            if (!CheckRankCompletion(guild))
                return false;
            
            foreach (var rankRole in guild.Roles.RankRoles) 
                if (socketGuild.GetRole(rankRole.Id) == null)
                    return false;

            return true;
        }

        public static bool CheckRankOverlap(Guild guild, ulong id) {
            foreach (var rankRole in guild.Roles.RankRoles) 
                if (rankRole.Id == id)
                    return false;

            return true;
        }

        public static async Task<IRole> GetRank(IGuild guild, ulong userId, ulong guildId) {
            var cguild = GuildRepository.FetchGuild(guildId);
            double cash = UserRepository.FetchUser(userId).Cash;

            int i = -1;
            foreach (double cost in Config.RANKS)
            {
                if (cash < cost)
                    break;
                i++;
            }

            if (i >= 0 && i < cguild.RankIds.Length)
            {
                if (cguild.RankIds[i] > 0)
                {
                    return guild.GetRole(cguild.RankIds[i]);
                }
                else
                {
                    throw new Exception($"The {i + 1}th role does not yet exist. Set it with `$SetRankRoles {i + 1}`");
                }
            }
            else if (i == -1)
            {
                return null;
            }
            else
            {
                throw new Exception($"Rank {i + 1} exists in the config but not in your guild.\n```diff\n" +
                                    $"- This is an error with DEA itself and is not your fault\n```\n" +
                                    $"Please report this error to https://github.com/RealBlazeIt/DEA/issues"); // Fixing this: add it to guild.cs
            }
        }
        public static async Task Handle(IGuild guild, ulong userId)
        {
            if (!((await guild.GetCurrentUserAsync()).GuildPermissions.ManageRoles)) return;
            double cash = UserRepository.FetchUser(userId).Cash;
            var user = await guild.GetUserAsync(userId); //FETCHES THE USER
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER
            var guildData = GuildRepository.FetchGuild(guild.Id); 
            List<IRole> rolesToAdd = new List<IRole>();
            List<IRole> rolesToRemove = new List<IRole>();
            if (guild != null && user != null)
            {
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                foreach (var rankRole in guildData.Roles.RankRoles)
                {
                    var role = guild.GetRole(rankRole.Id);
                    if (role != null && role.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= rankRole.CashRequired && !user.RoleIds.Any(x => x == rankRole.Id)) rolesToAdd.Add(role);
                        if (cash < rankRole.CashRequired && user.RoleIds.Any(x => x == rankRole.Id)) rolesToRemove.Add(role);
                    }
                }
                if (rolesToAdd.Count >= 1)
                    await user.AddRolesAsync(rolesToAdd);
                if (rolesToRemove.Count >= 1)
                    await user.RemoveRolesAsync(rolesToRemove);
            }
        }
    }
}