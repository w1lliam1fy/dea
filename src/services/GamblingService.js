const db = require('../database');
const config = require('../config.json');
const util = require('../utility');

class GamblingService {
  async gamble(msg, bet, odds, payoutMultiplier) {
    if (bet < config.minBet) {
      return util.Messenger.replyError(msg.channel, msg.author, 'The minimum bet is ' + util.NumberUtil.USD(config.minBet) + '.');
    }

    if (bet > util.NumberUtil.realValue(msg.dbUser.cash)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'You do not have enough money. Balance: ' + util.NumberUtil.format(msg.dbUser.cash) + '.');
    }

    const roll = util.Random.roll();

    if (roll >= odds) {
      const winnings = bet * payoutMultiplier;

      const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, winnings);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Congrats, you won ' + util.NumberUtil.USD(winnings) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    } else {
      const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, -bet);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Unfortunately, you lost ' + util.NumberUtil.USD(bet) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    }
  }
}

module.exports = new GamblingService();