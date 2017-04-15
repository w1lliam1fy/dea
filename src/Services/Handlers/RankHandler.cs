using DEA.Database.Repository;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services.Handlers
{
    public static class RankHandler
    {
        public static async Task Handle(IGuild guild, ulong userId)
        {
            if (!((await guild.GetCurrentUserAsync()).GuildPermissions.ManageRoles)) return;
            decimal cash = UserRepository.FetchUser(userId, guild.Id).Cash;
            var guildData = GuildRepository.FetchGuild(guild.Id);

            var user = await guild.GetUserAsync(userId); //FETCHES THE USER
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER

            List<IRole> rolesToAdd = new List<IRole>();
            List<IRole> rolesToRemove = new List<IRole>();

            if (guild != null && user != null && guildData.RankRoles.ElementCount != 0)
            {
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                foreach (var rankRole in guildData.RankRoles)
                {
                    var role = guild.GetRole(Convert.ToUInt64(rankRole.Name));
                    if (role != null && role.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= (decimal)rankRole.Value.AsDouble && !user.RoleIds.Any(x => x.ToString() == rankRole.Name)) rolesToAdd.Add(role);
                        if (cash < (decimal)rankRole.Value.AsDouble && user.RoleIds.Any(x => x.ToString() == rankRole.Name)) rolesToRemove.Add(role);
                    }
                    else
                    {
                        guildData.RankRoles.Remove(rankRole.Name);
                        await DEABot.Guilds.UpdateOneAsync(x => x.Id == guild.Id, DEABot.GuildUpdateBuilder.Set(x => x.RankRoles, guildData.RankRoles));
                    }
                }

                if (rolesToAdd.Count >= 1)
                    await user.AddRolesAsync(rolesToAdd);
                else if (rolesToRemove.Count >= 1)
                    await user.RemoveRolesAsync(rolesToRemove);
            }
        }

        public static IRole FetchRank(ulong userId, ulong guildId)
        {
            var dbGuild = GuildRepository.FetchGuild(guildId);
            var cash = UserRepository.FetchUser(userId, guildId).Cash;

            IRole role = null;
            IGuild guild = DEABot.Client.GetGuild(guildId);

            if (dbGuild.RankRoles.ElementCount != 0 && guild != null)
                foreach (var rankRole in dbGuild.RankRoles.OrderBy(x => x.Value))
                    if (cash >= (decimal)rankRole.Value.AsDouble)
                        role = guild.GetRole(Convert.ToUInt64(rankRole.Name));
            return role;
        }
    }
}