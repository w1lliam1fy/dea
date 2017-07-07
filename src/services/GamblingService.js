const db = require('../database');
const config = require('../config.json');
const util = require('../utility');

class GamblingService {
  async gamble(context, bet, odds, payoutMultiplier) {
    if (bet < config.minBet) {
      return util.Messenger.replyError(context.channel, context.author, 'The minimum bet is ' + util.NumberUtil.USD(config.minBet) + '.');
    }

    const dbUser = await db.userRepo.getUser(context.author.id, context.guild.id);

    if (bet > util.NumberUtil.realValue(dbUser.cash)) {
      return util.Messenger.replyError(context.channel, context.author, 'You do not have enough money. Balance: ' + util.NumberUtil.format(dbUser.cash) + '.');
    }

    const roll = util.Random.roll();

    if (roll >= odds) {
      const winnings = bet * payoutMultiplier;

      const newDbUser = await db.userRepo.findAndModifyCash(context.author.id, context.guild.id, winnings);
			
      return util.Messenger.reply(context.channel, context.author, 'You rolled: ' + roll.toFixed(2) + '. Congrats, you won ' + util.NumberUtil.USD(winnings) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    } else {
      const newDbUser = await db.userRepo.findAndModifyCash(context.author.id, context.guild.id, -bet);
			
      return util.Messenger.reply(context.channel, context.author, 'You rolled: ' + roll.toFixed(2) + '. Unfortunately, you lost ' + util.NumberUtil.USD(bet) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash));
    }
  }
}

module.exports = new GamblingService();