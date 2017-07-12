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
          name: 'member',
          key: 'member',
          type: 'member',
          default: patron.Default.Member,
          example:'Nibba You Cray#3333',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const dbUser = msg.author.id === args.member.id ? msg.dbUser : await db.userRepo.getUser(args.member.id, msg.guild.id);

    return util.Messenger.send(msg.channel, util.StringUtil.boldify(args.member.user.tag) + '\'s balance: ' + util.NumberUtil.format(dbUser.cash));
  }
}

module.exports = new Money();
