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

  async run(context, args) {
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);
    const modRoleList = dbGuild.roles.mod.sort((a, b) => a.permissionLevel - b.permissionLevel);

    if (dbGuild.roles.mod.length === 0) {
      return util.Messenger.replyError(context.channel, context.author, 'There are no mod roles yet!');
    }

    let description = '';
    for(let i = 0; i < modRoleList.length; i++) {
      const rank = context.guild.roles.find((x) => x.id === modRoleList[i].id);
      console.log(modRoleList[i]);
      description+= util.StringUtil.boldify(rank.name) + ', Permission Level: ' + (modRoleList[i].permissionLevel) + '.\n';
    }

    return util.Messenger.reply(context.channel, context.author, description);
  }
}

module.exports = new ModRoles();
