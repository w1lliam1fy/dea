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
    public class RankingService
    {
        private GuildRepository _guildRepo;

        public RankingService(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        public async Task HandleAsync(IGuild guild, IGuildUser user, Guild dbGuild, User dbUser)
        {
            var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser;
            if (!currentUser.GuildPermissions.ManageRoles) return;

            decimal cash = dbUser.Cash;

            List<IRole> rolesToAdd = new List<IRole>();
            List<IRole> rolesToRemove = new List<IRole>();

            if (user != null && dbGuild.RankRoles.ElementCount != 0)
            {
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                foreach (var rankRole in dbGuild.RankRoles)
                {
                    var role = guild.GetRole(Convert.ToUInt64(rankRole.Name));
                    if (role != null && role.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= (decimal)rankRole.Value.AsDouble && !user.RoleIds.Any(x => x.ToString() == rankRole.Name)) rolesToAdd.Add(role);
                        if (cash < (decimal)rankRole.Value.AsDouble && user.RoleIds.Any(x => x.ToString() == rankRole.Name)) rolesToRemove.Add(role);
                    }
                    else
                    {
                        dbGuild.RankRoles.Remove(rankRole.Name);
                        await _guildRepo.ModifyAsync(guild.Id, x => x.RankRoles, dbGuild.RankRoles);
                    }
                }

                if (rolesToAdd.Count >= 1)
                    await user.AddRolesAsync(rolesToAdd);
                else if (rolesToRemove.Count >= 1)
                    await user.RemoveRolesAsync(rolesToRemove);
            }
        }

        public Task<IRole> FetchRankAsync(DEAContext context, User dbUser)
        {
            IRole role = null;

            if (context.DbGuild.RankRoles.ElementCount != 0 && context.Guild != null)
                foreach (var rankRole in context.DbGuild.RankRoles.OrderBy(x => x.Value))
                    if (dbUser.Cash >= (decimal)rankRole.Value.AsDouble)
                        role = context.Guild.GetRole(Convert.ToUInt64(rankRole.Name));
            return Task.FromResult(role);
        }
    }
}