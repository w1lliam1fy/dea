const util = require('../utility');

class RankService {
  handle(dbUser, dbGuild, member) {
    if (dbGuild.roles.rank.length === 0) {
      return;
    } else if (!member.guild.me.hasPermission('MANAGE_ROLES')) {
      return;
    } else if (member.highestRole.id === member.guild.defaultRole.id) {
      return;
    }

    const highsetRolePosition = member.guild.me.highestRole.position;
    const rolesToAdd = [];
    const rolesToRemove = [];
    const cash = util.NumberUtil.realValue(dbUser.cash);

    for (const rank of dbGuild.roles.rank) {
      const role = member.guild.roles.get(rank.id);

      if (role !== undefined && role.position < highsetRolePosition) {
        const hasRole = member.roles.get(role.id) !== undefined;

        if (cash >= rank.cashRequired && !hasRole) {
          rolesToAdd.push(role);
        } else if (cash < rank.cashRequired && hasRole) {
          rolesToRemove.push(role);
        }
      }
    }

    if (rolesToAdd.length > 0) {
      return member.addRoles(rolesToAdd);
    } else if (rolesToRemove.length > 0) {
      return member.removeRoles(rolesToRemove);
    }
  }

  getRank(dbUser, dbGuild, guild) {
    let role;
    const cash = util.NumberUtil.realValue(dbUser.cash);

    for (const rank of dbGuild.roles.rank.sort((a, b) => a.cashRequired - b.cashRequired)) {
      if (cash >= rank.cashRequired) {
        role = guild.roles.get(rank.id);
      }
    }

    return role;
  }
}

module.exports = new RankService();
