using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEA
{
    public static class RankHandler
    {
        public static async Task Handle(IGuild guild, ulong userId)
        {
            if (!((await guild.GetCurrentUserAsync()).GuildPermissions.ManageRoles)) return;
            double cash = (await UserRepository.FetchUserAsync(userId, guild.Id)).Cash;
            var user = await guild.GetUserAsync(userId); //FETCHES THE USER
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER
            var guildData = await GuildRepository.FetchGuildAsync(guild.Id);
            List<IRole> rolesToAdd = new List<IRole>();
            List<IRole> rolesToRemove = new List<IRole>();
            if (guild != null && user != null)
            {
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                foreach (var rankRole in guildData.RankRoles)
                {
                    var role = guild.GetRole(rankRole.Key);
                    if (role != null && role.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= rankRole.Value && !user.RoleIds.Any(x => x == rankRole.Key)) rolesToAdd.Add(role);
                        if (cash < rankRole.Value && user.RoleIds.Any(x => x == rankRole.Key)) rolesToRemove.Add(role);
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