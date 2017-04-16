using DEA.Common;
using DEA.Database.Models;
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
        public static async Task HandleAsync(IGuild guild, ulong userId)
        {
            if (!((await guild.GetCurrentUserAsync()).GuildPermissions.ManageRoles)) return;
            decimal cash = (await UserRepository.FetchUserAsync(userId, guild.Id)).Cash;
            var guildData = await GuildRepository.FetchGuildAsync(guild.Id);

            var user = await guild.GetUserAsync(userId); //FETCHES THE USER
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER

            List<IRole> rolesToAdd = new List<IRole>();
            List<IRole> rolesToRemove = new List<IRole>();

            if (user != null && guildData.RankRoles.ElementCount != 0)
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
                        var builder = Builders<Guild>.Update;
                        await DEABot.Guilds.UpdateOneAsync(x => x.Id == guild.Id, builder.Set(x => x.RankRoles, guildData.RankRoles));
                    }
                }

                if (rolesToAdd.Count >= 1)
                    await user.AddRolesAsync(rolesToAdd);
                else if (rolesToRemove.Count >= 1)
                    await user.RemoveRolesAsync(rolesToRemove);
            }
        }

        public static IRole FetchRankAsync(DEAContext context, User dbUser)
        {
            IRole role = null;

            if (context.DbGuild.RankRoles.ElementCount != 0 && context.Guild != null)
                foreach (var rankRole in context.DbGuild.RankRoles.OrderBy(x => x.Value))
                    if (context.Cash >= (decimal)rankRole.Value.AsDouble)
                        role = context.Guild.GetRole(Convert.ToUInt64(rankRole.Name));
            return role;
        }
    }
}