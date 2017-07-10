const patron = require('patron.js');
const util = require('../utility');

class Cash extends patron.ArgumentPrecondition {
  async run(command, msg, argument, value) {
    const realValue = util.NumberUtil.realValue(msg.dbUser.cash);

    if (value <= realValue) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You do not have enough money. Balance: ' + util.NumberUtil.USD(realValue) + '.');
  }
}

module.exports = new Cash();
