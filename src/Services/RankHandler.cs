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
        public static async Task Handle(IGuild guild, ulong userId)
        {
            PrettyConsole.NewLine("1");
            if (!((await guild.GetCurrentUserAsync()).GuildPermissions.ManageRoles)) return;
            PrettyConsole.NewLine("2");
            double cash = UserRepository.FetchUser(userId, guild.Id).Cash;
            PrettyConsole.NewLine("3");
            var user = await guild.GetUserAsync(userId); //FETCHES THE USER
            PrettyConsole.NewLine("4");
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER
            PrettyConsole.NewLine("5");
            var guildData = GuildRepository.FetchGuild(guild.Id);
            PrettyConsole.NewLine("6");
            List<IRole> rolesToAdd = new List<IRole>();
            PrettyConsole.NewLine("7");
            List<IRole> rolesToRemove = new List<IRole>();
            PrettyConsole.NewLine("8");
            if (guild != null && user != null)
            {
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                foreach (var rankRole in guildData.Roles.RankRoles)
                {
                    PrettyConsole.NewLine("9");
                    var role = guild.GetRole(rankRole.Id);
                    if (role != null && role.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        PrettyConsole.NewLine("10");
                        if (cash >= rankRole.CashRequired && !user.RoleIds.Any(x => x == rankRole.Id)) rolesToAdd.Add(role);
                        if (cash < rankRole.CashRequired && user.RoleIds.Any(x => x == rankRole.Id)) rolesToRemove.Add(role);
                        PrettyConsole.NewLine("11");
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