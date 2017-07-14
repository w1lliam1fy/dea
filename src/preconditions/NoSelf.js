const patron = require('patron.js');

class NoSelf extends patron.ArgumentPrecondition {
  async run(command, msg, argument, value) {
    if (value.id !== msg.author.id) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You may not use this command on yourself.');
  }
}

module.exports = new NoSelf();