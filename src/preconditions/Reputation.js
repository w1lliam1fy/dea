const patron = require('patron.js');

class Reputation extends patron.Precondition {
  constructor(minRep) {
    super();
    this.minRep = minRep;
  }

  async run(command, msg) {
    if (msg.dbUser.reputation >= this.minRep) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'Only users with a reputation of ' + this.minRep + ' or higher may use this command.');
  }
}

module.exports = Reputation;