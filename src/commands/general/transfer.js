const patron = require('patron');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');

class Transfer extends patron.Command {
  constructor() {
    super({
      name: 'transfer',
      aliases: ['sauce'],
      group: 'general',
      description: 'Transfer money to any user.',
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          example: '"Nilly Nonka#6969"'
        }), 
        new patron.Argument({
          name: 'transfer',
          key: 'transfer',
          type: 'float',
          example: '500'
        })
      ]
    });
  }

  async run(context, args) {
    if (args.transfer < config.minTransfer) {
      return util.Messenger.replyError(context.channel, context.author, 'The minimum transfer is ' + util.NumberUtil.USD(config.minTransfer) + '.');
    }

    const dbUser = await db.userRepo.getUser(context.author.id, context.guild.id);

    if (args.transfer > util.NumberUtil.realValue(dbUser.cash)) {
      return util.Messenger.replyError(context.channel, context.author, 'You do not have enough money. Balance: ' + util.NumberUtil.format(dbUser.cash) + '.');
    }

    const transactionFee = args.transfer * config.transactionCut;
    const received = args.transfer - transactionFee;
    const newDbUser = await db.userRepo.findAndModifyCash(context.author.id, context.guild.id, -args.transfer);
    await db.userRepo.modifyCash(args.user.id, context.guild.id, received);

    return util.Messenger.reply(context.channel, context.author, 'You have successfully transfered ' + util.NumberUtil.USD(received) + ' to '+ util.StringUtil.boldify(args.user.tag) + 
      '. Transaction fee: ' + util.NumberUtil.USD(transactionFee) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash) + '.');
  }
}

module.exports = new Transfer();
