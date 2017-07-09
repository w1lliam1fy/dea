const db = require('../database');
const config = require('../config.json');
const util = require('../utility');

class GamblingService {
  async gamble(msg, bet, odds, payoutMultiplier) {
    if (bet < config.minBet) {
      return util.Messenger.replyError(msg.channel, msg.author, 'The minimum bet is ' + util.NumberUtil.USD(config.minBet) + '.');
    }

    const dbUser = await db.userRepo.getUser(msg.author.id, msg.guild.id);

    if (bet > util.NumberUtil.realValue(dbUser.cash)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'You do not have enough money. Balance: ' + util.NumberUtil.format(dbUser.cash) + '.');
    }

    const roll = util.Random.roll();

    if (roll >= odds) {
      const winnings = bet * payoutMultiplier;

      const newDbUser = await db.userRepo.findAndModifyCash(msg.author.id, msg.guild.id, winnings);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Congrats, you won ' + util.NumberUtil.USD(winnings) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    } else {
      const newDbUser = await db.userRepo.findAndModifyCash(msg.author.id, msg.guild.id, -bet);
			
      return util.Messenger.reply(msg.channel, msg.author, 'You rolled: ' + roll.toFixed(2) + '. Unfortunately, you lost ' + util.NumberUtil.USD(bet) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    }
  }
}

module.exports = new GamblingService();