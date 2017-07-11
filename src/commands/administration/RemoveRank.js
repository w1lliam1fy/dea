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

  async run(msg, args) {
    if (!msg.dbGuild.roles.rank.some((role) =>  role.id === args.role.id)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'You may not remove a rank role that has no been set.');
    }

    await db.guildRepo.upsertGuild(msg.guild.id, new db.updates.Pull('roles.rank', { id: args.role.id }));

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully removed the rank role ' + args.role + '.');
  }
}

module.exports = new RemoveRank();
