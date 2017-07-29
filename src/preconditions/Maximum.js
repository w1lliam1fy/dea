const patron = require('patron.js');

class Maximum extends patron.ArgumentPrecondition {
  constructor(maximum) {
    super();
    this.maximum = maximum;
  }

  async run(command, msg, argument, value) {
    if (value <= this.maximum) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'The maximum ' + argument.name + ' is ' + this.maximum + '.');
  }
}

module.exports = Maximum;
