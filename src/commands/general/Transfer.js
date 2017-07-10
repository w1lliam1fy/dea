const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const NoSelf = require('../../preconditions/NoSelf.js');
const Cash = require('../../preconditions/Cash.js');
const Minimum = require('../../preconditions/Minimum.js');

class Transfer extends patron.Command {
  constructor() {
    super({
      name: 'transfer',
      aliases: ['sauce', 'donate'],
      group: 'general',
      description: 'Transfer money to any member.',
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          example: '"Nilly Nonka#6969"',
          preconditions: [NoSelf]
        }), 
        new patron.Argument({
          name: 'transfer',
          key: 'transfer',
          type: 'float',
          example: '500',
          preconditions: [Cash, new Minimum(config.minTransfer)]
        })
      ]
    });
  }

  async run(msg, args) {
    const transactionFee = args.transfer * config.transactionCut;
    const received = args.transfer - transactionFee;
    const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, -args.transfer);
    await db.userRepo.findAndModifyCash(msg.dbGuild, args.member, received);

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully transfered ' + util.NumberUtil.USD(received) + ' to '+ util.StringUtil.boldify(args.member.user.tag) + '. Transaction fee: ' + util.NumberUtil.USD(transactionFee) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash) + '.');
  }
}

module.exports = new Transfer();
