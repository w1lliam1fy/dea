const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class ModifyCash extends patron.Command {
  constructor() {
    super({
      name: 'modifycash',
      aliases: ['give'],
      group: 'owners',
      description: 'Allows you to modify the cash of any user.',
      args: [
        new patron.Argument({
          name: 'amount',
          key: 'amount',
          type: 'float',
          example: '500'
        }),
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
    const newDbUser = await db.userRepo.findAndModifyCash(args.user.id, context.guild.id, args.amount);
    
    return util.Messenger.reply(context.channel, context.author, 'You have successfully modifed ' + (args.user.id === context.author.id ? 'your' : util.StringUtil.boldify(args.user.tag) + '\'s') 
      + ' balance to ' + util.NumberUtil.format(newDbUser.cash));
  }
}

module.exports = new ModifyCash();