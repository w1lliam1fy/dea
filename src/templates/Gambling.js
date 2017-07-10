const patron = require('patron.js');
const db = require('../database');
const util = require('../utility');
const config = require('../config.json');
const Cash = require('../preconditions/Cash.js');
const Minimum = require('../preconditions/Minimum.js');

class Gambling extends patron.Command {
  constructor(name, description, odds, payoutMultiplier) {
    super({
      name: name,
      group: 'gambling',
      description: description,
      args: [
        new patron.Argument({
          name: 'bet',
          key: 'bet',
          type: 'float',
          example: '500',
          preconditions: [Cash, new Minimum(config.minBet)]
        })
      ]
    });

    this.odds = odds;
    this.payoutMultiplier = payoutMultiplier;
  }

  async run(msg, args) {
    const roll = util.Random.roll();

    if (roll >= this.odds) {
      const winnings = args.bet * this.payoutMultiplier;

      const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, winnings);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Congrats, you won ' + util.NumberUtil.USD(winnings) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    } else {
      const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, -args.bet);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Unfortunately, you lost ' + util.NumberUtil.USD(args.bet) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    }
  }
}

module.exports = Gambling;
