const util = require('../utility');
const db = require('../database');

module.exports = (client) => {
  client.on('guildMemberAdd', async (member) => {
    const dbGuild = await db.guildRepo.getGuild(member.guild.id);

    if (dbGuild.settings.welcomeMessage !== null) {
      await util.Messenger.tryDM(member, dbGuild.settings.welcomeMessage);
    }

    if (dbGuild.roles.muted !== null && await db.muteRepo.anyMute(member.id, member.guild.id)) {
      const role = member.guild.roles.get(dbGuild.roles.muted);

      if (role === undefined || !member.guild.me.hasPermission('MANAGE_ROLES')) {
        return;
      }

      if (role.position >= member.guild.me.highestRole.position) {
        return;
      }

      return member.addRole(role);
    }
  });
};
