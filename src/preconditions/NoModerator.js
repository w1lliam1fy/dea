const ModerationService = require('../services/ModerationService.js');
const patron = require('patron.js');

class NoModerator extends patron.ArgumentPrecondition {
  async run(command, msg, argument, value) {
    if (ModerationService.getPermLevel(msg.dbGuild, value) === 0) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You may not use this command on a moderator.');
  }
}

module.exports = new NoModerator();
