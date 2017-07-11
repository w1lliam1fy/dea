const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class AddRank extends patron.Command {
  constructor() {
    super({
      name: 'addrank',
      aliases: ['setrank'],
      group: 'administration',
      description: 'Add a rank.',
      args: [
        new patron.Argument({
          name: 'role',
          key: 'role',
          type: 'role',
          example: 'Sicario'
        }),
        new patron.Argument({
          name: 'cashRequired',
          key: 'cashRequired',
          type: 'float',
          example: '500'
        })
      ]
    });
  }



    await db.guildRepo.upsertGuild(msg.guild.id, new db.updates.Push('roles.rank', { id: args.role.id, cashRequired: Math.round(args.cashRequired) }));

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully added the rank role ' + args.role + ' with a cash required amount of ' + args.cashRequired + '.');
  }
}

module.exports = new AddRank();
