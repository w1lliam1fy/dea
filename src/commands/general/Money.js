const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class Money extends patron.Command {
  constructor() {
    super({
      name: 'bal',
      aliases: ['cash', 'balance', 'money'],
      group: 'general',
      description: 'View the wealth of anyone.',
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          default: patron.Default.Author,
          example:'Supa Hot Fire#0911',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const dbUser = msg.author.id === args.user.id ? msg.dbUser : await db.userRepo.getUser(args.user.id, msg.guild.id);

    return util.Messenger.send(msg.channel, util.StringUtil.boldify(args.user.tag) + '\'s balance: ' + util.NumberUtil.format(dbUser.cash));
  }
}

module.exports = new Money();
