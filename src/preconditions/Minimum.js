const patron = require('patron.js');
const util = require('../utility');

class Minimum extends patron.ArgumentPrecondition {
  constructor(minimum) {
    super();
    this.minimum = minimum;
  }

  async run(command, msg, argument, value) {
    if (value >= this.minimum) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'The minimum '+ argument.name +' is '+ util.NumberUtil.USD(this.minimum) +'.');
  }
}

module.exports = Minimum;
