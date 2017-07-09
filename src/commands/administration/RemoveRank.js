const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class RemoveRank extends patron.Command {
  constructor() {
    super({
      name: 'removerank',
      group: 'administration',
      description: 'Remove a rank role.',
      args: [
        new patron.Argument({
          name: 'role',
          key: 'role',
          type: 'role',
          example: 'Sicario',
          isRemainder: true
        })
      ]
    });
  }

  async run(context, args) {
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);

    if (!dbGuild.roles.rank.some((role) =>  role.id === args.role.id)) {
      return util.Messenger.replyError(context.channel, context.author, 'You may not remove a rank role that has no been set.');
    }

    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Pull('roles.rank', { id: args.role.id }));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully removed the rank role ' + args.role + '.');
  }
}

module.exports = new RemoveRank();
