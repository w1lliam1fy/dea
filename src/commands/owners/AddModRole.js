const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class AddModRole extends patron.Command {
  constructor() {
    super({
      name: 'addmodrole',
      aliases: ['addmod', 'setmod'],
      group: 'owners',
      description: 'Add a mod role.',
      args: [
        new patron.Argument({
          name: 'role',
          key: 'role',
          type: 'role',
          example: 'Moderator'
        }),
        new patron.Argument({
          name: 'permissonLevel',
          key: 'permissionLevel',
          type: 'float',
          example: '2'
        })
      ]
    });
  }

  async run(context, args){
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);

    if (args.permissionLevel < 1 || args.permissionLevel > 3) {
      return util.Messenger.replyError(context.channel, context.author, 'Permission levels:\nModerator: 1\nAdministrator: 2\nOwner: 3');
    }

    if (dbGuild.roles.mod.some((role) =>  role.id === args.role.id)) {
      return util.Messenger.replyError(context.channel, context.author, 'This moderation role has already been set.');
    }

    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Push('roles.mod', { id: args.role.id, permissionLevel: args.permissionLevel }));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully added the mod role ' + args.role + ' with a permission level of ' + args.permissionLevel + '.');
  }
}

module.exports = new AddModRole();
