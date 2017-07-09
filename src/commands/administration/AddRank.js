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

  async run(context, args) {
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);

    if (args.role.comparePositionTo(context.guild.me.highestRole) > 0) {
      return util.Messenger.replyError(context.channel, context.author, 'DEA must be higher in the heigharhy than ' + args.role);
    }

    if (dbGuild.roles.rank.some((role) => role.id === args.role.id)) {
      return util.Messenger.replyError(context.channel, context.author, 'This rank role has already been set.');
    }

    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Push('roles.rank', { id: args.role.id, cashRequired: Math.round(args.cashRequired) }));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully added the rank role ' + args.role + ' with a cash required amount of ' + args.cashRequired + '.');
  }
}

module.exports = new AddRank();
