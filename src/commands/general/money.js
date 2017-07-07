const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class Money extends patron.Command {
  constructor() {
    super({
      name: 'money',
      aliases: ['cash', 'balance', 'bal'],
      group: 'general',
      description: 'View the wealth of anyone.',
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          default: patron.Default.Author,
          example:'Supa Hot Fire#0911',
          remainder: true
        })
      ]
    });
  }

  async run(context, args) {
    const dbUser = await db.userRepo.getUser(args.user.id, context.guild.id);

    return util.Messenger.send(context.channel, util.StringUtil.boldify(args.user.tag) + '\'s balance: ' + util.NumberUtil.format(dbUser.cash));
  }
}

module.exports = new Money();
