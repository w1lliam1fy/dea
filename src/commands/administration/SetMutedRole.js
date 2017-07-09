const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');

class SetMutedRole extends patron.Command {
  constructor() {
    super({
      name: 'setmutedrole',
      aliases: ['setmuterole', 'setmute', 'setmuted'],
      group: 'administration',
      description: 'Sets the muted role.',
      args: [
        new patron.Argument({
          name: 'Muted Role',
          key: 'role',
          type: 'role',
          example: 'Muted',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    await db.guildRepo.upsertGuild(msg.guild.id, new db.updates.Set('roles.muted', args.role.id));

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully set the muted role to ' + args.role + '.');
  }
}

module.exports = new SetMutedRole();
