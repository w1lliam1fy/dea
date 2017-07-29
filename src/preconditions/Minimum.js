const patron = require('patron.js');

class Minimum extends patron.ArgumentPrecondition {
  constructor(minimum) {
    super();
    this.minimum = minimum;
  }

  async run(command, msg, argument, value) {
    if (value >= this.minimum) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'The minimum ' + argument.name + ' is ' + this.minimum + '.');
  }
}

module.exports = Minimum;
