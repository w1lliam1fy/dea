const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class ModRoles extends patron.Command {
  constructor() {
    super({
      name: 'modroles',
      group: 'general',
      description: 'View all mod roles in this guild.'
    });
  }

  async run(msg, args) {
    const dbGuild = await db.guildRepo.getGuild(msg.guild.id);
    const modRoleList = dbGuild.roles.mod.sort((a, b) => a.permissionLevel - b.permissionLevel);

    if (dbGuild.roles.mod.length === 0) {
      return util.Messenger.replyError(msg.channel, msg.author, 'There are no mod roles yet!');
    }

    let description = '';
    for (let i = 0; i < modRoleList.length; i++) {
      const rank = msg.guild.roles.find((x) => x.id === modRoleList[i].id);

      description+= rank + ': ' + (modRoleList[i].permissionLevel) + '\n';
    }

    return util.Messenger.send(context.channel, description + '\n**Permission Levels:**\n1: Moderator\n2: Administrator\n3: Owner', 'Mod Roles.');
  }
}

module.exports = new ModRoles();
