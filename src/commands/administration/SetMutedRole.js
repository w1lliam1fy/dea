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
          remainder: true
        })
      ]
    });
  }

  async run(context, args) {
    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Set('roles.muted', args.role.id));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully set the muted role to ' + args.role + '.');
  }
}

module.exports = new SetMutedRole();
